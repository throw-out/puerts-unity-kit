using System;
using System.Collections.Generic;
using UnityEditor;

namespace XOR
{
    internal static class Prefs
    {
        /// <summary>
        /// 框架AST服务开关
        /// </summary>
        internal static readonly Accessor<bool> ASTEnable = new Accessor<bool>("ASTEnable");

        /// <summary>
        /// 检测key是否重定义
        /// </summary>
        internal static readonly Accessor<bool> CheckKeyRedefinition = new Accessor<bool>("CheckKeyRedefinition", true);
        /// <summary>
        /// 检测key是否为一个有效命名
        /// /// </summary>
        internal static readonly Accessor<bool> CheckKeyValidity = new Accessor<bool>("CheckKeyValidity", true);

        /// <summary>
        /// 指定语言环境
        /// </summary>
        internal static readonly Accessor<int> Language = new Accessor<int>("Language");

        internal class Accessor<T>
        {
            private readonly string key;
            private T _value;
            private T _defaultValue;

            public Accessor(string key, T defaultValue = default)
            {
                this.key = $"EditorPrefs.XOR.{key}";
                this.Read();
            }

            private void Read()
            {
                Func<string, object, object> Getter;
                if (Getters.TryGetValue(typeof(T), out Getter))
                {
                    _value = (T)Getter(key, _defaultValue);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Invaild data type: {typeof(T).FullName}");
                }
            }
            private void Save()
            {
                Action<string, object> Setter;
                if (Setters.TryGetValue(typeof(T), out Setter))
                {
                    Setter(key, _value);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Invaild data type: {typeof(T).FullName}");
                }
            }

            public void SetValue(T value)
            {
                if (this._value.Equals(value))
                    return;
                this._value = value;
                this.Save();
            }
            public T GetValue()
            {
                return this._value;
            }

            public static implicit operator T(Accessor<T> v)
            {
                if (v == null)
                    return default;
                return v.GetValue();
            }

            static Dictionary<Type, Action<string, object>> Setters = new Dictionary<Type, Action<string, object>>()
            {
                {typeof(bool), (key, value) => EditorPrefs.SetBool(key, (bool)value) },
                {typeof(int), (key, value) => EditorPrefs.SetInt(key, (int)value) },
                {typeof(float), (key, value) => EditorPrefs.SetFloat(key, (float)value) },
                {typeof(string), (key, value) => EditorPrefs.SetString(key, (string)value) },
            };
            static Dictionary<Type, Func<string, object, object>> Getters = new Dictionary<Type, Func<string, object, object>>()
            {
                {typeof(bool), (key, defaultValue) => EditorPrefs.GetBool(key, (bool)defaultValue) },
                {typeof(int), (key, defaultValue) => EditorPrefs.GetInt(key, (int)defaultValue) },
                {typeof(float), (key, defaultValue) => EditorPrefs.GetFloat(key, (float)defaultValue) },
                {typeof(string), (key, defaultValue) => EditorPrefs.GetString(key, (string)defaultValue) },
            };
        }
    }
}
