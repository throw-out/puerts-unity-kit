using System;
using System.Collections.Generic;

namespace XOR
{
    public static class JsTranslator
    {
        private static Dictionary<Type, Func<Puerts.JSObject, object>> interfaceBridgeCreators =
            new Dictionary<Type, Func<Puerts.JSObject, object>>();

        public static void AddInterfaceBridgeCreator(Type type, Func<Puerts.JSObject, object> creator)
        {
            //interfaceBridgeCreators.Add(type, creator);
            interfaceBridgeCreators[type] = creator;
        }

        public static object CreateInterfaceBridge(Puerts.JSObject target, Type interfaceType)
        {
            Func<Puerts.JSObject, object> creator;

            if (!interfaceBridgeCreators.TryGetValue(interfaceType, out creator))
            {
#if UNITY_EDITOR && !NET_STANDARD_2_0
                var bridgeType = CodeEmit.EmitInterfaceImpl(interfaceType);
                creator = (_t) =>
                {
                    return Activator.CreateInstance(bridgeType, new object[] { _t }) as JsBase;
                };
                interfaceBridgeCreators.Add(interfaceType, creator);
#else
                throw new InvalidCastException("This type must add to Configure: " + interfaceType);
#endif
            }

            return creator(target);
        }
    }
}