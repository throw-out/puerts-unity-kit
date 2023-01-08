using System;
using System.Collections.Generic;
using System.Threading;
using Puerts;
using UnityEngine;

namespace XOR
{
    public class ThreadWorker : MonoBehaviour, IDisposable
    {
        public static ThreadWorker Create(ILoader loader) => Create(loader, null);
        public static ThreadWorker Create(ILoader loader, string filepath)
        {
            GameObject gameObject = new GameObject($"{nameof(ThreadWorker)}");
            DontDestroyOnLoad(gameObject);
            ThreadWorker worker = gameObject.AddComponent<ThreadWorker>();

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

            return worker;
        }


        /// <summary>线程锁定超时(毫秒) </summary>
        private const int THREAD_LOCK_TIMEOUT = 1000;
        /// <summary>线程休眠时间(毫秒) </summary>
        private const int THREAD_SLEEP = 5;
        /// <summary>每次处理的事件数量</summary>
        private const int PROCESS_EVENT_COUNT = 5;
        /// <summary>Unity主线程ID </summary>
        private static readonly int MAIN_THREAD_ID = Thread.CurrentThread.ManagedThreadId;
        /// <summary>ts ThreadWorker脚本 </summary>
        private static readonly string TS_THREAD_WORKER_SCRIPT = "";

        //消息接口
        public Func<string, EventData, EventData> MainThreadHandler;
        public Func<string, EventData, EventData> ChildThreadHandler;
        //消息缓存
        private Queue<Event> _mainThreadMessages;
        private Queue<Event> _childThreadMessages;
        private Queue<Tuple<string, string>> _childThreadEval;

        public bool IsAlive
        {
            get
            {
                return this != null && this._running &&
                    this._thread != null && this._thread.IsAlive;
            }
        }
        public ThreadSyncr Syncr
        {
            get
            {
                if (!this.IsAlive)
                    throw new ThreadStateException($"{nameof(ThreadWorker)} not work.");
                return this._syncr;
            }
        }

        public JsEnv Env { get; private set; }
        public ILoader Loader { get; private set; }
        public ThreadLoader ThreadLoader { get; private set; }
        //跨线程同步
        private bool _running = false;
        private bool _syncing = false;
        private Thread _thread;
        private RWLocker _locker;
        private ThreadSyncr _syncr;

        void Start()
        {
            if (Loader == null)
            {
                this.enabled = false;
                UnityEngine.Debug.LogWarning($"{nameof(ThreadWorker)} instance is disable, loader instance required.");
            }
        }
        void FixedUpdate()
        {
            if (!this.IsAlive) return;
            ProcessMainThreadMessages();
            _syncr.ProcessChildThredMessages();
            ThreadLoader?.Process();
        }
        void OnDestroy()
        {
            Dispose();
        }

        public void Run(string filepath)
        {
            if (this.Env != null || this._thread != null || this._running)
                throw new Exception("Thread is running");
            if (this.Loader == null)
                throw new Exception("Thread cannot work, loader instance required.");
            if (!this.enabled)
                throw new Exception("Thread cannot work, main thread is disable");

            _running = true;
            _syncing = false;
            _thread = new Thread(new ThreadStart(() => ThreadExecute(filepath)));
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
            lock (_mainThreadMessages)
            {
                _mainThreadMessages.Enqueue(new Event()
                {
                    name = eventName,
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
            lock (_childThreadMessages)
            {
                _childThreadMessages.Enqueue(new Event()
                {
                    name = eventName,
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
            lock (_childThreadEval)
            {
                _childThreadEval.Enqueue(new Tuple<string, string>(chunk, chunkName));
            }
        }


        public void Dispose()
        {
            MainThreadHandler = null;
            ChildThreadHandler = null;
            _running = false;
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


        void ThreadExecute(string filepath)
        {
            JsEnv env = null;
            try
            {
                // JsEnv脚本放在Resource目录下,故ILoader仅允许在主线程调用
                // 子线程_SyncLoader接口会阻塞线程, 直到主线程调用ILoader后才会继续执行
                // JsEnv初始化时将调用_SyncLoader接口
                env = Env = new JsEnv(Loader);
                env.UsingAction<string, string>();
                env.UsingAction<string, EventData>();
                env.UsingFunc<string, EventData, object>();
                env.UsingFunc<string, EventData, EventData>();
                env.TryAutoUsing();
                env.Eval(TS_THREAD_WORKER_SCRIPT);
                env.Eval<Action<ThreadWorker>>(@"(function (_w){ (this ?? globalThis)['globalWorker'] = new ThreadWorker(_w); })")(this);
                env.Eval(string.Format("require(\"{0}\")", filepath));

                while (env == this.Env && IsAlive)
                {
                    env.Tick();
                    ProcessChildThreadMessages();
                    _syncr.ProcessChildThredMessages();
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
            if (_mainThreadMessages.Count == 0)
                return;
            List<Event> events = new List<Event>();
            lock (_mainThreadMessages)
            {
                int count = PROCESS_EVENT_COUNT;
                while (count-- > 0 && _mainThreadMessages.Count > 0)
                    events.Add(_mainThreadMessages.Dequeue());
            }

            Func<string, EventData, EventData> func = this.MainThreadHandler;
            if (func != null)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    try
                    {
                        func(events[i].name, events[i].data);
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
            if (_childThreadMessages.Count == 0)
                return;
            List<Event> events = new List<Event>();
            lock (_childThreadMessages)
            {
                int count = PROCESS_EVENT_COUNT;
                while (count-- > 0 && _childThreadMessages.Count > 0)
                    events.Add(_childThreadMessages.Dequeue());
            }
            Func<string, EventData, EventData> func = this.ChildThreadHandler;
            if (func != null)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    try
                    {
                        func(events[i].name, events[i].data);
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
            if (_childThreadEval.Count == 0)
                return;
            List<Tuple<string, string>> chunks = new List<Tuple<string, string>>();
            lock (_childThreadEval)
            {
                int count = PROCESS_EVENT_COUNT;
                while (count-- > 0 && _childThreadEval.Count > 0)
                    chunks.Add(_childThreadEval.Dequeue());
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
                _syncr.ProcessMainThredMessages();
                ThreadLoader.Process();
            }
            else
            {
                _syncr.ProcessChildThredMessages();
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
                _locker.AcquireWriter(true);
                if (!this._syncing)
                {
                    this._syncing = true;
                    _locker.ReleaseWriter();
                    return true;
                }
                _locker.ReleaseWriter();

                ProcessAsyncing();
                if (threadSleep) Thread.Sleep(THREAD_SLEEP);
            }
            return false;
        }
        internal void ReleaseSyncing()
        {
            if (!this.IsAlive)
                return;

            _locker.AcquireWriter(true);
            this._syncing = false;
            _locker.ReleaseWriter();
        }

        private class Event
        {
            public string name;
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
}