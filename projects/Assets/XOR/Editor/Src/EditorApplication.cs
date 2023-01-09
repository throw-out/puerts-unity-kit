using System;
using System.IO;
using Puerts;

namespace XOR
{
    internal class EditorApplication : Singleton<EditorApplication>, IDisposable
    {
        internal JsEnv Env { get; private set; }
        internal MergeLoader Loader { get; private set; }

        ~EditorApplication()
        {
            this.Release();
        }

        public override void Init()
        {
            base.Init();

            Loader = new MergeLoader();
            Loader.AddLoader(new DefaultLoader(), int.MaxValue);

            string projectRoot = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "TsEditorProject");
            string outputRoot = Path.Combine(projectRoot, "output");
            Loader.AddLoader(new FileLoader(outputRoot, projectRoot));

            Env = new JsEnv(Loader);
            Env.TryAutoUsing();
            Env.SupportCommonJS();
            Env.RequireXORModules(false);

            this.RegisterHandlers();
        }

        public override void Release()
        {
            base.Release();
            this.Dispose();
        }
        public void Tick()
        {
            Env?.Tick();
        }

        public void Dispose()
        {
            this.UnregisterHandlers();
            if (Env != null)
            {
                Env.GlobalListenerQuit();
                Env.Tick();
                Env.Dispose();
                Env = null;
            }
            if (Loader != null)
            {
                Loader.Dispose();
                Loader = null;
            }
        }

        void RegisterHandlers()
        {
            EditorApplicationHandler.Update += Tick;
            EditorApplicationHandler.Dispose += Dispose;
        }
        void UnregisterHandlers()
        {
            EditorApplicationHandler.Update -= Tick;
            EditorApplicationHandler.Dispose -= Dispose;
        }

    }

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
                UnityEngine.Debug.Log($"XOR.{nameof(EditorApplicationHandler)} Registered: <b><color=red>Failure</color></b>.");
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

        public static event Action Update;
        public static event Action DelayCall;
        public static event Action Dispose;

        static void UpdateHandler()
        {
            Update?.Invoke();
        }
        static void DelayCallHandler()
        {
            DelayCall?.Invoke();
        }
        static void DisposeHandler(object sender, EventArgs e)
        {
            Dispose?.Invoke();
        }
    }
}