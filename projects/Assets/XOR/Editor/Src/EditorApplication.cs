using System;
using System.IO;
using Puerts;
using XOR.Services;

namespace XOR
{
    internal class EditorApplication : Singleton<EditorApplication>, IDisposable
    {
        internal JsEnv Env { get; private set; }
        internal MixerLoader Loader { get; private set; }
        internal ThreadWorker Worker { get; private set; }
        internal Program Program { get; private set; }
        internal TSInterfaces Interfaces { get; private set; }

        ~EditorApplication()
        {
            this.Release();
        }

        public override void Init()
        {
            base.Init();

            Loader = new MixerLoader();
            Loader.AddLoader(new DefaultLoader(), int.MaxValue, filepath => !string.IsNullOrEmpty(filepath) && (
                filepath.StartsWith("puerts/") ||
                filepath.StartsWith("puer-commonjs/")
            ));

            Env = new JsEnv(Loader);
            Env.TryAutoUsing();
            Env.RequireXORModules();
            Env.SupportCommonJS();

            this.RegisterHandlers();
        }

        public override void Release()
        {
            base.Release();
            this.Dispose();

            if (Worker != null)
            {
                Worker.Dispose();
                Worker = null;
            }
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
            GC.SuppressFinalize(this);
        }
        public void SetWorker(ThreadWorker worker)
        {
            this.Worker = worker;
        }
        public void SetProgram(Program program)
        {
            this.Program = program;
        }
        public void SetInterfaces(TSInterfaces interfaces)
        {
            this.Interfaces = interfaces;
        }
        public bool IsInitializing()
        {
            return Worker != null && Worker.IsAlive && !Worker.IsInitialized;
        }
        public bool IsWorkerRunning()
        {
            return Worker != null && Worker.IsAlive;
        }

        void RegisterHandlers()
        {
            EditorApplicationHandler.update += Tick;
            EditorApplicationHandler.dispose += Dispose;
        }
        void UnregisterHandlers()
        {
            EditorApplicationHandler.update -= Tick;
            EditorApplicationHandler.dispose -= Dispose;
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