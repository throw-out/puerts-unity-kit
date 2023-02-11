using System;
using System.Linq;
using System.Reflection;
using Puerts;
using UnityEngine;

namespace XOR
{
    public static class JsEnvExtension
    {
        public static void GlobalListenerQuit(this JsEnv env)
        {
            ILoader loader = Helper.GetLoader(env, false);
            if (loader == null || !loader.FileExists(XORListener))
            {
                Logger.LogWarning($"Module missing: {XORListener}");
                return;
            }
            Action quit = Helper.IsESM(loader, XORListener) ?
                env.ExecuteModule<Action>(XORListener, "quit") :
                env.Eval<Action>($"var m = require('{XORListener}'); m.quit;");
            if (quit != null)
            {
                quit();
            }
            else
            {
                Logger.LogWarning($"Module function missing: {XORListener}");
            }
        }

        public static void TryAutoUsing(this JsEnv env, bool printWarning = true)
        {
            const string typeName = "PuertsStaticWrap.AutoStaticCodeUsing";
            Type type = (from _assembly in AppDomain.CurrentDomain.GetAssemblies()
                         let _type = _assembly.GetType(typeName, false)
                         where _type != null
                         select _type).FirstOrDefault();
            if (type != null)
            {
                type.GetMethod("AutoUsing").Invoke(null, new object[] { env });
            }
            else if (printWarning)
            {
                Logger.LogWarning($"AutoUsingCode not generate.");
            }
        }
        public static void SupportCommonJS(this JsEnv env)
        {
            try
            {
                if (PuertsDLL.GetApiLevel() >= 18)
                {
                    env.Eval(@"
(function(){
    let _g = (global || globalThis || this);
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

        static readonly string XORWorker = "puerts/xor/worker";
        static readonly string XORListener = "puerts/xor/listener";
        static readonly string[] XORModules = new string[] {
            XORWorker,
            XORListener,
            "puerts/xor/components/behaviour",
            "puerts/xor/components/component",
        };
        /// <summary>
        /// 初始化XOR依赖模块
        /// </summary>
        public static void RequireXORModules(this JsEnv env) => RequireXORModules(env, true);
        public static void RequireXORModules(this JsEnv env, bool throwOnFailure)
        {
            ILoader loader = Helper.GetLoader(env, throwOnFailure);
            if (loader == null)
            {
                return;
            }
            foreach (string module in XORModules)
            {
                if (!loader.FileExists(module))
                {
                    if (throwOnFailure)
                        throw Helper.ModuleMissingException(module);
                    Debug.LogWarning(Helper.ModuleMissingException(module).Message);
                    continue;
                }
                if (Helper.IsESM(loader, module))
                {
                    env.ExecuteModule(module);
                }
                else
                {
                    env.Eval($"require('{module}')");
                }
            }
        }
        /// <summary>
        /// 绑定ThreadWorker实例(仅ThreadWorker子线程可调用)
        /// </summary>
        /// <param name="env"></param>
        internal static void BindXORThreadWorker(this JsEnv env, ThreadWorker worker)
        {
            ILoader loader = Helper.GetLoader(env, false);
            if (loader == null || !loader.FileExists(XORWorker))
            {
                Logger.LogWarning($"Module missing: {XORWorker}");
                return;
            }
            Action<ThreadWorker> bind = Helper.IsESM(loader, XORWorker) ?
                env.ExecuteModule<Action<ThreadWorker>>(XORWorker, "bind") :
                env.Eval<Action<ThreadWorker>>($"var m = require('{XORWorker}'); m.bind;");
            if (bind != null)
            {
                bind(worker);
            }
            else
            {
                Logger.LogWarning($"Module function missing: {XORWorker}");
            }
        }

        static class Helper
        {

            public static ILoader GetLoader(JsEnv env, bool throwOnFailure)
            {
                if (Helper.Loader == null)
                {
                    if (throwOnFailure)
                        throw Helper.NullReferenceException();
                    Debug.LogWarning(Helper.NullReferenceException().Message);
                    return null;
                }
                ILoader loader = Helper.Loader(env);
                if (loader == null)
                {
                    if (throwOnFailure)
                        throw Helper.NullReferenceException();
                    Debug.LogWarning(Helper.NullReferenceException().Message);
                    return null;
                }
                return loader;
            }
            public static bool IsESM(ILoader loader, string module)
            {
                return loader is IModuleChecker && ((IModuleChecker)loader).IsESM(module);
            }
            static Func<JsEnv, ILoader> _loader = null;
            static Func<JsEnv, ILoader> Loader
            {
                get
                {
                    if (_loader == null)
                    {
                        FieldInfo fieldInfo = typeof(JsEnv).GetField("loader", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (fieldInfo != null)
                        {
                            _loader = DelegateUtil.CreateFieldDelegate<Func<JsEnv, ILoader>>(fieldInfo, null, false);
                        }
                    }
                    return _loader;
                }
            }
            public static Exception NullReferenceException()
            {
                return new NullReferenceException("Object reference null");
            }
            public static Exception ModuleMissingException(string module = null)
            {
                return new NullReferenceException($"XOR module missing: {module}");
            }
        }
    }
}