using System;
using System.Threading;
using System.Collections.Generic;
using Puerts;

namespace XOR
{
    public class ThreadLoader : ILoader, ISyncProcess, IModuleChecker
    {
        /// <summary>线程锁定超时(毫秒) </summary>
        private const int THREAD_LOCK_TIMEOUT = 1000;
        private const int THREAD_SLEEP = 5;

        private readonly ThreadWorker worker;
        private readonly ILoader source;
        private readonly Func<string, bool> match;
        private readonly RWLocker locker;

        //脚本缓存
        private readonly Dictionary<string, bool> _cacheFileESM;
        private readonly Dictionary<string, bool> _cacheFileExists;
        private readonly Dictionary<string, Tuple<string, string>> _cacheReadFile;

        //跨线程同步数据
        private SyncFileESM _syncFileESM;
        private SyncFileExists _syncFileExists;
        private SyncReadFile _syncReadFile;

        public ThreadLoader(ThreadWorker worker, ILoader source) : this(worker, source, null) { }
        public ThreadLoader(ThreadWorker worker, ILoader source, Func<string, bool> match)
        {
            this.worker = worker;
            this.source = source;
            this.match = match;
            this.locker = new RWLocker(THREAD_LOCK_TIMEOUT);
            this._cacheFileExists = new Dictionary<string, bool>();
            this._cacheReadFile = new Dictionary<string, Tuple<string, string>>();
            this._cacheFileESM = new Dictionary<string, bool>();
        }


        public bool FileExists(string filepath)
        {
            if (this._cacheFileExists.TryGetValue(filepath, out bool exists))
            {
                return exists;
            }
            if (this.match != null && !this.match(filepath))
            {
                return false;
            }
            //锁定ThreadWorker同步状态
            if (!worker.AcquireSyncing())
            {
                throw new ThreadStateException("Thread busy!");
            }
            try
            {
                //写入主线程
                locker.AcquireWriter();
                this._syncFileExists = new SyncFileExists(filepath);
                locker.ReleaseWriter();
                //等待主线程同步
                bool isExists = false;
                while (worker.IsAlive)
                {
                    locker.AcquireReader();
                    bool isCompleted = this._syncFileExists == null || this._syncFileExists.completed;
                    if (isCompleted)
                    {
                        isExists = this._syncFileExists != null && this._syncFileExists.exists;
                    }
                    locker.ReleaseReader();

                    if (isCompleted) break;
                    Thread.Sleep(THREAD_SLEEP);
                }

                this._cacheFileExists.Add(filepath, isExists);

                return isExists;
            }
            finally
            {
                worker.ReleaseSyncing();
            }
        }

        public string ReadFile(string filepath, out string debugpath)
        {
            if (this._cacheReadFile.TryGetValue(filepath, out Tuple<string, string> script))
            {
                debugpath = script.Item1;
                return script.Item2;
            }
            if (this.match != null && !this.match(filepath))
            {
                debugpath = null;
                return null;
            }
            //锁定ThreadWorker同步状态
            if (!worker.AcquireSyncing())
            {
                throw new ThreadStateException("Thread busy!");
            }
            try
            {
                //写入主线程
                locker.AcquireWriter();
                this._syncReadFile = new SyncReadFile(filepath);
                locker.ReleaseWriter();

                //等待主线程同步
                string _content = null, _debugpath = null;
                while (worker.IsAlive)
                {
                    locker.AcquireReader();
                    bool isCompleted = this._syncReadFile == null || this._syncReadFile.completed;
                    if (isCompleted)
                    {
                        _debugpath = this._syncReadFile != null ? _syncReadFile.debugpath : null;
                        _content = this._syncReadFile != null ? _syncReadFile.content : null;
                    }
                    locker.ReleaseReader();

                    if (isCompleted) break;
                    Thread.Sleep(THREAD_SLEEP);
                }
                debugpath = _debugpath;
                this._cacheReadFile.Add(filepath, new Tuple<string, string>(_debugpath, _content));

                return _content;
            }
            finally
            {
                worker.ReleaseSyncing();
            }
        }
        public bool IsESM(string filepath)
        {
            if (this._cacheFileESM.TryGetValue(filepath, out bool _isESM))
            {
                return _isESM;
            }
            if (this.match != null && !this.match(filepath))
            {
                return false;
            }
            //锁定ThreadWorker同步状态
            if (!worker.AcquireSyncing())
            {
                throw new ThreadStateException("Thread busy!");
            }
            try
            {
                //写入主线程
                locker.AcquireWriter();
                this._syncFileESM = new SyncFileESM(filepath);
                locker.ReleaseWriter();
                //等待主线程同步
                bool isESM = false;
                while (worker.IsAlive)
                {
                    locker.AcquireReader();
                    bool isCompleted = this._syncFileESM == null || this._syncFileESM.completed;
                    if (isCompleted)
                    {
                        isESM = this._syncFileESM != null && this._syncFileESM.isESM;
                    }
                    locker.ReleaseReader();

                    if (isCompleted) break;
                    Thread.Sleep(THREAD_SLEEP);
                }

                this._cacheFileESM.Add(filepath, isESM);

                return isESM;
            }
            finally
            {
                worker.ReleaseSyncing();
            }
        }

        public void Process()
        {
            if (this._syncFileExists != null && !this._syncFileExists.completed ||
                this._syncReadFile != null && !this._syncReadFile.completed ||
                this._syncFileESM != null && !this._syncFileESM.completed)
            {
                locker.AcquireWriter();
                try
                {
                    if (this._syncFileExists != null && !this._syncFileExists.completed)
                    {
                        string filepath = this._syncFileExists.filepath;

                        this._syncFileExists.completed = true;
                        this._syncFileExists.exists = source.FileExists(filepath);
                    }
                    if (this._syncReadFile != null && !this._syncReadFile.completed)
                    {
                        string filepath = this._syncReadFile.filepath;
                        string debugpath;
                        string content = source.ReadFile(filepath, out debugpath);

                        this._syncReadFile.completed = true;
                        this._syncReadFile.debugpath = debugpath;
                        this._syncReadFile.content = content;
                    }
                    if (this._syncFileESM != null && !this._syncFileESM.completed)
                    {
                        string filepath = this._syncFileESM.filepath;

                        this._syncFileESM.completed = true;
                        this._syncFileESM.isESM = source is IModuleChecker && ((IModuleChecker)source).IsESM(filepath);
                    }
                }
                catch (Exception e)
                {
                    this._syncFileExists = null;
                    this._syncReadFile = null;
                    this._syncFileESM = null;
                    throw e;
                }
                finally
                {
                    locker.ReleaseWriter();
                }
            }
        }
        class SyncFileESM
        {
            public readonly string filepath;
            public bool completed;
            public bool isESM;

            public SyncFileESM(string filepath)
            {
                this.filepath = filepath;
            }
        }
        class SyncFileExists
        {
            public readonly string filepath;
            public bool completed;
            public bool exists;

            public SyncFileExists(string filepath)
            {
                this.filepath = filepath;
            }
        }
        class SyncReadFile
        {
            public readonly string filepath;
            public bool completed;
            public string content;
            public string debugpath;

            public SyncReadFile(string filepath)
            {
                this.filepath = filepath;
            }
        }
    }
}