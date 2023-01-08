using System;
using System.Collections.Generic;
using UnityEditor;

namespace XOR
{
    public static class Prefs
    {
        /// <summary>
        /// 框架服务总开关
        /// </summary>
        /// <typeparam name="bool"></typeparam>
        /// <returns></returns>
        internal static readonly Accessor<bool> Enable = new Accessor<bool>("Enable");

        /// <summary>
        /// 指定项目路径(tsconfig.json文件路径)
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        internal static readonly Accessor<string> ProjectPath = new Accessor<string>("ProjectPath");

        /// <summary>
        /// 指定语言环境
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <returns></returns>
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
