using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Puerts;
using UnityEngine;

namespace XOR
{
    public class ThreadWorker : Multiple<ThreadWorker>, IDisposable
    {
        /// <summary>线程锁定超时(毫秒) </summary>
        private const int THREAD_LOCK_TIMEOUT = 1000;
        /// <summary>线程休眠时间(毫秒) </summary>
        private const int THREAD_SLEEP = 5;
        /// <summary>每次处理的事件数量</summary>
        private const int PROCESS_EVENT_COUNT = 5;
        /// <summary>Unity主线程ID </summary>
        private static readonly int MAIN_THREAD_ID = Thread.CurrentThread.ManagedThreadId;

        /// <summary>实际存活的线程(仅用于Editor环境统计)  </summary>
        private static int realThread = 0;
        public static int RealThread => realThread;

        //消息接口
        public Func<string, EventParameter, EventParameter> MainThreadHandler;
        public Func<string, EventParameter, EventParameter> ChildThreadHandler;
        //消息缓存
        private readonly Queue<Event> mainThreadMessages;
        private readonly Queue<Event> childThreadMessages;
        private readonly Queue<Tuple<string, string>> childThreadEval;

        public bool IsAlive
        {
            get
            {
                return !this._disposed && this._running &&
                    this._thread != null && this._thread.IsAlive &&
                    (!this.IsInitialized || this.Env != null);
            }
        }
        public bool IsInitialized { get; private set; } = false;
        public ThreadSyncr Syncr
        {
            get
            {
                if (!this.IsAlive)
                    throw new ThreadStateException($"{nameof(ThreadWorker)} is not alive.");
                if (!this.IsInitialized)
                    throw new InvalidOperationException($"{nameof(ThreadWorker)} is initializing.");
                return this.syncr;
            }
        }
        public bool Disposed => this._disposed;
        public int ThreadId { get; private set; }

        public JsEnv Env { get; private set; }
        public MixerLoader Loader { get; private set; }
        public ThreadOptions Options { get; private set; }
        private bool _running = false;
        private bool _disposed = false;
        //跨线程同步
        private bool _syncing = false;
        private Thread _thread;
        private readonly RWLocker locker;
        private readonly ThreadSyncr syncr;
        private readonly HashSet<ISyncProcess> syncProcesses;

        public ThreadWorker()
        {
            this.locker = new RWLocker(THREAD_LOCK_TIMEOUT);
            this.syncr = new ThreadSyncr(this);
            this.syncProcesses = new HashSet<ISyncProcess>();
            this.mainThreadMessages = new Queue<Event>();
            this.childThreadMessages = new Queue<Event>();
            this.childThreadEval = new Queue<Tuple<string, string>>();
            this.Init();
        }
        ~ThreadWorker()
        {
            this.Dispose();
        }

        public void Tick()
        {
            if (!this.IsAlive) return;
            ProcessMainThreadMessages();
            syncr.ProcessMainThreadMessages();
            ProcessMainThreadProcesses();
        }

        public void Run(string filepath)
        {
            if (this.Env != null || this._thread != null || this._running)
                throw new Exception("Thread is running");
            if (this.Loader == null)
                throw new Exception("Thread cannot work, loader instance required.");

            this.IsInitialized = false;
            this._running = true;
            this._syncing = false;
            this._thread = new Thread(new ThreadStart(() => ThreadExecute(filepath)));
            this._thread.IsBackground = true;
            this._thread.Start();
        }
        public static bool VerifyThread(bool isMainThread, bool throwError = true)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            if (isMainThread && id != MAIN_THREAD_ID || !isMainThread && id == MAIN_THREAD_ID)
            {
                if (throwError)
                    throw new ThreadStateException();
                return false;
            }
            return true;
        }
        /// <summary>
        /// 子线程向主线程推送消息
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        public void PostToMainThread(string eventName, EventParameter data, string resultEventName = null)
        {
            VerifyThread(false, true);
            Logger.Info($"XOR.{nameof(ThreadWorker)}({ThreadId}->Main) Enqueue: {eventName}");
            lock (mainThreadMessages)
            {
                mainThreadMessages.Enqueue(new Event()
                {
                    eventName = eventName,
                    resultEventName = resultEventName,
                    data = data
                });
            }
        }
        /// <summary>
        /// 主线程向子线程推送消息
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        public void PostToChildThread(string eventName, EventParameter data, string resultEventName = null)
        {
            VerifyThread(true, true);
            Logger.Info($"XOR.{nameof(ThreadWorker)}(Main->{ThreadId}) Enqueue: {eventName}");
            lock (childThreadMessages)
            {
                childThreadMessages.Enqueue(new Event()
                {
                    eventName = eventName,
                    resultEventName = resultEventName,
                    data = data
                });
            }
        }
        /// <summary>
        /// 主线程向子线程推送一段js代码并eval执行
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunkName"></param>
        public void PostEvalToChildThread(string chunk, string chunkName = "chunk")
        {
            VerifyThread(true, true);
            if (string.IsNullOrEmpty(chunk))
                return;
            lock (childThreadEval)
            {
                childThreadEval.Enqueue(new Tuple<string, string>(chunk, chunkName));
            }
        }


        public override void Release()
        {
            base.Release();
            this.Dispose();
        }
        public void Dispose()
        {
            if (this._disposed)
                return;
            Env = null;
            //等待syncing完成并锁定不在接收其他请求
            //while (!AcquireSyncing(false)) { };

            this._disposed = true;
            this._running = false;
            MainThreadHandler = null;
            ChildThreadHandler = null;

            if (_thread != null)
            {
                //此处仅通知线程抛出异常自行结束(使用Abort将导致crash<puerts>)
                _thread.Interrupt();
                //等待线程结束
                //while (_thread.IsAlive) { }
                DateTime timeout = DateTime.Now + TimeSpan.FromMilliseconds(THREAD_LOCK_TIMEOUT);
                while (_thread.IsAlive && DateTime.Now < timeout) { }
                if (_thread.IsAlive)
                {
                    Logger.LogError($"<b>XOR.{nameof(ThreadWorker)}({ThreadId}): <color=red>Stop Timeout</color></b>");
                }
                //GC
                //System.GC.Collect();
                //System.GC.WaitForPendingFinalizers();
                _thread = null;
            }
            ThreadWorkerCaller.Unregister(this);
#if UNITY_EDITOR
            ThreadWorkerEditorCaller.Unregister(this);
#endif
            GC.SuppressFinalize(this);
        }

        void ThreadExecute(string filepath)
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            int port =
#if UNITY_EDITOR
            Options.debugger.port;
#else
            0;
#endif
            bool waitDebugger = port > 0 && Options.debugger.wait;

            Logger.Log($"<b>XOR.{nameof(ThreadWorker)}({ThreadId}): <color=green>Executing</color></b>");
            JsEnv env = null;
            try
            {
#if UNITY_EDITOR
                Interlocked.Increment(ref realThread);
#endif
                // JsEnv内置脚本放在Resource目录下并使用DefaultLoader(Resolutions.Load)加载, 仅允许在主线程调用
                // 子线程ThreadLoader接口会阻塞线程, 直到主线程调用ThreadLoader.Process后才会继续执行
                // JsEnv初始化时将调用ThreadLoader接口
                env = this.Env = new JsEnv(Loader, port);
                env.TryAutoUsing();
                env.TryAutoInterfaceBridge();
                env.RequireXORModules();
                env.BindXORThreadWorker(this);
                env.SupportCommonJS();
#if UNITY_EDITOR
                if (waitDebugger) env.WaitDebugger();
#endif
                ThreadExecuteRun(env, filepath, !Options.stopOnError);

                this.IsInitialized = true;
                Logger.Log($"<b>XOR.{nameof(ThreadWorker)}({ThreadId}): <color=green>Started</color></b>");
                while (env == this.Env && IsAlive)
                {
                    ThreadExecuteTick(env, !Options.stopOnError);

                    Thread.Sleep(THREAD_SLEEP);
                }
            }
            catch (ThreadInterruptedException /** e */)
            {
                //线程在休眠期间, 调用thread.Interrupt()抛出此异常, 无须处理, 不影响运行
                Logger.Log($"<b>XOR.{nameof(ThreadWorker)}({ThreadId}): <color=red>Abort</color></b>");
            }
            catch (Exception e)
            {
                Logger.LogError($"<b>XOR.{nameof(ThreadWorker)}({ThreadId}): <color=red>Exception</color></b>\n{e}");
            }
            finally
            {
                this.Env = null;
                this._running = false;
                if (env != null)
                {
                    env.GlobalListenerQuit();
                    env.Tick();
                    env.Dispose();
                }
                Logger.Log($"<b>XOR.{nameof(ThreadWorker)}({ThreadId}): <color=red>Stoped</color></b>");
#if UNITY_EDITOR
                Interlocked.Decrement(ref realThread);
#endif
            }
        }
        void ThreadExecuteRun(JsEnv env, string filepath, bool catchException)
        {
            if (catchException)
            {
                try
                {
                    env.Load(filepath);
                }
                catch (Exception e)
                {
                    Logger.LogError($"<b>XOR.{nameof(ThreadWorker)}({ThreadId}): <color=red>Exception</color></b>\n{e}");
                }
            }
            else
            {
                env.Load(filepath);
            }
        }
        void ThreadExecuteTick(JsEnv env, bool catchException)
        {
            if (catchException)
            {
                try
                {
                    env.Tick();
                    ProcessChildThreadMessages();
                    syncr.ProcessChildThreadMessages();
                    ProcessChildThreadEval();
                }
                catch (Exception e)
                {
                    Logger.LogError($"<b>XOR.{nameof(ThreadWorker)}({ThreadId}): <color=red>Exception</color></b>\n{e}");
                }
            }
            else
            {
                env.Tick();
                ProcessChildThreadMessages();
                syncr.ProcessChildThreadMessages();
                ProcessChildThreadEval();
            }
        }


        void ProcessMainThreadMessages()
        {
            if (mainThreadMessages.Count == 0)
                return;
            List<Event> events = new List<Event>();
            lock (mainThreadMessages)
            {
                int count = PROCESS_EVENT_COUNT;
                while (count-- > 0 && mainThreadMessages.Count > 0)
                    events.Add(mainThreadMessages.Dequeue());
            }

            Func<string, EventParameter, EventParameter> func = this.MainThreadHandler;
            if (func != null)
            {
                EventParameter result = null;
                for (int i = 0; i < events.Count; i++)
                {
                    Event _event = events[i];
                    try
                    {
                        Logger.Info($"XOR.{nameof(ThreadWorker)}(Main<-{ThreadId}) Resolve: {_event.eventName}");
                        result = func(_event.eventName, _event.data);
                        if (!string.IsNullOrEmpty(_event.resultEventName))
                        {
                            PostToChildThread(_event.resultEventName, result);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.Message);
                        if (!string.IsNullOrEmpty(_event.resultEventName))
                        {
                            PostToChildThread(_event.resultEventName, EventParameter.Error(e));
                        }
                    }
                }
            }
            else
            {
                Logger.LogError($"{nameof(ThreadWorker)}.{nameof(MainThreadHandler)} unregister.");
            }
        }
        void ProcessMainThreadProcesses()
        {
            if (syncProcesses.Count == 0)
                return;
            foreach (ISyncProcess process in syncProcesses)
            {
                process.Process();
            }
        }
        void ProcessChildThreadMessages()
        {
            if (childThreadMessages.Count == 0)
                return;
            List<Event> events = new List<Event>();
            lock (childThreadMessages)
            {
                int count = PROCESS_EVENT_COUNT;
                while (count-- > 0 && childThreadMessages.Count > 0)
                    events.Add(childThreadMessages.Dequeue());
            }

            Func<string, EventParameter, EventParameter> func = this.ChildThreadHandler;
            if (func != null)
            {
                EventParameter result = null;
                for (int i = 0; i < events.Count; i++)
                {
                    Event _event = events[i];
                    try
                    {
                        Logger.Info($"XOR.{nameof(ThreadWorker)}({ThreadId}<-Main) Resolve: {_event.eventName}");
                        result = func(_event.eventName, _event.data);
                        if (!string.IsNullOrEmpty(_event.resultEventName))
                        {
                            PostToMainThread(_event.resultEventName, result);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.Message);
                        if (!string.IsNullOrEmpty(_event.resultEventName))
                        {
                            PostToMainThread(_event.resultEventName, EventParameter.Error(e));
                        }
                    }
                }
            }
            else
            {
                Logger.LogError($"{nameof(ThreadWorker)}.{nameof(ChildThreadHandler)} unregister.");
            }
        }
        void ProcessChildThreadEval()
        {
            if (childThreadEval.Count == 0)
                return;
            List<Tuple<string, string>> chunks = new List<Tuple<string, string>>();
            lock (childThreadEval)
            {
                int count = PROCESS_EVENT_COUNT;
                while (count-- > 0 && childThreadEval.Count > 0)
                    chunks.Add(childThreadEval.Dequeue());
            }
            for (int i = 0; i < chunks.Count; i++)
            {
                try
                {
                    var chunk = chunks[i];
                    Env.Eval(chunk.Item1, chunk.Item2);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }
            }
        }

        void ProcessAsyncing()
        {
            if (Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID)
            {
                syncr.ProcessMainThreadMessages();
                ProcessMainThreadProcesses();
            }
            else
            {
                syncr.ProcessChildThreadMessages();
            }
        }
        internal bool AcquireSyncing(bool threadSleep = true)
        {
            if (!this.IsAlive)
                return false;

            DateTime timeout = DateTime.Now + TimeSpan.FromMilliseconds(THREAD_LOCK_TIMEOUT);
            while (DateTime.Now <= timeout && this.IsAlive)
            {
                //请求锁
                locker.AcquireWriter(true);
                if (!this._syncing)
                {
                    this._syncing = true;
                    locker.ReleaseWriter();
                    return true;
                }
                locker.ReleaseWriter();

                ProcessAsyncing();
                if (threadSleep) Thread.Sleep(THREAD_SLEEP);
            }
            return false;
        }
        internal void ReleaseSyncing()
        {
            if (!this.IsAlive)
                return;

            locker.AcquireWriter(true);
            this._syncing = false;
            locker.ReleaseWriter();
        }

        public static ThreadWorker Create(ILoader loader) => Create(loader, default);
        public static ThreadWorker Create(ILoader loader, ThreadOptions options)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            throw new InvalidOperationException("environment not supported");
#endif

            ThreadWorker worker = new ThreadWorker();
            if (!UnityEngine.Application.isPlaying || options.isEditor)
            {
#if UNITY_EDITOR
                ThreadWorkerEditorCaller.Register(worker);
                options.isEditor = true;
#else
                throw new InvalidOperationException("environment not supported");
#endif
            }
            else
            {
                ThreadWorkerCaller.Register(worker);
            }

            //Create MergeLoader
            MixerLoader mloader;
            if (typeof(MixerLoader).IsAssignableFrom(loader.GetType()))
            {
                mloader = new MixerLoader((MixerLoader)loader);
            }
            else
            {
                mloader = new MixerLoader();
                mloader.AddLoader(loader);
            }
            mloader.RemoveLoader<DefaultLoader>();
            //Create ThreadLoader
            ThreadLoader tloader = new ThreadLoader(worker, new DefaultLoader());
            mloader.AddLoader(tloader, int.MaxValue, filepath => !string.IsNullOrEmpty(filepath) && (
                filepath.StartsWith("puerts/") ||
                filepath.StartsWith("puer-commonjs/")
            ));

            worker.Loader = mloader;
            worker.syncProcesses.Add(tloader);
            worker.Options = options;

            if (!string.IsNullOrEmpty(options.filepath))
            {
                worker.Run(options.filepath);
            }
            return worker;
        }

        private class Event
        {
            public string eventName;
            public string resultEventName;
            public EventParameter data;
        }
        public class EventParameter
        {
            private Dictionary<uint, object> references;
            public Puerts.ArrayBuffer Data { get; private set; }
            public Exception Exception { get; private set; }
            public bool IsError => Exception != null;

            public EventParameter() : this(null) { }
            public EventParameter(Puerts.ArrayBuffer data)
            {
                this.Data = data;
            }

            public void AddReference(uint objId, object obj)
            {
                if (references == null)
                    references = new Dictionary<uint, object>();
                references.Add(objId, obj);
            }
            public uint[] GetReferenceKeys()
            {
                if (references == null)
                    return null;
                return references.Keys.ToArray();
            }
            public object GetReferenceValue(uint objId)
            {
                if (references == null)
                    return null;
                references.TryGetValue(objId, out var result);
                return result;
            }

            public static EventParameter Error(Exception e)
            {
                return new EventParameter()
                {
                    Exception = e
                };
            }
        }
    }
    public struct ThreadOptions
    {
        public string filepath;
        /// <summary>
        /// 创建remote代理端口
        /// </summary>
        public bool remote;
        /// <summary>
        /// 发生错误时停止执行
        /// </summary>
        public bool stopOnError;
        /// <summary>
        /// 是否为Editor环境(线程将使用EditorApplication.update调用Tick/不会随stop play销毁线程)
        /// </summary>
        public bool isEditor;

        public ThreadDebuggerOptions debugger;
    }
    public struct ThreadDebuggerOptions
    {
        /// <summary>
        /// debugger监听端口
        /// </summary>
        public ushort port;
        /// <summary>
        /// 等待waitDebugger调试器
        /// </summary>
        public bool wait;
    }

    internal class ThreadWorkerCaller : MonoBehaviour
    {
        static HashSet<ThreadWorker> _instances;
        static ThreadWorkerCaller _instance;
        static object locker = new object();

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                UnityEngine.Object.DestroyImmediate(this);
                return;
            }
            _instance = this;
        }
        void FixedUpdate()
        {
            if (_instances == null)
                return;
            foreach (ThreadWorker worker in _instances)
            {
                if (worker.Disposed || !worker.IsAlive)
                    continue;
                worker.Tick();
            }
        }
        void OnDestroy()
        {

        }
        static void TryCreateCaller()
        {
            if (_instance != null)
                return;

            lock (locker)
            {
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject($"{nameof(ThreadWorkerCaller)}");
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                    _instance = gameObject.AddComponent<ThreadWorkerCaller>();
                }
            }
        }
        public static void Register(ThreadWorker worker)
        {
            if (worker == null || worker.Disposed)
                return;

            if (_instances == null)
            {
                lock (locker)
                {
                    if (_instances == null) _instances = new HashSet<ThreadWorker>();
                }
            }
            _instances.Add(worker);

            TryCreateCaller();
        }
        public static void Unregister(ThreadWorker worker)
        {
            if (_instances == null)
                return;
            _instances.Remove(worker);
        }
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    internal static class ThreadWorkerEditorCaller
    {
#if UNITY_EDITOR
        static ThreadWorkerEditorCaller()
        {
            RegisterHandlers();
        }
        static void RegisterHandlers()
        {
            UnityEditor.EditorApplication.update += Update;
            if (AppDomain.CurrentDomain != null)
            {
                AppDomain.CurrentDomain.DomainUnload += Dispose;
                AppDomain.CurrentDomain.ProcessExit += Dispose;
            }
            else
            {
                UnityEngine.Debug.LogError($"XOR.{nameof(ThreadWorkerEditorCaller)} Registered: <b><color=red>Failure</color></b>.");
            }
        }
        static void UnregisterHandlers()
        {
            UnityEditor.EditorApplication.update -= Update;
            if (AppDomain.CurrentDomain != null)
            {
                AppDomain.CurrentDomain.DomainUnload -= Dispose;
                AppDomain.CurrentDomain.ProcessExit -= Dispose;
            }
        }
#endif
        static HashSet<ThreadWorker> _instances;
        static object locker = new object();

        static void Update()
        {
            if (_instances == null)
                return;

            foreach (ThreadWorker worker in _instances)
            {
                if (worker.Disposed || !worker.IsAlive)
                    continue;
                worker.Tick();
            }
        }
        static void Dispose(object sender, EventArgs e)
        {
            HashSet<ThreadWorker> workers = null;
            lock (locker)
            {
                if (_instances != null)
                {
                    workers = _instances;
                    _instances = null;
                }
            }
            if (workers == null)
                return;

            foreach (ThreadWorker worker in workers)
            {
                if (worker.Disposed)
                    continue;
                worker.Dispose();
            }
        }

        public static void Register(ThreadWorker worker)
        {
            if (worker == null || worker.Disposed)
                return;
            if (_instances == null)
            {
                lock (locker)
                {
                    if (_instances == null) _instances = new HashSet<ThreadWorker>();
                }
            }
            _instances.Add(worker);
        }
        public static void Unregister(ThreadWorker worker)
        {
            if (_instances == null)
                return;
            _instances.Remove(worker);
        }
    }
}