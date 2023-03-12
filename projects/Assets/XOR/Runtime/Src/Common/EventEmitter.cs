using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace XOR
{
    public class EventEmitter
    {
        public delegate void DynamicHandler(params object[] args);

        private Dictionary<string, List<Handler>> events;

        public void On(string eventName, Action handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler));
        }
        public void On<T1>(string eventName, Action<T1> handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler));
        }
        public void On<T1, T2>(string eventName, Action<T1, T2> handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler));
        }
        public void On<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler));
        }
        public void On<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler));
        }
        public void On(string eventName, DynamicHandler handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler));
        }

        public void Once(string eventName, Action handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler, 1));
        }
        public void Once<T1>(string eventName, Action<T1> handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler, 1));
        }
        public void Once<T1, T2>(string eventName, Action<T1, T2> handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler, 1));
        }
        public void Once<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler, 1));
        }
        public void Once<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler, 1));
        }
        public void Once(string eventName, DynamicHandler handler)
        {
            List<Handler> handelrs = GetHandlers(eventName, true);
            handelrs.Add(new Handler(handler, 1));
        }

        public void Remove<TDelegate>(string eventName, TDelegate handler)
            where TDelegate : Delegate
        {
            List<Handler> handelrs = GetHandlers(eventName, false);
            if (handelrs == null || handelrs.Count == 0)
                return;
            for (int i = handelrs.Count - 1; i >= 0; i--)
            {
                if (handelrs[i].Equals(handler))
                {
                    handelrs.RemoveAt(i);
                }
            }
        }
        public void RemoveAll(string eventName)
        {
            if (this.events == null || this.events.ContainsKey(eventName))
                return;
            this.events.Remove(eventName);
        }
        public void Clear()
        {
            if (this.events == null)
                return;
            this.events.Clear();
        }

        protected void Emit(string eventName)
        {
            List<Handler> handelrs = GetHandlers(eventName, false);
            if (handelrs == null || handelrs.Count == 0)
                return;
            foreach (Handler handler in handelrs)
            {
                handler.Invoke();
            }
            RemoveCompletedHandlers(handelrs);
        }
        protected void Emit<T1>(string eventName, T1 arg1)
        {
            List<Handler> handelrs = GetHandlers(eventName, false);
            if (handelrs == null || handelrs.Count == 0)
                return;
            foreach (Handler handler in handelrs)
            {
                handler.Invoke(arg1);
            }
            RemoveCompletedHandlers(handelrs);
        }
        protected void Emit<T1, T2>(string eventName, T1 arg1, T2 arg2)
        {
            List<Handler> handelrs = GetHandlers(eventName, false);
            if (handelrs == null || handelrs.Count == 0)
                return;
            foreach (Handler handler in handelrs)
            {
                handler.Invoke(arg1, arg2);
            }
            RemoveCompletedHandlers(handelrs);
        }
        protected void Emit<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            List<Handler> handelrs = GetHandlers(eventName, false);
            if (handelrs == null || handelrs.Count == 0)
                return;
            foreach (Handler handler in handelrs)
            {
                handler.Invoke(arg1, arg2, arg3);
            }
            RemoveCompletedHandlers(handelrs);
        }
        protected void Emit<T1, T2, T3, T4>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            List<Handler> handelrs = GetHandlers(eventName, false);
            if (handelrs == null || handelrs.Count == 0)
                return;
            foreach (Handler handler in handelrs)
            {
                handler.Invoke(arg1, arg2, arg3, arg4);
            }
            RemoveCompletedHandlers(handelrs);
        }
        protected void Emit(string eventName, params object[] args)
        {
            List<Handler> handelrs = GetHandlers(eventName, false);
            if (handelrs == null || handelrs.Count == 0)
                return;
            foreach (Handler handler in handelrs)
            {
                handler.Invoke(args);
            }
            RemoveCompletedHandlers(handelrs);
        }


        List<Handler> GetHandlers(string eventName, bool create = false)
        {
            if (this.events == null)
            {
                if (!create)
                    return null;
                this.events = new Dictionary<string, List<Handler>>();
            }
            List<Handler> handelrs;
            if (!this.events.TryGetValue(eventName, out handelrs) && create)
            {
                handelrs = new List<Handler>();
                this.events.Add(eventName, handelrs);
            }
            return handelrs;
        }
        void RemoveCompletedHandlers(List<Handler> handlers)
        {
            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].IsCompleted)
                {
                    handlers.RemoveAt(i);
                }
            }
        }

        class Handler
        {
            private Delegate callback;
            private int count;
            private int argCount;
            private Type[] argTypes;

            public bool IsCompleted { get; private set; } = false;

            public Handler(Delegate callback, int count = -1)
            {
                this.callback = callback;
                this.count = count;
                if (callback != null && !(callback is DynamicHandler))
                {
                    this.argTypes = GetDelegateParameters(callback.GetType());
                    this.argCount = this.argTypes.Length;
                }
                else
                {
                    this.argCount = -1;
                }
            }
            public bool Equals(Delegate target)
            {
                return callback != null ? callback.Equals(target) : target == null;
            }

            public void Invoke()
            {
                if (IsCompleted)
                    return;
                if (_Invoke())
                {
                    InvokeComplete();
                }
                else if (_DynamicInvoke())
                {
                    InvokeComplete();
                }
                else if (callback is DynamicHandler)
                {
                    _DynamicHandlerInvoke();
                    InvokeComplete();
                }
            }
            public void Invoke<T1>(T1 arg1)
            {
                if (IsCompleted)
                    return;
                if (_Invoke(arg1) ||
                    _Invoke()
                )
                {
                    InvokeComplete();
                }
                else if (_DynamicInvoke(arg1) ||
                    _DynamicInvoke()
                )
                {
                    InvokeComplete();
                }
                else if (callback is DynamicHandler)
                {
                    _DynamicHandlerInvoke(arg1);
                    InvokeComplete();
                }
            }
            public void Invoke<T1, T2>(T1 arg1, T2 arg2)
            {
                if (IsCompleted)
                    return;
                if (_Invoke(arg1, arg2) ||
                    _Invoke(arg1) || _Invoke(arg2) ||
                    _Invoke()
                )
                {
                    InvokeComplete();
                }
                else if (_DynamicInvoke(arg1, arg2) ||
                    _DynamicInvoke(arg1) || _DynamicInvoke(arg2) ||
                    _DynamicInvoke()
                )
                {
                    InvokeComplete();
                }
                else if (callback is DynamicHandler)
                {
                    _DynamicHandlerInvoke(arg1, arg2);
                    InvokeComplete();
                }
            }
            public void Invoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
            {
                if (IsCompleted)
                    return;

                if (_Invoke(arg1, arg2, arg3) ||
                    _Invoke(arg1, arg2) || _Invoke(arg2, arg3) ||
                    _Invoke(arg1) || _Invoke(arg2) || _Invoke(arg3) ||
                    _Invoke()
                )
                {
                    InvokeComplete();
                }
                else if (_DynamicInvoke(arg1, arg2, arg3) ||
                    _DynamicInvoke(arg1, arg2) || _DynamicInvoke(arg2, arg3) ||
                    _DynamicInvoke(arg1) || _DynamicInvoke(arg2) || _DynamicInvoke(arg3) ||
                    _DynamicInvoke()
                )
                {
                    InvokeComplete();
                }
                else if (callback is DynamicHandler)
                {
                    _DynamicHandlerInvoke(arg1, arg2, arg3);
                    InvokeComplete();
                }
            }
            public void Invoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                if (IsCompleted)
                    return;

                if (_Invoke(arg1, arg2, arg3, arg4) ||
                    _Invoke(arg1, arg2, arg3) || _Invoke(arg2, arg3, arg4) ||
                    _Invoke(arg1, arg2) || _Invoke(arg2, arg3) || _Invoke(arg3, arg4) ||
                    _Invoke(arg1) || _Invoke(arg2) || _Invoke(arg3) || _Invoke(arg4) ||
                    _Invoke()
                )
                {
                    InvokeComplete();
                }
                else if (_DynamicInvoke(arg1, arg2, arg3, arg4) ||
                    _DynamicInvoke(arg1, arg2, arg3) || _DynamicInvoke(arg2, arg3, arg4) ||
                    _DynamicInvoke(arg1, arg2) || _DynamicInvoke(arg2, arg3) || _DynamicInvoke(arg3, arg4) ||
                    _DynamicInvoke(arg1) || _DynamicInvoke(arg2) || _DynamicInvoke(arg3) || _DynamicInvoke(arg4) ||
                    _DynamicInvoke()
                )
                {
                    InvokeComplete();
                }
                else if (callback is DynamicHandler)
                {
                    _DynamicHandlerInvoke(arg1, arg2, arg3, arg4);
                    InvokeComplete();
                }
            }
            public void Invoke(object[] args)
            {
                if (callback is DynamicHandler)
                {
                    _DynamicHandlerInvoke(args);
                    InvokeComplete();
                }
            }

            bool _Invoke()
            {
                if (argCount == 0 && callback is Action)
                {
                    try
                    {
                        ((Action)callback)();
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }
            bool _Invoke<T1>(T1 arg1)
            {
                if (argCount == 1 && callback is Action<T1>)
                {
                    try
                    {
                        ((Action<T1>)callback)(arg1);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }
            bool _Invoke<T1, T2>(T1 arg1, T2 arg2)
            {
                if (argCount == 2 && callback is Action<T1, T2>)
                {
                    try
                    {
                        ((Action<T1, T2>)callback)(arg1, arg2);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }
            bool _Invoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
            {
                if (argCount == 3 && callback is Action<T1, T2, T3>)
                {
                    try
                    {
                        ((Action<T1, T2, T3>)callback)(arg1, arg2, arg3);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }
            bool _Invoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                if (argCount == 4 && callback is Action<T1, T2, T3, T4>)
                {
                    try
                    {
                        ((Action<T1, T2, T3, T4>)callback)(arg1, arg2, arg3, arg4);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }

            bool _DynamicInvoke()
            {
                if (this.argCount == 0)
                {
                    try
                    {
                        callback.DynamicInvoke();
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }
            bool _DynamicInvoke<T1>(T1 arg1)
            {
                if (this.argCount == 1 && CheckParameters(typeof(T1)))
                {
                    try
                    {
                        callback.DynamicInvoke(arg1);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }
            bool _DynamicInvoke<T1, T2>(T1 arg1, T2 arg2)
            {
                if (this.argCount == 2 && CheckParameters(typeof(T1), typeof(T2)))
                {
                    try
                    {
                        callback.DynamicInvoke(arg1, arg2);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }
            bool _DynamicInvoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
            {
                if (this.argCount == 3 && CheckParameters(typeof(T1), typeof(T2), typeof(T3)))
                {
                    try
                    {
                        callback.DynamicInvoke(arg1, arg2, arg3);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }
            bool _DynamicInvoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                if (this.argCount == 4 && CheckParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4)))
                {
                    try
                    {
                        callback.DynamicInvoke(arg1, arg2, arg3, arg4);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    return true;
                }
                return false;
            }

            void _DynamicHandlerInvoke(params object[] args)
            {
                try
                {
                    ((DynamicHandler)callback)(args);
                }
                catch (Exception e) { Debug.LogError(e); }
            }

            bool CheckParameters(params Type[] types)
            {
                if (types.Length != this.argCount)
                    return false;
                for (int i = 0; i < argCount; i++)
                {
                    if (
                        types[i] == null && argTypes[i].IsValueType ||
                        types[i] != null && !argTypes[i].IsAssignableFrom(types[i])
                    )
                    {
                        return false;
                    }
                }
                return true;
            }
            void InvokeComplete()
            {
                if (count <= 0)
                    return;
                count--;
                if (count == 0)
                {
                    this.IsCompleted = true;
                }
            }

            private static readonly Dictionary<Type, Type[]> _cacheParameters =
                new Dictionary<Type, Type[]>();
            private static Type[] GetDelegateParameters(Type delegateType)
            {
                Type[] argTypes;
                if (!_cacheParameters.TryGetValue(delegateType, out argTypes))
                {
                    argTypes = delegateType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .GetParameters()
                        .Select(p => p.ParameterType)
                        .ToArray();
                    _cacheParameters.Add(delegateType, argTypes);
                }
                return argTypes;
            }
        }
    }
}