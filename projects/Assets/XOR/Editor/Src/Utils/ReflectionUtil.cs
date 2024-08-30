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
            if (string.IsNullOrEmpty(fullName))
                return null;
            if (types.TryGetValue(fullName, out var type))
                return type;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(fullName, false);
                if (type != null)
                    break;
            }
            types.Add(fullName, type);
            return type;
        }
    }

    internal class PropertyAccessor<TObject, TField>
    {
        const BindingFlags Flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly string memberName;
        private MemberInfo member;

        public PropertyAccessor(string memberName)
        {
            this.memberName = memberName;
        }
        public TField Get(TObject obj)
        {
            TryGetMember();
            if (member is FieldInfo field)
            {
                return (TField)field.GetValue(field.IsStatic ? default : obj);
            }
            else if (member is PropertyInfo property)
            {
                return (TField)property.GetValue(property.GetMethod.IsStatic ? default : obj);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        public void Set(TObject obj, TField value)
        {
            TryGetMember();
            if (member is FieldInfo field)
            {
                field.SetValue(field.IsStatic ? default : obj, value);
            }
            else if (member is PropertyInfo property)
            {
                property.SetValue(property.GetMethod.IsStatic ? default : obj, value);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        void TryGetMember()
        {
            if (member != null)
                return;
            member = typeof(TObject).GetField(this.memberName, Flags) as MemberInfo ?? typeof(TObject).GetProperty(this.memberName, Flags);
            if (member == null ||
                member is FieldInfo && !typeof(TField).IsAssignableFrom(((FieldInfo)member).FieldType) ||
                member is PropertyInfo && !typeof(TField).IsAssignableFrom(((PropertyInfo)member).PropertyType)
            )
            {
                throw new InvalidCastException();
            }
        }
    }
}
