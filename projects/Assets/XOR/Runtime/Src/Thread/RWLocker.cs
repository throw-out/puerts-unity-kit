using System;
using System.Threading;

namespace XOR
{
    public class RWLocker
    {
        private readonly int timeout;
        private readonly ReaderWriterLock locker = new ReaderWriterLock();
        public bool IsReaderLockHeld { get => locker.IsReaderLockHeld; }
        public bool IsWriterLockHeld { get => locker.IsWriterLockHeld; }

        public RWLocker(int millisecondsTimeout)
        {
            this.timeout = millisecondsTimeout;
        }
        public bool AcquireWriter(bool throwOnFailure = true)
        {
            try
            {
                //Interlocked.Increment()
                locker.AcquireWriterLock(timeout);
                return true;
            }
            catch (Exception e)
            {
                if (throwOnFailure) throw e;
            }
            return false;
        }
        public bool AcquireReader(bool throwOnFailure = true)
        {
            try
            {
                locker.AcquireReaderLock(timeout);
                return true;
            }
            catch (Exception e)
            {
                if (throwOnFailure) throw e;
            }
            return false;
        }
        public void ReleaseWriter()
        {
            locker.ReleaseWriterLock();
        }
        public void ReleaseReader()
        {
            locker.ReleaseReaderLock();
        }
    }
}