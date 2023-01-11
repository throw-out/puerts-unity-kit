using System;
using System.Threading;

namespace XOR
{
    public interface ISyncProcess
    {
        void Process();
    }
    public class ThreadSyncr
    {
        /// <summary>线程锁定超时(毫秒) </summary>
        private const int THREAD_LOCK_TIMEOUT = 1000;
        private const int THREAD_SLEEP = 5;

        private readonly ThreadWorker worker = null;
        private readonly RWLocker locker;

        //同步数据
        private SyncEventData _syncToMainThread;
        private SyncEventData _syncToChildThread;

        public ThreadSyncr(ThreadWorker worker)
        {
            this.worker = worker;
            this.locker = new RWLocker(THREAD_LOCK_TIMEOUT);
        }
        /// <summary>
        /// 子线程同步调用主线程, 并获取返回值
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public ThreadWorker.EventData PostToMainThread(string eventName, ThreadWorker.EventData data, bool throwOnError = true)
        {
            worker.VerifyThread(false, true);
            //检查worker是否初始化完成
            if (!worker.IsInitialized)
            {
                if (throwOnError)
                    throw new InvalidOperationException($"{nameof(ThreadWorker)} is initializing.");
                Logger.LogWarning($"{nameof(ThreadWorker)} is initializing.");
                return null;
            }
            //锁定ThreadWorker同步状态
            if (!worker.AcquireSyncing())
            {
                if (throwOnError)
                    throw new ThreadStateException("Thread busy!");
                Logger.LogWarning("Thread busy!");
                return null;
            }
            try
            {
                //写入主线程
                locker.AcquireWriter();
                this._syncToMainThread = new SyncEventData(eventName, data);
                locker.ReleaseWriter();

                //等待主线程同步
                ThreadWorker.EventData result = null; Exception exception = null;
                while (worker.IsAlive)
                {
                    locker.AcquireReader();
                    bool isCompleted = this._syncToMainThread == null || this._syncToMainThread.completed;
                    if (isCompleted)
                    {
                        result = this._syncToMainThread?.result;
                        exception = this._syncToMainThread?.exception;
                    }
                    locker.ReleaseReader();

                    if (isCompleted) break;
                    Thread.Sleep(THREAD_SLEEP);
                }
                //检查错误
                if (exception != null)
                {
                    if (throwOnError) throw exception;
                    result = null;
                }
                return result;
            }
            finally
            {
                worker.ReleaseSyncing();
            }
        }
        /// <summary>
        /// 主线程同步调用子线程, 并获取返回值
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public ThreadWorker.EventData PostToChildThread(string eventName, ThreadWorker.EventData data, bool throwOnError = true)
        {
            worker.VerifyThread(true, true);
            //检查worker是否初始化完成
            if (!worker.IsInitialized)
            {
                if (throwOnError)
                    throw new InvalidOperationException($"{nameof(ThreadWorker)} is initializing.");
                Logger.LogWarning($"{nameof(ThreadWorker)} is initializing.");
                return null;
            }
            //锁定ThreadWorker同步状态
            if (!worker.AcquireSyncing())
            {
                if (throwOnError)
                    throw new ThreadStateException("Thread busy!");
                Logger.LogWarning("Thread busy!");
                return null;
            }
            try
            {
                //写入子线程
                locker.AcquireWriter();
                this._syncToChildThread = new SyncEventData(eventName, data);
                locker.ReleaseWriter();

                //等待子线程同步
                ThreadWorker.EventData result = null; Exception exception = null;
                while (worker.IsAlive)
                {
                    locker.AcquireReader();
                    bool isCompleted = this._syncToChildThread == null || this._syncToChildThread.completed;
                    if (isCompleted)
                    {
                        result = this._syncToChildThread?.result;
                        exception = this._syncToChildThread?.exception;
                    }
                    locker.ReleaseReader();

                    if (isCompleted) break;
                    Thread.Sleep(THREAD_SLEEP);
                }
                //检查错误
                if (exception != null)
                {
                    if (throwOnError) throw exception;
                    result = null;
                }
                return result;
            }
            finally
            {
                worker.ReleaseSyncing();
            }
        }

        internal void ProcessMainThredMessages() => Process(this._syncToMainThread, worker.MainThreadHandler);
        internal void ProcessChildThredMessages() => Process(this._syncToChildThread, worker.ChildThreadHandler);

        void Process(SyncEventData d, Func<string, ThreadWorker.EventData, ThreadWorker.EventData> invoke)
        {
            if (d == null || d.completed || invoke == null)
                return;

            locker.AcquireWriter();
            try
            {
                if (d == null || d.completed)
                    return;

                d.completed = true;
                d.result = invoke(d.eventName, d.data);
            }
            catch (Exception e)
            {
                d.completed = true;
                d.exception = e;
                d.result = null;
            }
            finally
            {
                locker.ReleaseWriter();
            }
        }

        class SyncEventData
        {
            public readonly string eventName;
            public readonly ThreadWorker.EventData data;
            public bool completed;
            public ThreadWorker.EventData result;
            public Exception exception;
            public SyncEventData(string eventName, ThreadWorker.EventData data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }
    }
}