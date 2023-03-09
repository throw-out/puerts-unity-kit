using System;

namespace HR
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    internal static class EditorApplicationHandler
    {
#if UNITY_EDITOR
        static EditorApplicationHandler()
        {
            RegisterHandlers();
        }
        static void RegisterHandlers()
        {
            UnityEditor.EditorApplication.update += UpdateHandler;
            UnityEditor.EditorApplication.delayCall += DelayCallHandler;
            if (AppDomain.CurrentDomain != null)
            {
                AppDomain.CurrentDomain.DomainUnload += DisposeHandler;
                AppDomain.CurrentDomain.ProcessExit += DisposeHandler;
            }
            else
            {
                UnityEngine.Debug.LogError($"XOR.{nameof(EditorApplicationHandler)} Registered: <b><color=red>Failure</color></b>.");
            }
        }
        static void UnregisterHandlers()
        {
            UnityEditor.EditorApplication.update -= UpdateHandler;
            UnityEditor.EditorApplication.delayCall -= DelayCallHandler;
            if (AppDomain.CurrentDomain != null)
            {
                AppDomain.CurrentDomain.DomainUnload -= DisposeHandler;
                AppDomain.CurrentDomain.ProcessExit -= DisposeHandler;
            }
        }
#endif

        public static event Action update;
        public static event Action delayCall;
        public static event Action dispose;

        static void UpdateHandler()
        {
            update?.Invoke();
        }
        static void DelayCallHandler()
        {
            delayCall?.Invoke();
        }
        static void DisposeHandler(object sender, EventArgs e)
        {
            dispose?.Invoke();
        }
    }
}