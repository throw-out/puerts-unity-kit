using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Puerts;
using UnityEngine;

namespace XOR
{
    public class TsWorker : MonoBehaviour, IDisposable
    {
        public static TsWorker Create(ILoader loader)
        {
            return Create(loader, null);
        }
        public static TsWorker Create(ILoader loader, string filepath)
        {
            var obj = new GameObject("JsWorker");
            DontDestroyOnLoad(obj);
            var ins = obj.AddComponent<TsWorker>();
            ins._loader = new LoaderProxy(ins, loader);
            if (!string.IsNullOrEmpty(filepath))
            {
                ins.Startup(filepath);
            }

            return ins;
        }

        /// <summary>jsWorker.ts脚本</summary>
        private const string JS_WORKER_SCRIPT = "require('./common/jsWorker')";
        /// <summary>每次处理的事件数量</summary>
        private const int PROCESS_EVENT_COUNT = 5;
        /// <summary>Unity主线程ID</summary>
        private static readonly int MAIN_THREAD_ID = Thread.CurrentThread.ManagedThreadId;
        /// <summary>线程安全锁定超时时间</summary>
        private const int THREAD_LOCK_TIMEOUT = 1000;
        /// <summary>线程休眠等级 </summary>
        private const int THREAD_SLEEP = 5;

        public JsEnv JsEnv { get; private set; }
        //消息接口
        public Func<string, EventData, EventData> OnMessageOfMain { get; set; }
        public Func<string, EventData, EventData> OnMessageOfChild { get; set; }
        //线程初始完成, 且运行中
        public bool IsAlive
        {
            get
            {
                return this != null && this._running &&
                     this._thread != null && this._thread.IsAlive;
            }
        }
        //同步对象
        private LoaderProxy _loader;
        //同步状态
        private bool _syncing;
        private Sync _sync;
        public Sync sync
        {
            get
            {
                if (this._thread == null || !this._thread.IsAlive)
                    throw new Exception("Thread is stop!");
                return this._sync;
            }
        }
        //线程
        private Thread _thread;
        private bool _running = false;
        private ReaderWriterLock _locker;
        //消息集合
        private Queue<Event> _messageOfMain;
        private Queue<Event> _messageOfChild;
        private Queue<(string, string)> _messageOfEval;

        private TsWorker()
        {
            _sync = new Sync(this);
            _locker = new ReaderWriterLock();
            _messageOfMain = new Queue<Event>();
            _messageOfChild = new Queue<Event>();
            _messageOfEval = new Queue<(string, string)>();
        }
        void Start()
        {
            if (_loader == null)
            {
                this.enabled = false;
                throw new Exception("instance cannot working, loader is null");
            }
        }
        void FixedUpdate()
        {
            if (!this.IsAlive) return;
            ProcessOfMain();
            _sync.ProcessOfMain();
            _loader.Process();
        }
        void OnDestroy()
        {
            Dispose();
        }
        public void Startup(string filepath)
        {
            if (this.JsEnv != null || this._thread != null || this._running)
                throw new Exception("Thread is running, cannot start repeatedly!");
            if (this._loader == null)
                throw new Exception("Thread cannot start working, loader is null!");
            if (!this.enabled)
                throw new Exception("Thread cannot start working, main thread is disable");

            _syncing = false;
            _running = true;
            _thread = new Thread(new ThreadStart(() =>
            {
                JsEnv jsEnv = null;
                try
                {
                    // JsEnv脚本放在Resource目录下,故ILoader仅允许在主线程调用
                    // 子线程_SyncLoader接口会阻塞线程, 直到主线程调用ILoader后才会继续执行
                    // JsEnv初始化时将调用_SyncLoader接口
                    jsEnv = JsEnv = new JsEnv(_loader);
                    jsEnv.UsingAction<string, string>();
                    jsEnv.UsingAction<string, EventData>();
                    jsEnv.UsingFunc<string, EventData, object>();
                    jsEnv.UsingFunc<string, EventData, EventData>();
                    AutoUsing(jsEnv);
                    jsEnv.Eval(JS_WORKER_SCRIPT);
                    jsEnv.Eval<Action<TsWorker>>(@"(function (_w){ (this ?? globalThis)['globalWorker'] = new JsWorker(_w); })")(this);
                    jsEnv.Eval(string.Format("require(\"{0}\")", filepath));
                    while (_running && jsEnv == JsEnv && this != null)
                    {
                        jsEnv.Tick();
                        ProcessOfChild();
                        _sync.ProcessOfChild();
                        ProcessOfChildEval(jsEnv);

                        Thread.Sleep(20);
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
                    JsEnv = null;
                    if (jsEnv != null)
                    {
                        jsEnv.GlobalListenerQuit();
                        jsEnv.Tick();
                        jsEnv.Dispose();
                    }
                }
            }));
            _thread.IsBackground = true;
            _thread.Start();
        }
        public void VerifySafety(bool isMainThread)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            if (isMainThread && id != MAIN_THREAD_ID || !isMainThread && id == MAIN_THREAD_ID)
                throw new Exception("Incorrect in thread");
        }
        public void Dispose()
        {
            OnMessageOfMain = null;
            OnMessageOfChild = null;
            _running = false;
            JsEnv = null;
            if (_thread != null)
            {
                //此处仅通知线程中断, 由线程自行结束(使用Abort将导致crash<puerts>)
                _thread.Interrupt();
                //等待线程结束
                while (_thread.IsAlive) { }
                //GC
                //System.GC.Collect();
                //System.GC.WaitForPendingFinalizers();
            }
            _thread = null;
        }
        public void InvokeMain(string name, EventData data)
        {
            lock (_messageOfMain)
            {
                _messageOfMain.Enqueue(new Event()
                {
                    name = name,
                    data = data
                });
            }
        }
        public void InvokeChild(string name, EventData data)
        {
            lock (_messageOfChild)
            {
                _messageOfChild.Enqueue(new Event()
                {
                    name = name,
                    data = data
                });
            }
        }
        public void Eval(string chunk, string chunkName = "chunk")
        {
            if (chunk == null)
                return;
            lock (_messageOfEval)
            {
                _messageOfEval.Enqueue((chunk, chunkName));
            }
        }
        private void ProcessOfMain()
        {
            if (_messageOfMain.Count > 0)
            {
                List<Event> events = new List<Event>();
                lock (_messageOfMain)
                {
                    int count = PROCESS_EVENT_COUNT;
                    while (count-- > 0 && _messageOfMain.Count > 0)
                        events.Add(_messageOfMain.Dequeue());
                }
                Func<string, EventData, EventData> func = this.OnMessageOfMain;
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
        }
        private void ProcessOfChild()
        {
            if (_messageOfChild.Count > 0)
            {
                List<Event> events = new List<Event>();
                lock (_messageOfChild)
                {
                    int count = PROCESS_EVENT_COUNT;
                    while (count-- > 0 && _messageOfChild.Count > 0)
                        events.Add(_messageOfChild.Dequeue());
                }
                Func<string, EventData, EventData> func = this.OnMessageOfChild;
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
        }
        private void ProcessOfChildEval(JsEnv jsEnv)
        {
            if (_messageOfEval.Count > 0)
            {
                List<(string, string)> chunks = new List<(string, string)>();
                lock (_messageOfEval)
                {
                    int count = PROCESS_EVENT_COUNT;
                    while (count-- > 0 && _messageOfEval.Count > 0)
                        chunks.Add(_messageOfEval.Dequeue());
                }
                for (int i = 0; i < chunks.Count; i++)
                {
                    try
                    {
                        var chunk = chunks[i];
                        jsEnv.Eval(chunk.Item1, chunk.Item2);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e.Message);
                    }
                }
            }
        }
        private void ProcessAsyncing()
        {
            if (Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID)
            {
                _sync.ProcessOfMain();
                _loader.Process();
            }
            else
            {
                _sync.ProcessOfChild();
            }
        }
        /// <summary>
        /// 获取同步锁定, 返回是否成功
        /// (注:如果两条线程都锁定则会死锁(它们都在等待对方同步), 因此只能有一条线程锁定同步状态)
        /// </summary>
        internal bool AcquireSyncing()
        {
            if (!this.IsAlive)
                return false;

            var timeout = DateTime.Now + TimeSpan.FromMilliseconds(THREAD_LOCK_TIMEOUT);
            //如果未处于同步中直接返回, 否则同步后等待状态更新
            while (DateTime.Now <= timeout)
            {
                //请求锁
                _locker.AcquireWriterLock(THREAD_LOCK_TIMEOUT);
                if (!this._syncing)
                {
                    this._syncing = true;
                    _locker.ReleaseWriterLock();
                    return true;
                }
                ProcessAsyncing();
                //释放锁
                _locker.ReleaseWriterLock();
                Thread.Sleep(20);
            }
            return false;
        }
        /// <summary> 释放同步锁定 </summary>
        internal void ReleaseSyncing()
        {
            if (!this.IsAlive)
                return;

            _locker.AcquireWriterLock(THREAD_LOCK_TIMEOUT);
            this._syncing = false;
            _locker.ReleaseWriterLock();
        }

        static void AutoUsing(JsEnv env)
        {
            const string typeName = "PuertsStaticWrap.AutoStaticCodeUsing";
            var type = (from _assembly in AppDomain.CurrentDomain.GetAssemblies()
                        let _type = _assembly.GetType(typeName, false)
                        where _type != null
                        select _type).FirstOrDefault();
            if (type != null)
            {
                type.GetMethod("AutoUsing").Invoke(null, new object[] { env });
            }
        }

        /// <summary>
        /// 同步操作
        /// </summary>
        public class Sync
        {
            private TsWorker _worker = null;
            private ReaderWriterLock _locker;
            //同步数据
            private string _mainEvent = null;
            private EventData _mainEventData = null;
            private string _childEvent = null;
            private EventData _childEventData = null;

            public Sync(TsWorker worker)
            {
                this._worker = worker;
                this._locker = new ReaderWriterLock();
            }

            public object InvokeMain(string name, EventData data, bool throwOnError = true)
            {
                if (name == null) return null;
                //获取同步状态
                if (!_worker.AcquireSyncing())
                {
                    if (!throwOnError) return null;
                    throw new Exception("Other thread is syncing!");
                }
                //写入主线程
                _locker.AcquireWriterLock(THREAD_LOCK_TIMEOUT);
                this._mainEvent = name;
                this._mainEventData = data;
                _locker.ReleaseWriterLock();
                //等待主线程同步
                try
                {
                    while (_worker.IsAlive)
                    {
                        _locker.AcquireReaderLock(THREAD_LOCK_TIMEOUT);
                        if (this._mainEvent == null)
                            break;
                        _locker.ReleaseReaderLock();
                        Thread.Sleep(20);
                    }
                    return this._mainEventData;
                }
                finally
                {
                    if (_locker.IsReaderLockHeld)
                        _locker.ReleaseReaderLock();
                    _worker.ReleaseSyncing();
                }
            }
            public object InvokeChild(string name, EventData data, bool throwOnError = true)
            {
                if (name == null) return null;
                //获取同步状态
                if (!_worker.AcquireSyncing())
                {
                    if (!throwOnError) return null;
                    throw new Exception("Other thread is syncing!");
                }
                //写入子线程
                _locker.AcquireWriterLock(THREAD_LOCK_TIMEOUT);
                this._childEvent = name;
                this._childEventData = data;
                _locker.ReleaseWriterLock();
                //等待子线程同步
                try
                {
                    while (_worker.IsAlive)
                    {
                        _locker.AcquireReaderLock(THREAD_LOCK_TIMEOUT);
                        if (this._childEvent == null)
                            break;
                        _locker.ReleaseReaderLock();
                        Thread.Sleep(20);
                    }
                    return this._childEventData;
                }
                finally
                {
                    if (_locker.IsReaderLockHeld)
                        _locker.ReleaseReaderLock();
                    _worker.ReleaseSyncing();
                }
            }
            internal void ProcessOfMain()
            {
                if (this._mainEvent != null)
                {
                    Func<string, EventData, EventData> func = this._worker.OnMessageOfMain;
                    try
                    {
                        _locker.AcquireWriterLock(THREAD_LOCK_TIMEOUT);
                        EventData data = null;
                        if (this._mainEvent != null && func != null)
                            data = func(this._mainEvent, this._mainEventData);
                        this._mainEventData = data;
                    }
                    catch (Exception e)
                    {
                        this._mainEventData = null;
                        throw e;
                    }
                    finally
                    {
                        this._mainEvent = null;
                        _locker.ReleaseWriterLock();
                    }
                }
            }
            internal void ProcessOfChild()
            {
                if (this._childEvent != null)
                {
                    Func<string, EventData, EventData> func = this._worker.OnMessageOfChild;
                    try
                    {
                        _locker.AcquireWriterLock(THREAD_LOCK_TIMEOUT);
                        EventData data = null;
                        if (this._childEvent != null && func != null)
                            data = func(this._childEvent, this._childEventData);
                        this._childEventData = data;
                    }
                    catch (Exception e)
                    {
                        this._childEventData = null;
                        throw e;
                    }
                    finally
                    {
                        this._childEvent = null;
                        _locker.ReleaseWriterLock();
                    }
                }
            }
        }

        /// <summary>
        /// Loader代理: 如果某个ILoader只能在主线程访问, 则可以使用此类型实例在子线程中访问
        /// </summary>
        class LoaderProxy : ILoader
        {
            private ILoader _source;
            private TsWorker _worker = null;
            //脚本缓存
            private Dictionary<string, bool> _cacheFileExists;
            private Dictionary<string, string> _cacheFileContents;
            private Dictionary<string, string> _cacheFilePaths;

            //线程安全
            private ReaderWriterLock locker = new ReaderWriterLock();
            //同步数据
            private string _filePath = null;
            private bool _fileExists = false;
            private string _readPath = null;
            private string _readContent = null;
            private string _readDebugpath = null;

            public LoaderProxy(TsWorker worker, ILoader loader)
            {
                this._worker = worker;
                this._source = loader;
                this._cacheFileContents = new Dictionary<string, string>();
                this._cacheFilePaths = new Dictionary<string, string>();
                this._cacheFileExists = new Dictionary<string, bool>();
            }

            public bool FileExists(string filepath)
            {
                if (this._cacheFileExists.TryGetValue(filepath, out bool exists))
                {
                    return exists;
                }
                //获取同步状态
                if (!_worker.AcquireSyncing())
                    throw new Exception("Other thread is syncing!");
                //写入主线程
                locker.AcquireWriterLock(THREAD_LOCK_TIMEOUT);
                this._filePath = filepath;
                this._fileExists = false;
                locker.ReleaseWriterLock();
                //等待主线程同步
                try
                {
                    while (_worker.IsAlive)
                    {
                        locker.AcquireReaderLock(THREAD_LOCK_TIMEOUT);
                        if (this._filePath == null)
                            break;
                        locker.ReleaseReaderLock();
                        Thread.Sleep(20);
                    }
                    this._cacheFileExists.Add(filepath, this._fileExists);
                    return this._fileExists;
                }
                finally
                {
                    if (locker.IsReaderLockHeld)
                        locker.ReleaseReaderLock();
                    _worker.ReleaseSyncing();
                }
            }
            public string ReadFile(string filepath, out string debugpath)
            {
                if (this._cacheFileContents.TryGetValue(filepath, out string script))
                {
                    debugpath = this._cacheFilePaths[filepath];
                    return script;
                }
                //获取同步状态
                if (!_worker.AcquireSyncing())
                    throw new Exception("Other thread is syncing!");
                //写入主线程
                locker.AcquireWriterLock(THREAD_LOCK_TIMEOUT);
                this._readPath = filepath;
                this._readContent = null;
                this._readDebugpath = null;
                locker.ReleaseWriterLock();
                //等待主线程同步
                try
                {
                    while (_worker.IsAlive)
                    {
                        locker.AcquireReaderLock(THREAD_LOCK_TIMEOUT);
                        if (this._readPath == null)
                            break;
                        locker.ReleaseReaderLock();
                        Thread.Sleep(20);
                    }
                    this._cacheFileContents.Add(filepath, this._readContent);
                    this._cacheFilePaths.Add(filepath, this._readDebugpath);
                    debugpath = this._readDebugpath;
                    return this._readContent;
                }
                finally
                {
                    if (locker.IsReaderLockHeld)
                        locker.ReleaseReaderLock();
                    _worker.ReleaseSyncing();
                }
            }

            public void Process()
            {
                if (this._filePath != null || this._readPath != null)
                {
                    locker.AcquireWriterLock(THREAD_LOCK_TIMEOUT);
                    try
                    {
                        if (this._filePath != null)
                        {
                            this._fileExists = _source.FileExists(this._filePath);
                            this._filePath = null;
                        }
                        if (this._readPath != null)
                        {
                            this._readContent = _source.ReadFile(this._readPath, out this._readDebugpath);
                            this._readPath = null;
                        }
                    }
                    catch (Exception e)
                    {
                        this._filePath = null;
                        this._fileExists = false;
                        this._readPath = null;
                        this._readContent = null;
                        this._readDebugpath = null;
                        throw e;
                    }
                    finally
                    {
                        locker.ReleaseWriterLock();
                    }
                }
            }
        }

        private class Event
        {
            public string name;
            public EventData data;
        }
        /// <summary>
        /// js传递数据到C#(仅传递C#引用和值拷贝)
        /// </summary>
        public class EventData
        {
            /**data type */
            public EventDataType type;
            /**data value */
            public object value;
            /**info */
            public object info;
            /**object id */
            public int id = -1;
        }
        /// <summary>
        /// js传递到C#的数据类型
        /// </summary>
        public enum EventDataType
        {
            Unknown,
            Value,
            Object,
            Array,
            Function,
            /**ArrayBuffer类型为指针传递, 直接传递将因共享内存而crash */
            ArrayBuffer,
            RefObject
        }

        public static class Util
        {
            public static byte[] ToBytes(Puerts.ArrayBuffer value)
            {
                if (value != null)
                {
                    var source = value.Bytes;
                    var result = new byte[source.Length];
                    Array.Copy(source, 0, result, 0, source.Length);
                    return result;
                }
                return null;
            }
            public static Puerts.ArrayBuffer ToArrayBuffer(byte[] value)
            {
                if (value != null)
                {
                    return new Puerts.ArrayBuffer(value);
                }
                return null;
            }
        }
    }
}
