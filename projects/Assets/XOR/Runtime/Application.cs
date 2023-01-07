using System;
using System.IO;
using Puerts;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XOR
{
    public class Application : SingletonMonoBehaviour<Application>
    {
        public JsEnv JsEnv { get; private set; } = null;
        public PackageLoader Loader { get; private set; }
        public ushort DebugPort = 0;

        #region  Editor Debugger
#if UNITY_EDITOR
        private static bool IsWaitDebugger
        {
            get { return EditorPrefs.GetBool("Editor.DebugEnable"); }
            set { EditorPrefs.SetBool("Editor.DebugEnable", value); }
        }
        [MenuItem("PuerTS/Enable WaitDebugger")]
        private static void EnableWait() { IsWaitDebugger = true; }
        [MenuItem("PuerTS/Enable WaitDebugger", true)]
        private static bool EnableWaitCheck() { return !IsWaitDebugger; }
        [MenuItem("PuerTS/Disable WaitDebugger")]
        private static void DisableWait() { IsWaitDebugger = false; }
        [MenuItem("PuerTS/Disable WaitDebugger", true)]
        private static bool DisableWaitCheck() { return IsWaitDebugger; }
#endif
        #endregion

        void Awake()
        {
            if (__instance != null && __instance != this)
            {
                DestroyImmediate(this);
                Debug.LogWarning($"Repeated instantiation of {nameof(Application)}");
                return;
            }
            __instance = this;

            Loader = new PackageLoader();
            Loader.AddLoader(new DefaultLoader(), int.MaxValue);
            JsEnv = new JsEnv(Loader, DebugPort);
#if UNITY_EDITOR
            string projectRoot = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "TsProject");
            string outputRoot = Path.Combine(projectRoot, "dist");
            Loader.AddLoader(new FileLoader(outputRoot, projectRoot));
            if (IsWaitDebugger && DebugPort > 0)
            {
                JsEnv.WaitDebugger();
            }
#endif
            AutoUsing(JsEnv);
            SupportCommonJS(JsEnv);
        }
        void Update()
        {
            JsEnv?.Tick();
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
            if (JsEnv != null)
            {
                JsEnv.GlobalListenerQuit();
                JsEnv.Tick();
                //GC
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                //Dispose
                JsEnv.Dispose();
                JsEnv = null;
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
        /// <summary>
        /// puerts 1.4.0+ (apiLevel: 18+)
        /// default use nodejs plugins, default unsupport common js
        /// </summary>
        static void SupportCommonJS(JsEnv env)
        {
            try
            {
                if (PuertsDLL.GetApiLevel() >= 18)
                {
                    env.Eval(@"(function(){
var _g = global || globalThis || this;
_g.nodeRequire = _g.nodeRequire || _g.require;
})();
");
                    Puerts.ThirdParty.CommonJS.InjectSupportForCJS(env);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
        static void AutoUsing(JsEnv env)
        {
            const string typeName = "PuertsStaticWrap.AutoStaticCodeUsing";
            var type = (from _assembly in AppDomain.CurrentDomain.GetAssemblies()
                        let _type = _assembly.GetType(typeName, false)
                        where _type != null
                        select _type).FirstOrDefault();
            if (type != null)
            {
                type.GetMethod("AutoUsing").Invoke(null, new object[] { env });
            }
        }
    }
}