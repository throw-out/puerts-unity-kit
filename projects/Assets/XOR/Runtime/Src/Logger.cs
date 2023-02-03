using System.Threading;
using UnityEngine;
using LogCallback = UnityEngine.Application.LogCallback;

namespace XOR
{
    public class Logger : Singleton<Logger>
    {
        public event LogCallback logMessageReceived;
        public event LogCallback logMessageReceivedThreaded;

        void LogReceived(string condition, string stackTrace, LogType type)
        {
            logMessageReceived?.Invoke(condition, stackTrace, type);
        }
        void LogReceivedThreaded(string condition, string stackTrace, LogType type)
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

        public static void Info(object firstMessage, params object[] messages)
        {
            if (!IsEnable(Settings.LOGGER.INFO))
                return;
            if (messages.Length > 0)
            {
                UnityEngine.Debug.Log(string.Join(", ", firstMessage, string.Join(", ", messages)));
            }
            else
            {
                UnityEngine.Debug.Log(firstMessage);
            }
        }
        public static void Log(object firstMessage, params object[] messages)
        {
            if (!IsEnable(Settings.LOGGER.LOG))
                return;
            if (messages.Length > 0)
            {
                UnityEngine.Debug.Log(string.Join(", ", firstMessage, string.Join(", ", messages)));
            }
            else
            {
                UnityEngine.Debug.Log(firstMessage);
            }
        }
        public static void LogWarning(object firstMessage, params object[] messages)
        {
            if (!IsEnable(Settings.LOGGER.WARN))
                return;
            if (messages.Length > 0)
            {
                UnityEngine.Debug.LogWarning(string.Join(", ", firstMessage, string.Join(", ", messages)));
            }
            else
            {
                UnityEngine.Debug.LogWarning(firstMessage);
            }
        }
        public static void LogError(object firstMessage, params object[] messages)
        {
            if (!IsEnable(Settings.LOGGER.ERROR))
                return;
            if (messages.Length > 0)
            {
                UnityEngine.Debug.LogError(string.Join(", ", firstMessage, string.Join(", ", messages)));
            }
            else
            {
                UnityEngine.Debug.LogError(firstMessage);
            }
        }

        static readonly int MAIN_THREAD_ID = Thread.CurrentThread.ManagedThreadId;
        public static bool IsEnable(Settings.LOGGER type)
        {
            Settings.LOGGER current = Settings.LOGGER.NONE;
            if (Settings.Instance != null)
            {
                current = Settings.Instance.logger;
            }
            else if (Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID)
            {
                current = Settings.Load(true, false).logger;
            }
            else
            {
                current = type;             //(child thread)force log
            }
            return (current & type) == type;
        }
    }
}
