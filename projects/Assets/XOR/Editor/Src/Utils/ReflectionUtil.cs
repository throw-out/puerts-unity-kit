using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace XOR
{
    public static class ReflectionUtil
    {
        private static Dictionary<string, Type> types = new Dictionary<string, Type>();
        public static Type GetType(string fullName)
        {
            Type type;
            if (!types.TryGetValue(fullName, out type))
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(fullName, false);
                    if (type != null) break;
                }
                types.Add(fullName, type);
            }
            return type;
        }
    }
}
