using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace XOR.Serializables
{
    public interface IAccessor
    {
        /// <summary>用于运行时获取组件序列化数据</summary>
        ResultPair[] GetProperties();
        /// <summary>设置属性值(Editor Only)</summary>
        void SetProperty(string key, object value);
        /// <summary>设置属性值编辑丶更新回调(Editor Only)</summary>
        void SetPropertyListener(Action<string, object> handler);
    }

    public class ResultPair
    {
        public string key;
        public object value;
        public ResultPair(IPair pair)
        {
            this.key = pair.Key;
            this.value = pair.Value;
        }
    }

    public interface IPair
    {
        int Index { get; }
        string Key { get; }
        object Value { get; }
        Type ValueType { get; }
    }
    public class Pair<T> : IPair
    {
        public int index;
        public string key;
        public T value;

        public int Index { get { return this.index; } }
        public string Key { get { return this.key; } }
        public object Value { get { return this.value; } }
        public Type ValueType { get { return typeof(T); } }
    }
    [System.Serializable]
    public class String : Pair<System.String> { }

    [Implicit(
        typeof(byte), typeof(sbyte), typeof(char),
        typeof(short), typeof(ushort), typeof(int),
        typeof(uint), typeof(float), typeof(double)
    )]
    [System.Serializable]
    public class Number : Pair<System.Double> { }
    [Implicit(typeof(long), typeof(ulong))]
    [System.Serializable]
    public class Bigint : Pair<System.Int64> { }
    [System.Serializable]
    public class Boolean : Pair<System.Boolean> { }
    [System.Serializable]
    public class Vector2 : Pair<UnityEngine.Vector2> { }
    [System.Serializable]
    public class Vector3 : Pair<UnityEngine.Vector3> { }
    [System.Serializable]
    public class Object : Pair<UnityEngine.Object> { }

    [MenuPath("Array/String")]
    [System.Serializable]
    public class StringArray : Pair<System.String[]> { }

    [Implicit(
        typeof(byte), typeof(sbyte), typeof(char),
        typeof(short), typeof(ushort), typeof(int),
        typeof(uint), typeof(float), typeof(double)
    )]
    [MenuPath("Array/Number")]
    [System.Serializable]
    public class NumberArray : Pair<System.Double[]> { }
    [MenuPath("Array/Bigint")]
    [System.Serializable]
    public class BigintArray : Pair<System.Int64[]> { }
    [MenuPath("Array/Boolean")]
    [System.Serializable]
    public class BooleanArray : Pair<System.Boolean[]> { }
    [MenuPath("Array/Vector2")]
    [System.Serializable]
    public class Vector2Array : Pair<UnityEngine.Vector2[]> { }
    [MenuPath("Array/Vector3")]
    [System.Serializable]
    public class Vector3Array : Pair<UnityEngine.Vector3[]> { }
    [MenuPath("Array/Object")]
    [System.Serializable]
    public class ObjectArray : Pair<UnityEngine.Object[]> { }

    /// <summary>
    /// 定义菜单路径
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class MenuPathAttribute : Attribute
    {
        public string Path { get; private set; }
        public MenuPathAttribute(string path)
        {
            this.Path = path;
        }
    }
    /// <summary>
    /// 定义隐式转换类型
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ImplicitAttribute : Attribute
    {
        public Type[] Types { get; private set; }
        public ImplicitAttribute(Type firstType, params Type[] types)
        {
            this.Types = types.Concat(new[] { firstType }).Distinct().ToArray();
        }
    }

    internal static class Accessor<TComponent>
        where TComponent : UnityEngine.Component
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static Func<TComponent, IEnumerable<XOR.Serializables.IPair>>[] memberAccessor;
        private static Dictionary<Type, FieldInfo> memberValueAccessor;

        public static IEnumerable<IPair> GetProperties(TComponent component)
        {
            if (GetAccessor().Length > 0)
            {
                List<IPair> results = new List<IPair>();
                foreach (var m in GetAccessor().Select(func => func(component)))
                {
                    results.AddRange(m);
                }
                return results;
            }
            return null;
        }
        public static void SetProperty(TComponent component, string key, object newValue)
        {
            foreach (var func in GetAccessor())
            {
                var pairs = func(component);
                if (pairs == null)
                    continue;
                foreach (var pair in pairs)
                {
                    if (pair == null)
                        continue;
                    if (pair.Key == key)
                    {
                        var setter = GetValueSetter(pair.GetType());
                        //if (setter == null) continue;
                        if (newValue == null)
                        {
                            setter.SetValue(pair, default);
                        }
                        else if (pair.ValueType.IsAssignableFrom(newValue.GetType()))
                        {
                            setter.SetValue(pair, newValue);
                        }
                        else
                        {
                            Logger.LogWarning($"Invail Type Assignment: The target type require {pair.ValueType.FullName}, but actual type is {newValue.GetType().FullName}");
                            setter.SetValue(pair, default);
                        }
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 创建Delegate而非使用反射调用
        /// </summary>
        public static Func<TComponent, IEnumerable<IPair>>[] GetAccessor()
        {
            if (memberAccessor == null)
            {
                memberAccessor = typeof(TComponent).GetFields(Flags)
                    .Where(m => typeof(IEnumerable<IPair>).IsAssignableFrom(m.FieldType) && (m.IsPublic || m.GetCustomAttribute<UnityEngine.SerializeField>() != null))
                    .Select(m => DelegateUtil.CreateDelegate<Func<TComponent, IEnumerable<IPair>>>(m, false))
                    .Where(func => func != null)
                    .ToArray();
            }
            return memberAccessor;
        }

        public static FieldInfo GetValueSetter(Type type)
        {
            if (memberValueAccessor == null)
            {
                memberValueAccessor = new Dictionary<Type, FieldInfo>();
            }
            FieldInfo accessor;
            if (!memberValueAccessor.TryGetValue(type, out accessor))
            {
                if (typeof(IPair).IsAssignableFrom(type))
                {
                    accessor = type.GetField("value", Flags);
                }
                memberValueAccessor.Add(type, accessor);
            }
            return accessor;
        }
    }
}