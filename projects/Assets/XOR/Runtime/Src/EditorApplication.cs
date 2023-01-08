using System;

namespace XOR
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class EditorApplication
    {
#if UNITY_EDITOR
        static EditorApplication()
        {
            UnityEditor.EditorApplication.update += UpdateHandler;
            UnityEditor.EditorApplication.delayCall += DelayCallHandler;
            RegisterAppDomainUnload();
        }
        [UnityEditor.Callbacks.DidReloadScripts]
        static void DidReloadScripts()
        {
            //UnityEngine.Debug.Log("DidReloadScripts");
        }
        [UnityEditor.Callbacks.PostProcessBuild]
        static void PostProcessBuild()
        {
            //UnityEngine.Debug.Log("PostProcessBuild");
        }
        static void RegisterAppDomainUnload()
        {
            if (AppDomain.CurrentDomain != null)
            {
                AppDomain.CurrentDomain.DomainUnload += (e, argv) => StopHandler();
                AppDomain.CurrentDomain.ProcessExit += (e, argv) => StopHandler();

                UnityEngine.Debug.Log("XOR.AppDomainUnload Registered: <b><color=green>Successed</color></b>.");
            }
            else
            {
                UnityEngine.Debug.Log("XOR.AppDomainUnload Registered: <b><color=red>Failure</color></b>.");
            }
        }
#endif

        public static event Action Stop;
        public static event Action Update;
        public static event Action DelayCall;
 
        static void StopHandler()
        {
            Stop?.Invoke();
        }
        static void UpdateHandler()
        {
            Update?.Invoke();
        }
        static void DelayCallHandler()
        {
            DelayCall?.Invoke();
        }
    }
}