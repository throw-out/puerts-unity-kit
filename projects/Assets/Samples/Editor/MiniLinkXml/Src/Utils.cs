using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace MiniLinkXml
{
    public static class Utils
    {
        public static Type GetType(string typeName)
        {
            Type result = Type.GetType(typeName, false);
            if (result == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    result = assembly.GetType(typeName, false);
                    if (result != null)
                        break;
                }
            }
            return result;
        }
        public static IEnumerable<Type> GetTypes(string typeName)
        {
            List<Type> results = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName, false);
                if (type != null)
                    results.Add(type);
            }
            return results;
        }

        public static string GetFullname(Type type)
        {
            if (type.IsGenericType)
            {
                var fullName = string.IsNullOrEmpty(type.FullName) ? type.ToString() : type.FullName;
                var parts = fullName.Replace('+', '.').Split('`');
                var argTypenames = type.GetGenericArguments()
                    .Select(x => type.IsGenericTypeDefinition && x.IsGenericParameter ? "" : GetFullname(x)).ToArray();
                return parts[0] + "<" + string.Join(", ", argTypenames) + ">";
            }
            if (!string.IsNullOrEmpty(type.FullName))
                return type.FullName.Replace('+', '.');
            return type.ToString();
        }

        public delegate void ForEachLinkConfigureCallback(Type fieldType, Func<object> getValue);
        public static void ForEachLinkConfigure(ForEachLinkConfigureCallback callback)
        {
            if (callback == null)
                return;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                    continue;
                foreach (var type in assembly.GetExportedTypes())
                {
                    var fields = type
                        .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(f => f.IsDefined(typeof(LinkAttribute)) || f.IsDefined(typeof(LinkXmlAttribute)));
                    foreach (var field in fields)
                    {
                        callback(field.FieldType, () => field.GetValue(null));
                    }
                    var properties = type
                        .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(p => p.CanRead)
                        .Where(p => p.IsDefined(typeof(LinkAttribute)) || p.IsDefined(typeof(LinkXmlAttribute)));
                    foreach (var property in properties)
                    {
                        callback(property.PropertyType, () => property.GetValue(null));
                    }
                }
            }
        }

        public delegate void ForEachExtensionMethodDeclarationCallback(Type clsType, string methodName, Type thisArgType);
        public static void ForEachExtensionMethodDeclaration(ForEachExtensionMethodDeclarationCallback resolveCallback)
        {
            if (resolveCallback == null)
                return;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                    continue;
                foreach (Type clsType in assembly.GetExportedTypes())
                {
                    if (!clsType.IsAbstract || !clsType.IsSealed || !clsType.IsDefined(typeof(ExtensionAttribute), false))
                        continue;
                    var methods = clsType
                        .GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .Where(m => m.IsDefined(typeof(ExtensionAttribute)));
                    foreach (var method in methods)
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length < 1)
                            continue;
                        resolveCallback(clsType, method.Name, parameters[0].ParameterType);
                    }
                }
            }
        }

        public delegate void ForEachFilterFunctionCallback(MethodInfo methodInfo);
        public static void ForEachFilterFunction(ForEachFilterFunctionCallback callback)
        {
            if (callback == null)
                return;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                    continue;
                foreach (var type in assembly.GetExportedTypes())
                {
                    var methods = type
                        .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(f => f.IsDefined(typeof(FilterAttribute)));
                    foreach (var method in methods)
                    {
                        callback(method);
                    }
                }
            }
        }
    }
}
