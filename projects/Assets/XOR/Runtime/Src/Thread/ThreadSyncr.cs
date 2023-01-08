using System;
using System.Threading;

namespace XOR
{
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
        public object PostToMainThread(string eventName, ThreadWorker.EventData data, bool throwOnError = true)
        {
            worker.VerifyThread(false, true);

            //锁定ThreadWorker同步状态
            if (!worker.AcquireSyncing())
            {
                if (throwOnError)
                    throw new ThreadStateException("Thread busy!");
                UnityEngine.Debug.LogWarning("Thread busy!");
                return null;
            }
            try
            {
                //写入主线程
                locker.AcquireWriter();
                this._syncToMainThread = new SyncEventData(eventName, data);
                locker.ReleaseWriter();

                //等待主线程同步
                while (worker.IsAlive)
                {
                    locker.AcquireReader();
                    bool isCompleted = this._syncToMainThread == null || this._syncToMainThread.completed;
                    locker.ReleaseReader();

                    if (isCompleted) break;
                    Thread.Sleep(THREAD_SLEEP);
                }
                //检查错误
                Exception exception = this._syncToMainThread?.exception;
                if (throwOnError && exception != null)
                {
                    throw exception;
                }

                object result = this._syncToMainThread?.data;
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
        public object PostToChildThread(string eventName, ThreadWorker.EventData data, bool throwOnError = true)
        {
            worker.VerifyThread(true, true);

            //锁定ThreadWorker同步状态
            if (!worker.AcquireSyncing())
            {
                if (throwOnError)
                    throw new ThreadStateException("Thread busy!");
                UnityEngine.Debug.LogWarning("Thread busy!");
                return null;
            }
            try
            {
                //写入子线程
                locker.AcquireWriter();
                this._syncToChildThread = new SyncEventData(eventName, data);
                locker.ReleaseWriter();

                //等待子线程同步
                while (worker.IsAlive)
                {
                    locker.AcquireReader();
                    bool isCompleted = this._syncToChildThread == null || this._syncToChildThread.completed;
                    locker.ReleaseReader();

                    if (isCompleted) break;
                    Thread.Sleep(THREAD_SLEEP);
                }
                //检查错误
                Exception exception = this._syncToChildThread?.exception;
                if (throwOnError && exception != null)
                {
                    throw exception;
                }

                object result = this._syncToChildThread?.data;
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
            public object result;
            public Exception exception;
            public SyncEventData(string eventName, ThreadWorker.EventData data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }
    }
}