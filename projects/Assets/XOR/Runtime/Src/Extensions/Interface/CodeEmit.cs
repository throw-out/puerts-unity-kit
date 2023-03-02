using System;
#if UNITY_EDITOR && !NET_STANDARD_2_0
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
#endif

namespace XOR
{
    internal static class CodeEmit
    {
        private static int genID = 1;
        public static Type EmitInterfaceImpl(Type toBeImpl)
        {
            if (!toBeImpl.IsInterface)
            {
                throw new InvalidOperationException("interface expected, but got " + toBeImpl);
            }
#if UNITY_EDITOR && !NET_STANDARD_2_0
            TypeBuilder typeBuilder = CodeEmitModule.DefineType("PuertsGenInterfaceImpl" + (genID++), TypeAttributes.Public | TypeAttributes.Class, typeof(XOR.JsBase), new Type[] { toBeImpl });

            foreach (MemberInfo member in (new Type[] { toBeImpl }.Concat(toBeImpl.GetInterfaces()).SelectMany(i => i.GetMembers()).Distinct()))
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Method:
                        {
                            MethodInfo method = member as MethodInfo;
                            if (method.IsSpecialName && (
                                method.Name.StartsWith("get_") || method.Name.StartsWith("set_") ||
                                method.Name.StartsWith("add_") || method.Name.StartsWith("remove_")
                            ))
                            {
                                continue;
                            }
                            MethodBuilder methodBuilder = DefineMethodImpl(typeBuilder, method, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual);
                            EmitMethodImpl(methodBuilder.GetILGenerator(), method, true);
                        }
                        break;
                    case MemberTypes.Property:
                        {
                            PropertyInfo property = member as PropertyInfo;
                            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, Type.EmptyTypes);
                            if (property.Name == "Item")
                            {
                                if (property.CanRead)
                                {
                                    MethodBuilder getterBuilder = DefineMethodImpl(typeBuilder, property.GetGetMethod(), MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig);
                                    EmitMethodImpl(getterBuilder.GetILGenerator(), property.GetGetMethod(), true);
                                    propertyBuilder.SetGetMethod(getterBuilder);
                                }
                                if (property.CanWrite)
                                {
                                    MethodBuilder setterBuildler = DefineMethodImpl(typeBuilder, property.GetSetMethod(), MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig);
                                    EmitMethodImpl(setterBuildler.GetILGenerator(), property.GetSetMethod(), true);
                                    propertyBuilder.SetSetMethod(setterBuildler);
                                }
                                continue;
                            }
                            if (property.CanRead)
                            {
                                MethodBuilder getterBuilder = typeBuilder.DefineMethod("get_" + property.Name,
                                    MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                    property.PropertyType,
                                    Type.EmptyTypes
                                );
                                //TODO: emit impl
                                ILGenerator il = getterBuilder.GetILGenerator();

                                propertyBuilder.SetGetMethod(getterBuilder);
                            }
                            if (property.CanWrite)
                            {
                                MethodBuilder setterBuilder = typeBuilder.DefineMethod("set_" + property.Name,
                                    MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                    null,
                                    new Type[] { property.PropertyType }
                                );
                                //TODO: emit impl
                                propertyBuilder.SetSetMethod(setterBuilder);
                            }
                        }
                        break;
                    case MemberTypes.Event:
                        {
                            EventInfo eventInfo = member as EventInfo;
                            EventBuilder eventBuilder = typeBuilder.DefineEvent(eventInfo.Name, eventInfo.Attributes, eventInfo.EventHandlerType);
                            if (eventInfo.GetAddMethod() != null)
                            {
                                var addEventBuildler = DefineMethodImpl(typeBuilder, eventInfo.GetAddMethod(), MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig);
                                EmitMethodImpl(addEventBuildler.GetILGenerator(), eventInfo.GetAddMethod(), true);
                                eventBuilder.SetAddOnMethod(addEventBuildler);
                            }
                            if (eventInfo.GetRemoveMethod() != null)
                            {
                                var removeEventBuildler = DefineMethodImpl(typeBuilder, eventInfo.GetRemoveMethod(), MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig);
                                EmitMethodImpl(removeEventBuildler.GetILGenerator(), eventInfo.GetRemoveMethod(), true);
                                eventBuilder.SetRemoveOnMethod(removeEventBuildler);
                            }
                        }
                        break;
                }
            }

            // Constructor
            Type[] ctorParametersTypes = new Type[] { typeof(Puerts.JSObject) };
            ConstructorInfo parentCtor = typeof(XOR.JsBase).GetConstructor(ctorParametersTypes);

            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorParametersTypes);
            ILGenerator ctorIL = ctorBuilder.GetILGenerator();
            //TODO: ctor impl

            return typeBuilder.CreateType();
#else
            throw new NotSupportedException();
#endif
        }

#if UNITY_EDITOR && !NET_STANDARD_2_0
        static MethodBuilder DefineMethodImpl(TypeBuilder typeBuilder, MethodInfo method, MethodAttributes attributes, string methodName = null)
        {
            ParameterInfo[] parameters = method.GetParameters();

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                methodName != null ? methodName : method.Name,
                attributes,
                method.ReturnType,
                parameters.Select(o => o.ParameterType).ToArray()
            );
            for (int i = 0; i < parameters.Length; ++i)
            {
                methodBuilder.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
            }

            return methodBuilder;
        }
        static void EmitMethodImpl(ILGenerator il, MethodInfo method, bool isObject)
        {
            ParameterInfo[] parameters = method.GetParameters();
        }

        static ModuleBuilder _codeEmitModule;
        static ModuleBuilder CodeEmitModule
        {
            get
            {
                if (_codeEmitModule == null)
                {
                    var assemblyName = new AssemblyName();
                    assemblyName.Name = "PuertsCodeEmit";
                    _codeEmitModule = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                        .DefineDynamicModule("PuertsCodeEmit");
                }
                return _codeEmitModule;
            }
        }
#endif
    }
}