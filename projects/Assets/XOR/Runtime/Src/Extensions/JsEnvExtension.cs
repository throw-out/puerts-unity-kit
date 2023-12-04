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
            Action quit = null;
#if UNITY_EDITOR || !UNITY_WEBGL
            ILoader loader = Helper.GetLoader(env, false);
            if (loader == null || !loader.FileExists(XORListener))
            {
                Logger.LogWarning($"Module missing: {XORListener}");
                return;
            }
            quit = Helper.IsESM(loader, XORListener) ?
                env.ExecuteModule<Action>(XORListener, "quit") :
                env.Eval<Action>($"require('{XORListener}').quit;");
#else
            quit = env.ExecuteModule<Action>(XORListener, "quit");
#endif
            if (quit != null)
            {
                quit();
            }
            else
            {
                Logger.LogWarning($"Module function missing: {XORListener}");
            }
        }

        public static void AddInterfaceBridgeCreator(this JsEnv env, Type type, Func<Puerts.JSObject, object> creator)
        {
            JsTranslator
                .GetOrCreate(env)
                .AddInterfaceBridgeCreator(type, creator);
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
                Logger.LogWarning($"AutoUsing code not generate.");
            }
        }
        public static void TryAutoInterfaceBridge(this JsEnv env, bool printWarning = true)
        {
            const string typeName = "PuertsStaticWrap.AutoStaticCodeInterfaceBridge";
            Type type = (from _assembly in AppDomain.CurrentDomain.GetAssemblies()
                         let _type = _assembly.GetType(typeName, false)
                         where _type != null
                         select _type).FirstOrDefault();
            if (type != null)
            {
                type.GetMethod("Register").Invoke(null, new object[] { env });
            }
            else if (printWarning)
            {
                Logger.LogWarning($"AutoInterfaceBridge code not generate.");
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

        static readonly string XORWorker = "puerts/xor/worker.mjs";
        static readonly string XORListener = "puerts/xor/listener.mjs";
        static readonly string XORComponent = "puerts/xor/components/component.mjs";
        static readonly string[] XORModules = new string[] {
            XORWorker,
            XORListener,
            "puerts/xor/components/behaviour.mjs",
            XORComponent,
        };
        /// <summary>
        /// 初始化XOR依赖模块
        /// </summary>
        public static void RequireXORModules(this JsEnv env) => RequireXORModules(env, true);
        public static void RequireXORModules(this JsEnv env, bool throwOnFailure)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
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
#else
            foreach (string module in XORModules)
            {
                env.ExecuteModule(module);
            }
#endif
        }
        /// <summary>
        /// 获取TsComponent.JSObject创建方法
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static Func<TsComponent, string, Puerts.JSObject> ComponentJSObjectCreator(this JsEnv env)
        {
            Func<TsComponent, string, Puerts.JSObject> create = null;
#if UNITY_EDITOR || !UNITY_WEBGL
            ILoader loader = Helper.GetLoader(env, false);
            if (loader == null || !loader.FileExists(XORComponent))
            {
                Logger.LogWarning($"Module missing: {XORComponent}");
                return null;
            }
            create = Helper.IsESM(loader, XORComponent) ?
                env.ExecuteModule<Func<TsComponent, string, Puerts.JSObject>>(XORComponent, "create") :
                env.Eval<Func<TsComponent, string, Puerts.JSObject>>($"require('{XORComponent}').create;");
#else
            create = env.ExecuteModule<Func<TsComponent, string, Puerts.JSObject>>(XORComponent, "create");
#endif
            return create;
        }
        /// <summary>
        /// 获取Puerts.JSObject对象调用方法(值将进行装箱操作)
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static Action<Puerts.JSObject, string, object[]> ComponentInvokeMethod(this JsEnv env)
        {
            Action<Puerts.JSObject, string, object[]> invoke = null;
#if UNITY_EDITOR || !UNITY_WEBGL
            ILoader loader = Helper.GetLoader(env, false);
            if (loader == null || !loader.FileExists(XORComponent))
            {
                Logger.LogWarning($"Module missing: {XORComponent}");
                return null;
            }
            invoke = Helper.IsESM(loader, XORComponent) ?
                env.ExecuteModule<Action<Puerts.JSObject, string, object[]>>(XORComponent, "invokeMethod") :
                env.Eval<Action<Puerts.JSObject, string, object[]>>($"require('{XORComponent}').invokeMethod;");
#else
            invoke = env.ExecuteModule<Action<Puerts.JSObject, string, object[]>>(XORComponent, "invokeMethod");
#endif
            return invoke;
        }
        /// <summary>
        /// 绑定ThreadWorker实例(仅ThreadWorker子线程可调用)
        /// </summary>
        /// <param name="env"></param>
        internal static void BindXORThreadWorker(this JsEnv env, ThreadWorker worker)
        {
            Action<ThreadWorker> bind = null;
#if UNITY_EDITOR || !UNITY_WEBGL
            ILoader loader = Helper.GetLoader(env, false);
            if (loader == null || !loader.FileExists(XORWorker))
            {
                Logger.LogWarning($"Module missing: {XORWorker}");
                return;
            }
            bind = Helper.IsESM(loader, XORWorker) ?
                env.ExecuteModule<Action<ThreadWorker>>(XORWorker, "bind") :
                env.Eval<Action<ThreadWorker>>($"require('{XORWorker}').bind;");
#else
            bind = env.ExecuteModule<Action<ThreadWorker>>(XORWorker, "bind");
#endif
            if (bind != null)
            {
                bind(worker);
            }
            else
            {
                Logger.LogWarning($"Module function missing: {XORWorker}");
            }
        }
        /// <summary>
        /// 加载模块(通过IsESM判定使用ExecuteModule或者Eval)
        /// </summary>
        /// <param name="env"></param>
        /// <param name="filepath"></param>
        public static void Load(this JsEnv env, string filepath)
        {
            if (filepath != null && (filepath.StartsWith("./") || filepath.StartsWith(".\\")))
            {
                filepath = filepath.Substring(2);
            }
#if UNITY_EDITOR || !UNITY_WEBGL
            ILoader loader = Helper.GetLoader(env, false);
            if (loader == null || !loader.FileExists(filepath))
            {
                Logger.LogError($"Module missing: {filepath}");
                return;
            }
            if (Helper.IsESM(loader, filepath))
            {
                env.ExecuteModule(filepath);
            }
            else
            {
                env.Eval($"require('{filepath}');");
            }
#else
           env.ExecuteModule(filepath);
#endif
        }
        /// <summary>
        /// 加载模块并获取export(通过IsESM判定使用ExecuteModule或者Eval)
        /// </summary>
        public static TResult Load<TResult>(this JsEnv env, string filepath, string exportee = "")
        {
            if (filepath != null && (filepath.StartsWith("./") || filepath.StartsWith(".\\")))
            {
                filepath = filepath.Substring(2);
            }
#if UNITY_EDITOR || !UNITY_WEBGL
            ILoader loader = Helper.GetLoader(env, false);
            if (loader == null || !loader.FileExists(filepath))
            {
                Logger.LogError($"Module missing: {filepath}");
                return default;
            }
            if (Helper.IsESM(loader, filepath))
            {
                return env.ExecuteModule<TResult>(filepath, exportee);
            }
            else
            {
                return string.IsNullOrEmpty(exportee) ?
                    env.Eval<TResult>($"require('{filepath}');") :
                    env.Eval<TResult>($"require('{filepath}').{exportee};");
            }
#else
            return env.ExecuteModule<TResult>(filepath, exportee);
#endif
        }


        static readonly string XORUtils = "puerts/xor/utils.mjs";
        /// <summary>
        /// 创建工具类方法
        /// </summary>
        /// <param name="env"></param>
        /// <param name="methodName"></param>
        /// <typeparam name="TDelegate"></typeparam>
        /// <returns></returns>
        internal static TDelegate XORUtilMethod<TDelegate>(this JsEnv env, string methodName)
            where TDelegate : Delegate
        {

            TDelegate func = null;
#if UNITY_EDITOR || !UNITY_WEBGL
            ILoader loader = Helper.GetLoader(env, false);
            if (loader == null || !loader.FileExists(XORUtils))
            {
                Logger.LogWarning($"Module missing: {XORUtils}");
                return null;
            }
            func = Helper.IsESM(loader, XORUtils) ?
                env.ExecuteModule<TDelegate>(XORUtils, methodName) :
                env.Eval<TDelegate>($"require('{XORUtils}').{methodName};");
#else
            func = env.ExecuteModule<TDelegate>(XORUtils, methodName);
#endif
            return func;
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