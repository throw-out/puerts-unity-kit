using System;
#if UNITY_EDITOR && !NET_STANDARD_2_0
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
#endif

namespace XOR
{
    internal class CodeEmit
    {
        private const BindingFlags Flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static Type[] JSBase_ctor_parameters = new Type[] { typeof(Puerts.JSObject) };
        private ConstructorInfo JSBase_ctor = typeof(XOR.JsBase).GetConstructor(JSBase_ctor_parameters);
        private FieldInfo JSBase_target = typeof(XOR.JsBase).GetField("target", Flags);
        private MethodInfo JSObject_Get_generic1 = typeof(XOR.JsObjectExtension).GetMethods(Flags).FirstOrDefault(m =>
            "Get".Equals(m.Name) &&
            m.IsGenericMethod &&
            m.GetGenericArguments().Length == 1
        );
        private MethodInfo JSObject_Set_generic1 = typeof(XOR.JsObjectExtension).GetMethods(Flags).FirstOrDefault(m =>
            "Set".Equals(m.Name) &&
            m.IsGenericMethod &&
            m.GetGenericArguments().Length == 1
        );
        private MethodInfo JSObject_Call_void = typeof(XOR.JsObjectExtension).GetMethods(Flags).FirstOrDefault(m =>
            "Call".Equals(m.Name) &&
            !m.IsGenericMethod
        );
        private MethodInfo JSObject_Call_generic1 = typeof(XOR.JsObjectExtension).GetMethods(Flags).FirstOrDefault(m =>
            "Call".Equals(m.Name) &&
            m.IsGenericMethod &&
            m.GetGenericArguments().Length == 1
        );


        private static ulong genID = 1;
        private static Dictionary<Type, Type> genTypes = new Dictionary<Type, Type>();
        public Type EmitInterfaceImpl(Type toBeImpl)
        {
            if (!toBeImpl.IsInterface)
            {
                throw new InvalidOperationException("interface expected, but got " + toBeImpl);
            }
#if UNITY_EDITOR && !NET_STANDARD_2_0
            Type impl;
            if (!genTypes.TryGetValue(toBeImpl, out impl))
            {
                TypeBuilder typeBuilder = CodeEmitModule.DefineType($"<{genID++}>PuertsGenInterfaceImpl", TypeAttributes.Public | TypeAttributes.Class, typeof(XOR.JsBase), new Type[] { toBeImpl });

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
                                EmitMethodImpl(methodBuilder.GetILGenerator(), method);
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
                                        MethodBuilder getterBuilder = DefineMethodImpl(
                                            typeBuilder,
                                            property.GetGetMethod(),
                                            MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig
                                        );
                                        EmitMethodImpl(getterBuilder.GetILGenerator(), property.GetGetMethod());
                                        propertyBuilder.SetGetMethod(getterBuilder);
                                    }
                                    if (property.CanWrite)
                                    {
                                        MethodBuilder setterBuildler = DefineMethodImpl(
                                            typeBuilder,
                                            property.GetSetMethod(),
                                            MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig
                                        );
                                        EmitMethodImpl(setterBuildler.GetILGenerator(), property.GetSetMethod());
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
                                    EmitGetterMethodImpl(getterBuilder.GetILGenerator(), property.PropertyType, property.Name);
                                    propertyBuilder.SetGetMethod(getterBuilder);
                                }
                                if (property.CanWrite)
                                {
                                    MethodBuilder setterBuilder = typeBuilder.DefineMethod("set_" + property.Name,
                                        MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                                        null,
                                        new Type[] { property.PropertyType }
                                    );
                                    EmitSetterMethodImpl(setterBuilder.GetILGenerator(), property.PropertyType, property.Name);
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
                                    EmitMethodImpl(addEventBuildler.GetILGenerator(), eventInfo.GetAddMethod());
                                    eventBuilder.SetAddOnMethod(addEventBuildler);
                                }
                                if (eventInfo.GetRemoveMethod() != null)
                                {
                                    var removeEventBuildler = DefineMethodImpl(typeBuilder, eventInfo.GetRemoveMethod(), MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig);
                                    EmitMethodImpl(removeEventBuildler.GetILGenerator(), eventInfo.GetRemoveMethod());
                                    eventBuilder.SetRemoveOnMethod(removeEventBuildler);
                                }
                            }
                            break;
                    }
                }

                // Constructor
                ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, JSBase_ctor_parameters);
                EmitConstructorImpl(ctorBuilder.GetILGenerator(), JSBase_ctor);

                impl = typeBuilder.CreateType();
                genTypes.Add(toBeImpl, impl);
            }
            return impl;
#else
            throw new NotSupportedException();
#endif
        }

#if UNITY_EDITOR && !NET_STANDARD_2_0
        MethodBuilder DefineMethodImpl(TypeBuilder typeBuilder, MethodInfo method, MethodAttributes attributes, string methodName = null)
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
        void EmitMethodImpl(ILGenerator il, MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            bool hasReturn = method.ReturnType != typeof(void);

            LocalBuilder target = il.DeclareLocal(typeof(Puerts.JSObject));
            LocalBuilder args = il.DeclareLocal(typeof(object[]));

            //target = JsBase.target
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, JSBase_target);
            il.Emit(OpCodes.Stloc, target);

            //args = new object[] -> params object[] args
            il.Emit(OpCodes.Ldc_I4, parameters.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc, args);
            //args add members
            for (int i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldloc, args);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarga, i + 1);     //non-static method, first parameter is [this]
                il.Emit(OpCodes.Stelem_Ref);
            }

            //Call
            il.Emit(OpCodes.Ldloc, target);
            il.Emit(OpCodes.Ldstr, method.Name);
            if (args != null) il.Emit(OpCodes.Ldloc, args);
            il.Emit(OpCodes.Call, hasReturn ? JSObject_Call_generic1.MakeGenericMethod(new Type[] { method.ReturnType }) : JSObject_Call_void);

            if (!hasReturn) il.Emit(OpCodes.Pop);
            il.Emit(OpCodes.Ret);
        }
        void EmitGetterMethodImpl(ILGenerator il, Type propertyType, string propertyName)
        {
            LocalBuilder target = il.DeclareLocal(typeof(Puerts.JSObject));

            //target = JsBase.target
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, JSBase_target);
            il.Emit(OpCodes.Stloc, target);

            //Call
            il.Emit(OpCodes.Ldloc, target);
            il.Emit(OpCodes.Ldstr, propertyName);
            il.Emit(OpCodes.Call, JSObject_Get_generic1.MakeGenericMethod(new Type[] { propertyType }));

            il.Emit(OpCodes.Ret);
        }
        void EmitSetterMethodImpl(ILGenerator il, Type propertyType, string propertyName)
        {
            LocalBuilder target = il.DeclareLocal(typeof(Puerts.JSObject));

            //target = JsBase.target
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, JSBase_target);
            il.Emit(OpCodes.Stloc, target);

            //Call
            il.Emit(OpCodes.Ldloc, target);
            il.Emit(OpCodes.Ldstr, propertyName);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, JSObject_Set_generic1.MakeGenericMethod(new Type[] { propertyType }));

            //il.Emit(OpCodes.Pop);
            //il.Emit(OpCodes.Ret);
        }
        void EmitConstructorImpl(ILGenerator il, ConstructorInfo method)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, method);

            il.Emit(OpCodes.Ret);
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