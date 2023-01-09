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
            env.Eval(@"
(function(){
    let listener = (global || globalThis || this)['globalListener'];
    if( listener && listener.quit){
        listener.quit.invoke();
    }
})();");
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
                Debug.LogWarning($"AutoUsingCode not generate ");
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

        static readonly string[] XORModules = new string[] {
            "puerts/xor/globalListener",
            "puerts/xor/threadWorker",
        };
        /// <summary>
        /// 初始化XOR依赖模块
        /// </summary>
        public static void RequireXORModules(this JsEnv env) => RequireXORModules(env, false, false);
        public static void RequireXORModules(this JsEnv env, bool isESM) => RequireXORModules(env, isESM, false);
        public static void RequireXORModules(this JsEnv env, bool isESM, bool throwOnFailure)
        {
            if (Helper.GetLoader == null)
            {
                if (throwOnFailure)
                    throw Helper.NullReferenceException();
                Debug.LogWarning(Helper.NullReferenceException().Message);
                return;
            }
            ILoader loader = Helper.GetLoader(env);
            if (loader == null)
            {
                if (throwOnFailure)
                    throw Helper.NullReferenceException();
                Debug.LogWarning(Helper.NullReferenceException().Message);
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
                if (isESM)
                {
                    env.ExecuteModule(module);
                }
                else
                {
                    env.Eval($"require('./{module}')");
                }
            }
        }
        /// <summary>
        /// 绑定ThreadWorker实例(仅ThreadWorker子线程可调用)
        /// </summary>
        /// <param name="env"></param>
        internal static void BindThreadWorker(this JsEnv env, ThreadWorker worker)
        {
            string script = @"
function func(worker){ 
    let _g = (function(){ return global || globalThis || this; })();
    _g.XOR = _g.XOR ?? {};
    _g.XOR.globalWorker = new ThreadWorker(worker);
}
func
";
            env.Eval<Action<ThreadWorker>>(script)(worker);
        }

        static class Helper
        {
            static Func<JsEnv, ILoader> _loader = null;
            public static Func<JsEnv, ILoader> GetLoader
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