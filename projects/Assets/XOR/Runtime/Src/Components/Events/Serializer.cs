using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR.Events
{
    public static class Serializer
    {
        public static string ToString<T>(T data)
        {
            if (SerializeFuncs.TryGetValue(typeof(T), out var func))
            {
                return func(data);
            }
            return default;
        }
        public static T ToData<T>(string data)
        {
            if (DeserializeFuncs.TryGetValue(typeof(T), out var func))
            {
                return (T)func(data);
            }
            return default;
        }
        public static bool IsSupport<T>()
        {
            return IsSupport(typeof(T));
        }
        public static bool IsSupport(Type type)
        {
            return SerializeFuncs.ContainsKey(type);
        }

        static readonly Dictionary<Type, Func<object, string>> SerializeFuncs = new Dictionary<Type, Func<object, string>>(){
            { typeof(string), (data) => (string)data },
            { typeof(double), (data) => data.ToString() },
            { typeof(bool), (data) => data.ToString() },
            { typeof(long), (data) => data.ToString() },
            { typeof(Vector2), (data) => ToString((Vector2)data) },
            { typeof(Vector3), (data) => ToString((Vector3)data) },
            { typeof(Color), (data) => ToString((Color)data) },
            { typeof(Color32), (data) => ToString((Color32)data) },
        };
        static readonly Dictionary<Type, Func<string, object>> DeserializeFuncs = new Dictionary<Type, Func<string, object>>(){
            { typeof(string), (data) => data },
            { typeof(double), (data) => Parse<double>(double.TryParse, data) },
            { typeof(bool), (data) => Parse<bool>(bool.TryParse, data) },
            { typeof(long), (data) => Parse<long>(long.TryParse, data) },
            { typeof(Vector2), (data) => ParseVector2(data) },
            { typeof(Vector3), (data) => ParseVector3(data) },
            { typeof(Color), (data) => ParseColor(data) },
            { typeof(Color32), (data) => ParseColor32(data) },
        };

        delegate bool TryParseFunction<T>(string data, out T result);
        static T Parse<T>(TryParseFunction<T> tryParse, string data)
        {
            tryParse(data, out T value);
            return value;
        }

        static Vector2 ParseVector2(string data)
        {
            if (string.IsNullOrEmpty(data))
                return default;
            string[] arr = data.Split(',');
            if (arr.Length >= 2 &&
                float.TryParse(arr[0], out var x) &&
                float.TryParse(arr[1], out var y)
            )
            {
                return new Vector2(x, y);
            }
            return default;
        }
        static Vector3 ParseVector3(string data)
        {
            if (string.IsNullOrEmpty(data))
                return default;
            string[] arr = data.Split(',');
            if (arr.Length >= 3 &&
                float.TryParse(arr[0], out var x) &&
                float.TryParse(arr[1], out var y) &&
                float.TryParse(arr[2], out var z)
            )
            {
                return new Vector3(x, y, z);
            }
            return default;
        }
        static Color ParseColor(string data)
        {
            if (string.IsNullOrEmpty(data))
                return default;
            string[] arr = data.Split(',');
            if (arr.Length >= 3 &&
                float.TryParse(arr[0], out var r) &&
                float.TryParse(arr[1], out var g) &&
                float.TryParse(arr[2], out var b)
            )
            {
                float a;
                if (arr.Length < 4 | !float.TryParse(arr[3], out a))
                {
                    a = Color.white.a;
                }
                return new Color(r, g, b, a);
            }
            return Color.white;
        }
        static Color32 ParseColor32(string data)
        {
            return ParseColor(data);
        }
        static string ToString(Vector2 value)
        {
            return $"{value.x},{value.y}";
        }
        static string ToString(Vector3 value)
        {
            return $"{value.x},{value.y},{value.z}";
        }
        static string ToString(Color value)
        {
            return $"{value.r},{value.g},{value.b},{value.a}";
        }
    }
}