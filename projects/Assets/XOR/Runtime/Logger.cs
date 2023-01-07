using UnityEngine;
using LogCallback = UnityEngine.Application.LogCallback;

namespace XOR
{
    public class Logger : Singleton<Logger>
    {
        public event LogCallback logMessageReceived;
        public event LogCallback logMessageReceivedThreaded;

        private void LogReceived(string condition, string stackTrace, LogType type)
        {
            logMessageReceived?.Invoke(condition, stackTrace, type);
        }
        private void LogReceivedThreaded(string condition, string stackTrace, LogType type)
        {
            logMessageReceivedThreaded?.Invoke(condition, stackTrace, type);
        }
        public override void Init()
        {
            base.Init();
            UnityEngine.Application.logMessageReceived += LogReceived;
            UnityEngine.Application.logMessageReceivedThreaded += LogReceivedThreaded;
        }
        public override void Release()
        {
            base.Release();
            UnityEngine.Application.logMessageReceived -= LogReceived;
            UnityEngine.Application.logMessageReceivedThreaded -= LogReceivedThreaded;
            this.logMessageReceived = null;
            this.logMessageReceivedThreaded = null;
        }
    }
}
