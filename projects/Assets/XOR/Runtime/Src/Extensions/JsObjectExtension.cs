using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XOR
{
    public static class JsObjectExtension
    {
        public static TValue Get<TValue>(this Puerts.JSObject obj, string key)
        {
            Get<string, TValue>(obj, key, out var value);
            return value;
        }
        public static TValue Get<TKey, TValue>(this Puerts.JSObject obj, TKey key)
        {
            Get<TKey, TValue>(obj, key, out var value);
            return value;
        }
        public static TValue GetInPath<TValue>(this Puerts.JSObject obj, string key)
        {
            GetInPath<TValue>(obj, key, out var value);
            return value;
        }
        public static void Set<TValue>(this Puerts.JSObject obj, string key, TValue value)
        {
            Set<string, TValue>(obj, key, value);
        }

        public static void Get<TKey, TValue>(this Puerts.JSObject obj, TKey key, out TValue value)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var getter = accessor.Get<Func<Puerts.JSObject, TKey, TValue>>("get");
            value = getter(obj, key);
        }
        public static void Set<TKey, TValue>(this Puerts.JSObject obj, TKey key, TValue value)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var setter = accessor.Get<Action<Puerts.JSObject, TKey, TValue>>("set");
            setter(obj, key, value);
        }
        public static void GetInPath<TValue>(this Puerts.JSObject obj, string key, out TValue value)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var getter = accessor.Get<Func<Puerts.JSObject, string, TValue>>("getInPath");
            value = getter(obj, key);
        }
        public static void SetInPath<TValue>(this Puerts.JSObject obj, string key, TValue value)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var setter = accessor.Get<Action<Puerts.JSObject, string, TValue>>("setInPath");
            setter(obj, key, value);
        }
        public static bool ContainsKey<TKey>(this Puerts.JSObject obj, TKey key)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var containsKey = accessor.Get<Func<Puerts.JSObject, TKey, bool>>("containsKey");
            return containsKey(obj, key);
        }
        public static void RemoveKey<TKey>(this Puerts.JSObject obj, TKey key)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var removeKey = accessor.Get<Action<Puerts.JSObject, TKey>>("removeKey");
            removeKey(obj, key);
        }

        public static string[] GetKeys(this Puerts.JSObject obj)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var getKeys = accessor.Get<Func<Puerts.JSObject, string[]>>("getKeys");
            return getKeys(obj);
        }
        public static int Length(this Puerts.JSObject obj)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var length = accessor.Get<Func<Puerts.JSObject, int>>("length");
            return length(obj);
        }
        public static void ForEach<TValue>(this Puerts.JSObject obj, Action<string, TValue> action)
        {
            ForEach(obj, (key, value) =>
            {
                if (value != null && typeof(TValue).IsAssignableFrom(value.GetType()))
                {
                    action(key, (TValue)value);
                }
            });
        }
        public static void ForEach(this Puerts.JSObject obj, Action<string, object> action)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var forEach = accessor.Get<Action<Puerts.JSObject, Action<string, object>>>("forEach");
            forEach(obj, action);
        }

        public static void Call(this Puerts.JSObject obj, string methodName, params object[] args)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var call = accessor.Get<Action<Puerts.JSObject, string, object[]>>("call");
            call(obj, methodName, args);
        }
        public static TResult Call<TResult>(this Puerts.JSObject obj, string methodName, params object[] args)
        {
            var accessor = Accessor.GetOrCreate(obj);
            var call = accessor.Get<Func<Puerts.JSObject, string, object[], object>>("call");
            return (TResult)(call(obj, methodName, args));
        }

        public static T Cast<T>(this Puerts.JSObject obj)
            where T : class
        {
            if (typeof(T).IsInterface)
            {
                return JsTranslator.CreateInterfaceBridge(obj, typeof(T)) as T;
            }
            var accessor = Accessor.GetOrCreate(obj);
            var cast = accessor.Get<Func<Puerts.JSObject, T>>("cast");
            return cast(obj);
        }

        class Accessor : IDisposable
        {
            static System.Runtime.CompilerServices.ConditionalWeakTable<Puerts.JsEnv, Accessor> accessorList =
                new System.Runtime.CompilerServices.ConditionalWeakTable<Puerts.JsEnv, Accessor>();

            public static Accessor GetOrCreate(Puerts.JSObject obj)
            {
                return accessorList.GetValue(Helper.GetJsEnv(obj, true), (e) => new Accessor(e));
            }

            private WeakReference<Puerts.JsEnv> reference;
            private Dictionary<string, Dictionary<Type, Delegate>> methods;
            public Accessor(Puerts.JsEnv env)
            {
                this.reference = new WeakReference<Puerts.JsEnv>(env);
                this.methods = new Dictionary<string, Dictionary<Type, Delegate>>();
            }

            public void Dispose()
            {
                this.methods.Clear();
            }

            public TDelegate Get<TDelegate>(string utilMethodName)
                where TDelegate : Delegate
            {
                Dictionary<Type, Delegate> funcs;
                if (!methods.TryGetValue(utilMethodName, out funcs))
                {
                    funcs = new Dictionary<Type, Delegate>();
                    methods.Add(utilMethodName, funcs);
                }

                Delegate func;
                if (!funcs.TryGetValue(typeof(TDelegate), out func))
                {
                    if (reference.TryGetTarget(out var env))
                    {
                        func = env.XORUtilMethod<TDelegate>(utilMethodName);
                    }
                    funcs.Add(typeof(TDelegate), func);
                }
                return func as TDelegate;
            }
        }

        static class Helper
        {
            public static Puerts.JsEnv GetJsEnv(Puerts.JSObject obj, bool throwOnFailure)
            {
                if (Helper.JsEnv == null)
                {
                    if (throwOnFailure)
                        throw Helper.NullReferenceException();
                    Debug.LogWarning(Helper.NullReferenceException().Message);
                    return null;
                }
                Puerts.JsEnv env = Helper.JsEnv(obj);
                if (env == null)
                {
                    if (throwOnFailure)
                        throw Helper.NullReferenceException();
                    Debug.LogWarning(Helper.NullReferenceException().Message);
                    return null;
                }
                return env;
            }
            static Func<Puerts.JSObject, Puerts.JsEnv> _JsEnv = null;
            static Func<Puerts.JSObject, Puerts.JsEnv> JsEnv
            {
                get
                {
                    if (_JsEnv == null)
                    {
                        FieldInfo fieldInfo = typeof(Puerts.JSObject).GetField("jsEnv", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (fieldInfo != null)
                        {
                            _JsEnv = DelegateUtil.CreateFieldDelegate<Func<Puerts.JSObject, Puerts.JsEnv>>(fieldInfo, null, false);
                        }
                    }
                    return _JsEnv;
                }
            }
            public static Exception NullReferenceException()
            {
                return new NullReferenceException("Object reference null");
            }
        }
    }
}
