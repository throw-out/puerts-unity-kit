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

    [CastAssignable(
        typeof(byte), typeof(sbyte), typeof(char),
        typeof(short), typeof(ushort), typeof(int),
        typeof(uint), typeof(float), typeof(double)
    )]
    [System.Serializable]
    public class Number : Pair<System.Double> { }
    [CastAssignable(typeof(long), typeof(ulong))]
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

    [CastAssignable(
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
    public class CastAssignableAttribute : Attribute
    {
        public Type[] Types { get; private set; }
        public CastAssignableAttribute(Type firstType, params Type[] types)
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
                    if (m == null) continue;
                    results.AddRange(m);
                }
#if UNITY_EDITOR
                results.Sort((o1, o2) => o1.Index < o2.Index ? -1 : o1.Index > o2.Index ? 1 : 0);
#endif
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
                    if (pair == null || pair.Key != key)
                        continue;
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
                    else if (Convert.IsCastAssignable(pair.GetType(), pair.ValueType, newValue.GetType()))
                    {
                        setter.SetValue(pair, Convert.GetCastAssignableValue(pair.ValueType, newValue));
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
        /// <summary>
        /// 创建Delegate而非使用反射调用
        /// </summary>
        public static Func<TComponent, IEnumerable<IPair>>[] GetAccessor()
        {
            if (memberAccessor == null)
            {
                memberAccessor = typeof(TComponent).GetFields(Flags)
                    .Where(m => typeof(IEnumerable<IPair>).IsAssignableFrom(m.FieldType) && (m.IsPublic || m.IsDefined(typeof(UnityEngine.SerializeField), true)))
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

    public static class Convert
    {
        /// <summary>
        /// 是否可转换类型(强转或隐式转换)
        /// </summary>
        public static bool IsCastAssignable(Type elementType, Type elementValueType, Type valueType)
        {
            if (elementValueType.IsArray != valueType.IsArray)
                return false;

            Type[] castTypes = GetCastAssignableTypes(elementType);
            if (castTypes != null && castTypes.Contains(valueType))
            {
                return true;
            }
            if (elementValueType.IsArray)      //数组类型隐式分配
            {
                Type e1Type = elementValueType.GetElementType(), e2Type = valueType.GetElementType();
                if (e1Type.IsAssignableFrom(e2Type) || castTypes != null && castTypes.Contains(e2Type))
                {
                    return true;
                }
            }
            return IsImplicitAssignable(elementValueType, valueType);
        }
        /// <summary>
        /// 是否可以隐式转换
        /// </summary>
        public static bool IsImplicitAssignable(Type targetType, Type valueType)
        {
            return GetImplicitAssignableMethod(targetType, valueType) != null;
        }

        static Dictionary<Type, Func<object, object>> castConvertFuncs = new Dictionary<Type, Func<object, object>>()
        {
            { typeof(double), v => System.Convert.ToDouble(v)},
        };
        /// <summary>
        /// 进行类型转换(允许隐式转换分配)
        /// </summary>
        public static object GetCastAssignableValue(Type targetType, object value)
        {
            if (value == null || targetType.IsArray != value.GetType().IsArray)
            {
                return default;
            }
            //获取转化方法
            Func<object, object> convertFunc;
            castConvertFuncs.TryGetValue(targetType.IsArray ? targetType.GetElementType() : targetType, out convertFunc);
            if (convertFunc == null)
            {
                return GetImplicitAssignableValue(targetType, value);
            }
            if (targetType.IsArray)
            {
                Array array = (Array)value;
                Array newArray = Array.CreateInstance(targetType.GetElementType(), array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    var am = array.GetValue(i);
                    if (am == null) continue;
                    newArray.SetValue(convertFunc(am), i);
                }
                return newArray;
            }
            else
            {
                return convertFunc(value);
            }
        }
        /// <summary>
        /// 进行隐式转化
        /// </summary>
        public static object GetImplicitAssignableValue(Type targetType, object value)
        {
            if (value == null || targetType.IsArray != value.GetType().IsArray)
            {
                return default;
            }
            MethodInfo assignMethod = GetImplicitAssignableMethod(targetType, value.GetType());
            if (assignMethod == null)
            {
                return default;
            }
            return assignMethod.Invoke(null, new object[] { value });
        }

        static Dictionary<Type, Type[]> cacheCastAssignableTypes = new Dictionary<Type, Type[]>();
        static Type[] GetCastAssignableTypes(Type type)
        {
            Type[] types;
            if (!cacheCastAssignableTypes.TryGetValue(type, out types))
            {
                CastAssignableAttribute castDefine = type.GetCustomAttribute<CastAssignableAttribute>(false);
                if (castDefine != null)
                {
                    types = castDefine.Types;
                }
                cacheCastAssignableTypes.Add(type, types);
            }
            return types;
        }

        static Dictionary<Type, Tuple<Type, Type, MethodInfo>[]> cacheImpliciitAssignableTypes =
            new Dictionary<Type, Tuple<Type, Type, MethodInfo>[]>();
        static Tuple<Type, Type, MethodInfo>[] GetImplicitAssignableTypes(Type type)
        {
            Tuple<Type, Type, MethodInfo>[] types;
            if (!cacheImpliciitAssignableTypes.TryGetValue(type, out types))
            {
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                if (methods != null)
                {
                    types = methods
                        .Where(m => m.IsSpecialName && "op_Implicit".Equals(m.Name) && m.GetParameters().Length == 1)
                        .Select(m => new Tuple<Type, Type, MethodInfo>(m.ReturnType, m.GetParameters()[0].ParameterType, m))
                        .ToArray();
                }
                cacheImpliciitAssignableTypes.Add(type, types);
            }
            return types;
        }
        static MethodInfo GetImplicitAssignableMethod(Type targetType, Type valueType)
        {
            if (targetType.IsArray != valueType.IsArray)
                return null;
            if (targetType.IsArray)
            {
                return GetImplicitAssignableMethod(targetType.GetElementType(), valueType.GetElementType());
            }

            var assignMethod = GetImplicitAssignableTypes(targetType)?.FirstOrDefault(m =>
                targetType.IsAssignableFrom(m.Item1) &&
                m.Item2.IsAssignableFrom(valueType)
            ) ?? GetImplicitAssignableTypes(valueType)?.FirstOrDefault(m =>
                targetType.IsAssignableFrom(m.Item1) &&
                m.Item2.IsAssignableFrom(valueType)
            );
            return assignMethod?.Item3;
        }
    }
}