using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Puerts;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    public static class GeneratorBridgeCode
    {
        const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        public class EventGenInfo
        {
            public string Name { get; private set; }
            public string TypeName { get; private set; }
            public static EventGenInfo From(EventInfo info)
            {
                return new EventGenInfo()
                {
                    Name = info.Name,
                    TypeName = GetFullName(info.EventHandlerType)
                };
            }
        }
        public class PropertyGenInfo
        {
            public string Name { get; private set; }
            public string TypeName { get; private set; }
            public bool CanRead { get; private set; }
            public bool CanWrite { get; private set; }
            public ParameterGenInfo[] Parameters { get; private set; }

            public static PropertyGenInfo From(PropertyInfo info)
            {
                ParameterGenInfo[] Parameters = info.Name == "Item" ? (info.CanRead ? info.GetGetMethod() : info.GetSetMethod())
                    .GetParameters()
                    .Select(p => ParameterGenInfo.From(p))
                    .ToArray() : null;

                return new PropertyGenInfo()
                {
                    Name = info.Name,
                    TypeName = GetFullName(info.PropertyType),
                    CanRead = info.CanRead,
                    CanWrite = info.CanWrite,
                    Parameters = Parameters,
                };
            }
        }
        public class ParameterGenInfo
        {
            public string Name { get; private set; }
            public string TypeName { get; private set; }
            public bool IsByRef { get; private set; }
            public bool IsIn;
            public bool IsOut;
            public bool IsParams { get; private set; }
            public static ParameterGenInfo From(ParameterInfo info)
            {
                return new ParameterGenInfo()
                {
                    Name = info.Name,
                    TypeName = GetFullName(info.ParameterType),
                    IsByRef = info.ParameterType.IsByRef,
                    IsIn = info.IsIn,
                    IsOut = !info.IsIn && info.IsOut,
                    IsParams = info.IsDefined(typeof(ParamArrayAttribute), false),
                };
            }
        }
        public class MethodGenInfo
        {
            public string Name { get; private set; }
            public string ReturnTypeName { get; private set; }
            public ParameterGenInfo[] Parameters { get; private set; }

            public static MethodGenInfo From(MethodInfo info)
            {
                return new MethodGenInfo()
                {
                    Name = info.Name,
                    ReturnTypeName = info.ReturnType == typeof(void) ? "void" : GetFullName(info.ReturnType),
                    Parameters = info.GetParameters()
                        .Select(p => ParameterGenInfo.From(p))
                        .ToArray(),
                };
            }
        }
        public class TypeGenInfo
        {
            public string Name { get; private set; }
            public string InterfaceTypeName { get; private set; }

            public EventGenInfo[] Events { get; private set; }
            public MethodGenInfo[] Methods { get; private set; }
            public PropertyGenInfo[] Properties { get; private set; }

            public static TypeGenInfo From(Type type)
            {
                MemberInfo[] members = new Type[] { type }
                    .Concat(type.GetInterfaces())
                    .SelectMany(i => i.GetMembers())
                    .Distinct()
                    .ToArray();
                string fullName = GetFullName(type);
                return new TypeGenInfo()
                {
                    Name = fullName.Replace(".", "_") + "_Bridge",
                    InterfaceTypeName = fullName,
                    Events = members
                        .Where(m => m.MemberType == MemberTypes.Event)
                        .Select(m => EventGenInfo.From((EventInfo)m))
                        .ToArray(),
                    Methods = members
                        .Where(m => m.MemberType == MemberTypes.Method && !((MethodInfo)m).IsSpecialName)
                        .Select(m => MethodGenInfo.From((MethodInfo)m))
                        .ToArray(),
                    Properties = members
                        .Where(m => m.MemberType == MemberTypes.Property)
                        .Select(m => PropertyGenInfo.From((PropertyInfo)m))
                        .ToArray(),
                };
            }
        }

        [MenuItem("Tools/PuerTS/Generate BridgeCode[Interface]", false, 1)]
        public static void GenerateUsingCode()
        {
            var start = DateTime.Now;
            var saveTo = Configure.GetCodeOutputDirectory();
            Directory.CreateDirectory(saveTo);
            GenerateCode(saveTo);
            Debug.Log("finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms");
            AssetDatabase.Refresh();
        }

        static void GenerateCode(string saveTo)
        {
            var configure = Configure.GetConfigureByTags(new List<string>() {
                "Puerts.BindingAttribute",
            });
            var genTypes = configure["Puerts.BindingAttribute"].Select(kv => kv.Key)
                .Where(o => o is Type)
                .Cast<Type>()
                .Where(t => t.IsInterface && !t.IsGenericTypeDefinition);
            var genInfos = genTypes.Select(t => TypeGenInfo.From(t)).ToArray();

            using (var jsEnv = new JsEnv())
            {
                Puerts.ThirdParty.CommonJS.InjectSupportForCJS(jsEnv);
                var autoRegisterRender = jsEnv.Eval<Func<TypeGenInfo[], string>>("require('puerts/templates/wrapper-interface-bridge.tpl.cjs')");
                using (StreamWriter textWriter = new StreamWriter(saveTo + "AutoStaticCodeInterfaceBridge.cs", false, Encoding.UTF8))
                {
                    string fileContext = autoRegisterRender(genInfos);
                    textWriter.Write(fileContext);
                    textWriter.Flush();
                }
            }
        }

        static string GetFullName(Type type)
        {
            if (type.IsByRef)
            {
                //return GetFullName(Nullable.GetUnderlyingType(type));
                return GetFullName(type.GetElementType());
            }
            if (type.IsGenericType)
            {
                var fullName = string.IsNullOrEmpty(type.FullName) ? type.ToString() : type.FullName;
                var parts = fullName.Replace('+', '.').Split('`');
                var argTypenames = type.GetGenericArguments()
                    .Select(x => GetFullName(x)).ToArray();
                return parts[0] + "<" + string.Join(", ", argTypenames) + ">";
            }
            if (!string.IsNullOrEmpty(type.FullName))
                return type.FullName.Replace('+', '.');
            return type.ToString();
        }
    }
}
