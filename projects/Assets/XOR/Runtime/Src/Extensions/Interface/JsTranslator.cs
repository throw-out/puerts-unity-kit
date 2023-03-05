using System;
using System.Collections.Generic;

namespace XOR
{
    public class JsTranslator
    {
        static System.Runtime.CompilerServices.ConditionalWeakTable<Puerts.JsEnv, XOR.JsTranslator> translators =
            new System.Runtime.CompilerServices.ConditionalWeakTable<Puerts.JsEnv, XOR.JsTranslator>();

        public static JsTranslator GetOrCreate(Puerts.JsEnv env)
        {
            return translators.GetValue(env, (e) => new JsTranslator());
        }

#if UNITY_EDITOR && !NET_STANDARD_2_0
        private readonly CodeEmit ce = new CodeEmit();
#endif
        private readonly Dictionary<Type, Func<Puerts.JSObject, object>> interfaceBridgeCreators =
            new Dictionary<Type, Func<Puerts.JSObject, object>>();

        public void AddInterfaceBridgeCreator(Type type, Func<Puerts.JSObject, object> creator)
        {
            //interfaceBridgeCreators.Add(type, creator);
            interfaceBridgeCreators[type] = creator;
        }

        public object CreateInterfaceBridge(Puerts.JSObject target, Type interfaceType)
        {
            Func<Puerts.JSObject, object> creator;

            if (!interfaceBridgeCreators.TryGetValue(interfaceType, out creator))
            {
#if UNITY_EDITOR && !NET_STANDARD_2_0
                var bridgeType = ce.EmitInterfaceImpl(interfaceType);
                creator = (_t) =>
                {
                    return Activator.CreateInstance(bridgeType, new object[] { _t }) as JsBase;
                };
                interfaceBridgeCreators.Add(interfaceType, creator);
#else
                throw new InvalidCastException($"This type must add to [Configure] and generate bridge code: {interfaceType}\nYou can use .NET_4.x without generating bridge code.");
#endif
            }

            return creator(target);
        }
    }
}