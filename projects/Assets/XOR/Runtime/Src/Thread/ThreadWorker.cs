using System;
using System.Collections.Generic;
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

        //消息接口
        public Func<string, EventData, EventData> MainThreadHandler;
        public Func<string, EventData, EventData> ChildThreadHandler;
        //消息缓存
        private readonly Queue<Event> mainThreadMessages;
        private readonly Queue<Event> childThreadMessages;
        private readonly Queue<Tuple<string, string>> childThreadEval;

        public bool IsAlive
        {
            get
            {
                return !this._disposed && this._running &&
                    this._thread != null && this._thread.IsAlive;
            }
        }
        public ThreadSyncr Syncr
        {
            get
            {
                if (!this.IsAlive)
                    throw new ThreadStateException($"{nameof(ThreadWorker)} not work.");
                return this.syncr;
            }
        }
        public bool Disposed => this._disposed;

        public JsEnv Env { get; private set; }
        public ILoader Loader { get; private set; }
        public ThreadLoader ThreadLoader { get; private set; }
        private bool _running = false;
        private bool _disposed = false;
        //跨线程同步
        private bool _syncing = false;
        private Thread _thread;
        private readonly RWLocker locker;
        private readonly ThreadSyncr syncr;

        public ThreadWorker()
        {
            this.locker = new RWLocker(THREAD_LOCK_TIMEOUT);
            this.syncr = new ThreadSyncr(this);
            this.mainThreadMessages = new Queue<Event>();
            this.childThreadMessages = new Queue<Event>();
            this.childThreadEval = new Queue<Tuple<string, string>>();
        }
        ~ThreadWorker()
        {
            this.Dispose();
        }

        public void Tick()
        {
            if (!this.IsAlive) return;
            ProcessMainThreadMessages();
            syncr.ProcessChildThredMessages();
            ThreadLoader?.Process();
        }

        public void Run(string filepath)
        {
            if (this.Env != null || this._thread != null || this._running)
                throw new Exception("Thread is running");
            if (this.Loader == null)
                throw new Exception("Thread cannot work, loader instance required.");

            bool isESM = Settings.Load().IsESM;

            _running = true;
            _syncing = false;
            _thread = new Thread(new ThreadStart(() => ThreadExecute(isESM, filepath)));
            _thread.IsBackground = true;
            _thread.Start();
        }
        public bool VerifyThread(bool isMainThread, bool throwError = true)
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
        public void PostToMainThread(string eventName, EventData data)
        {
            VerifyThread(false, true);
            lock (mainThreadMessages)
            {
                mainThreadMessages.Enqueue(new Event()
                {
                    eventName = eventName,
                    data = data
                });
            }
        }
        /// <summary>
        /// 主线程向子线程推送消息
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        public void PostToChildThread(string eventName, EventData data)
        {
            VerifyThread(true, true);
            lock (childThreadMessages)
            {
                childThreadMessages.Enqueue(new Event()
                {
                    eventName = eventName,
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
#if UNITY_EDITOR
            ThreadWorkerEditorCaller.Unregister(this);
#endif
            //等待syncing完成并锁定不在接收其他请求
            while (!AcquireSyncing(false)) { };

            this._disposed = true;
            this._running = false;
            MainThreadHandler = null;
            ChildThreadHandler = null;
            Env = null;
            if (_thread != null)
            {
                //此处仅通知线程抛出异常自行结束(使用Abort将导致crash<puerts>)
                _thread.Interrupt();
                //等待线程结束
                while (_thread.IsAlive) { }
                //GC
                //System.GC.Collect();
                //System.GC.WaitForPendingFinalizers();

                _thread = null;
            }
        }

        void ThreadExecute(bool isESM, string filepath)
        {
            JsEnv env = null;
            try
            {
                // JsEnv内置脚本放在Resource目录下并使用DefaultLoader加载,故仅允许在主线程调用
                // 子线程ThreadLoader接口会阻塞线程, 直到主线程调用ThreadLoader.Process后才会继续执行
                // JsEnv初始化时将调用ThreadLoader接口
                env = Env = new JsEnv(Loader);
                env.TryAutoUsing();
                env.SupportCommonJS();
                env.RequireXORModules(isESM);
                env.BindThreadWorker(this);
                env.Eval(string.Format("require('{0}')", filepath));

                while (env == this.Env && IsAlive)
                {
                    env.Tick();
                    ProcessChildThreadMessages();
                    syncr.ProcessChildThredMessages();
                    ProcessChildThreadEval();

                    Thread.Sleep(THREAD_SLEEP);
                }
            }
            catch (ThreadInterruptedException /** e */)
            {
                //线程在休眠期间, 调用thread.Interrupt()抛出此异常, 此处不处理, 不影响运行
                //UnityEngine.Debug.Log(e.ToString());
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.ToString());
            }
            finally
            {
                Env = null;
                if (env != null)
                {
                    env.GlobalListenerQuit();
                    env.Tick();
                    env.Dispose();
                }
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

            Func<string, EventData, EventData> func = this.MainThreadHandler;
            if (func != null)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    try
                    {
                        func(events[i].eventName, events[i].data);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e.Message);
                    }
                }
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
            Func<string, EventData, EventData> func = this.ChildThreadHandler;
            if (func != null)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    try
                    {
                        func(events[i].eventName, events[i].data);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e.Message);
                    }
                }
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
                    UnityEngine.Debug.LogError(e.Message);
                }
            }
        }

        void ProcessAsyncing()
        {
            if (Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID)
            {
                syncr.ProcessMainThredMessages();
                ThreadLoader.Process();
            }
            else
            {
                syncr.ProcessChildThredMessages();
            }
        }
        internal bool AcquireSyncing(bool threadSleep = true)
        {
            if (!this.IsAlive)
                return false;

            DateTime timeout = DateTime.Now + TimeSpan.FromMilliseconds(THREAD_LOCK_TIMEOUT);
            while (DateTime.Now <= timeout)
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

        public static ThreadWorker Create(ILoader loader) => Create(loader, null);
        public static ThreadWorker Create(ILoader loader, string filepath)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            throw new InvalidOperationException();
#endif

            ThreadWorker worker = new ThreadWorker();
            if (!UnityEngine.Application.isPlaying)
            {
#if UNITY_EDITOR
                ThreadWorkerEditorCaller.Register(worker);
#endif
            }
            else
            {
                UnityEngine.Object.DontDestroyOnLoad(ThreadWorkerCaller.Register(worker));
            }

            //Create MergeLoader
            MergeLoader mloader;
            if (loader is MergeLoader)
            {
                mloader = new MergeLoader((MergeLoader)loader);
            }
            else
            {
                mloader = new MergeLoader();
                mloader.AddLoader(loader);
            }
            mloader.RemoveLoader<DefaultLoader>();
            //Create ThreadLoader
            ThreadLoader tloader = new ThreadLoader(worker, new DefaultLoader());
            mloader.AddLoader(tloader, int.MaxValue);

            worker.Loader = mloader;
            worker.ThreadLoader = tloader;

            if (!string.IsNullOrEmpty(filepath))
            {
                worker.Run(filepath);
            }
            return worker;
        }

        private class Event
        {
            public string eventName;
            public EventData data;
        }
        public class EventData
        {
            public ValueType Type;
            public object Value;
            /// <summary>当Type为Array/Object时, 此字段有效 </summary>
            public object Key;
            /// <summary>当Type为RefObject时, 此字段有效 </summary>
            public int Id = -1;
        }
        public enum ValueType
        {
            Unknown,
            Value,
            Object,
            Array,
            ArrayBuffer,
            JSON,
            RefObject
        }
    }

    internal class ThreadWorkerCaller : MonoBehaviour
    {
        public ThreadWorker worker;

        void FixedUpdate()
        {
            if (worker == null || worker.Disposed)
            {
                GameObject.Destroy(gameObject);
            }
            else
            {
                worker.Tick();
            }
        }
        void OnDestroy()
        {
            worker?.Dispose();
            worker = null;
        }

        public static GameObject Register(ThreadWorker worker)
        {
            if (worker == null || worker.Disposed)
                return null;
            GameObject gameObject = new GameObject($"{nameof(ThreadWorkerCaller)}");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            ThreadWorkerCaller caller = gameObject.AddComponent<ThreadWorkerCaller>();
            caller.worker = worker;
            return gameObject;
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
                UnityEngine.Debug.Log($"XOR.{nameof(ThreadWorkerEditorCaller)} Registered: <b><color=red>Failure</color></b>.");
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

        static void Update()
        {
            if (_instances == null)
                return;

            foreach (ThreadWorker worker in _instances)
            {
                if (worker.Disposed)
                    continue;
            }
        }
        static void Dispose(object sender, EventArgs e)
        {
            if (_instances == null)
                return;

            foreach (ThreadWorker worker in _instances)
            {
                if (worker.Disposed)
                    continue;
                worker.Dispose();
            }
            _instances = null;
        }

        public static void Register(ThreadWorker worker)
        {
            if (worker == null || worker.Disposed)
                return;
            if (_instances == null)
                _instances = new HashSet<ThreadWorker>();
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