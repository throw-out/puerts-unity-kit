using System;
using System.IO;
using Puerts;
using UnityEngine;
using System.Linq;

namespace XOR
{
    public class Application : SingletonMonoBehaviour<Application>
    {
        public JsEnv Env { get; private set; } = null;
        public MixerLoader Loader { get; private set; }
        public ushort debugPort = 9090;

        #region  Editor Debugger
#if UNITY_EDITOR
        private static bool IsWaitDebugger
        {
            get { return UnityEditor.EditorPrefs.GetBool("Editor.DebugEnable"); }
            set { UnityEditor.EditorPrefs.SetBool("Editor.DebugEnable", value); }
        }
        [UnityEditor.MenuItem("PuerTS/Enable WaitDebugger")]
        private static void Enable() { IsWaitDebugger = true; }
        [UnityEditor.MenuItem("PuerTS/Enable WaitDebugger", true)]
        private static bool EnableValidate() { return !IsWaitDebugger; }
        [UnityEditor.MenuItem("PuerTS/Disable WaitDebugger")]
        private static void Disable() { IsWaitDebugger = false; }
        [UnityEditor.MenuItem("PuerTS/Disable WaitDebugger", true)]
        private static bool DisableValidate() { return IsWaitDebugger; }
#endif
        #endregion

        public void Load(string filepath)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            this.Env.ExecuteModule(filepath);
#else
            if (this.Loader.IsESM(filepath))
            {
                this.Env.ExecuteModule(filepath);
            }
            else
            {
                this.Env.Eval($"require('{filepath}')");
            }
#endif
        }
        public TResult Load<TResult>(string filepath, string exportee = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return this.Env.ExecuteModule<TResult>(filepath, exportee);
#else
            if (this.Loader.IsESM(filepath))
            {
                return this.Env.ExecuteModule<TResult>(filepath, exportee);
            }
            else
            {
                return string.IsNullOrEmpty(exportee) ?
                    this.Env.Eval<TResult>($"require('{filepath}');") :
                    this.Env.Eval<TResult>($"require('{filepath}').{exportee};");
            }
#endif
        }

        void Awake()
        {
            if (__instance != null && __instance != this)
            {
                DestroyImmediate(this);
                Debug.LogWarning($"Multiple instantiation of {nameof(Application)}");
                return;
            }
            __instance = this;

            Loader = new MixerLoader();
            Loader.AddLoader(new DefaultLoader(), int.MaxValue);

#if UNITY_EDITOR || !UNITY_WEBGL
            Env = new JsEnv(Loader, debugPort);
#else
            Env = Puerts.WebGL.GetBrowserEnv(Loader);
#endif
            Env.TryAutoUsing();
            Env.RequireXORModules();
#if !UNITY_EDITOR && UNITY_WEBGL
            Env.SupportCommonJS();
#endif
#if UNITY_EDITOR
            if (IsWaitDebugger && debugPort > 0)
            {
                Env.WaitDebugger();
            }
#endif
        }
        void Update()
        {
            Env?.Tick();
        }
        void OnDestroy()
        {
            Dispose();
        }
        public override void Release()
        {
            base.Release();
            Dispose();
        }
        void Dispose()
        {
            if (Env != null)
            {
                Env.GlobalListenerQuit();
                Env.Tick();
                //GC
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                //Dispose
                Env.Dispose();
                Env = null;
            }
            if (Loader != null)
            {
                Loader.Dispose();
                Loader = null;
            }
            //GC
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
    }
}