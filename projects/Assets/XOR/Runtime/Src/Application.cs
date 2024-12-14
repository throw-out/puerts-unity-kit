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
        const string kMenu = "Tools/PuerTS/WaitDebugger";

        [UnityEditor.MenuItem(kMenu)]
        private static void WaitDebugger() { IsWaitDebugger = !IsWaitDebugger; }
        [UnityEditor.MenuItem(kMenu, true)]
        private static bool WaitDebuggerValidate()
        {
            UnityEditor.Menu.SetChecked(kMenu, IsWaitDebugger);
            return true;
        }
#endif
        #endregion

        /// <summary>
        /// 加载模块(通过IsESM判定使用ExecuteModule或者Eval)
        /// </summary>
        public void Load(string filepath)
        {
            this.Env.Load(filepath);
        }
        /// <summary>
        /// 加载模块并获取export(通过IsESM判定使用ExecuteModule或者Eval)
        /// </summary>
        public TResult Load<TResult>(string filepath, string exportee = "")
        {
            return this.Env.Load<TResult>(filepath, exportee);
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
            Loader.AddLoader(new DefaultLoader(), int.MaxValue, filepath => !string.IsNullOrEmpty(filepath) && (
                filepath.StartsWith("puerts/") ||
                filepath.StartsWith("puer-commonjs/")
            ));
#if UNITY_EDITOR || !UNITY_WEBGL
            Env = new JsEnv(Loader, debugPort);
#else
            //Env = Puerts.WebGL.GetBrowserEnv(Loader);   //适用于1.4.x及以下版本, 必须包含在UNITY_WEBGL宏编译条件下
            Env = Puerts.WebGL.MainEnv.Get(Loader);
#endif
            Env.TryAutoUsing();
            Env.TryAutoInterfaceBridge();
            Env.RequireXORModules();
#if UNITY_EDITOR || !UNITY_WEBGL
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