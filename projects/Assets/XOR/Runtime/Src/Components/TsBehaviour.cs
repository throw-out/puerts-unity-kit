using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XOR.Behaviour;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XOR
{
    //[DisallowMultipleComponent]
    public class TsBehaviour : MonoBehaviour, IDisposable
    {
        private static HashSet<WeakReference<TsBehaviour>> referenceInstances;
        private WeakReference<TsBehaviour> referenceSelf;
        #region Editor 
#if UNITY_EDITOR
        public ModuleInfo Module { get; set; }
#endif
        #endregion

        //Unity 接口组件
        private int objectID;
        public bool isObtainedObejctID;
        private List<Behaviour.Behaviour> behaviours;
        private Behaviour.Args.Mono @base;
        private Action<Behaviour.Args.Mono> lifecycle;
        public bool IsActivated { get; private set; } = false;      //是否已执行Awake回调
        public bool IsStarted { get; private set; } = false;        //是否已执行Start回调
        public bool IsDestroyed { get; private set; } = false;      //是否已执行OnDestroy回调
        public bool IsEnable { get; private set; } = false;

        public TsBehaviour()
        {
            if (referenceInstances == null)
                referenceInstances = new HashSet<WeakReference<TsBehaviour>>();

            referenceSelf = new WeakReference<TsBehaviour>(this);
            referenceInstances.Add(referenceSelf);
        }
        ~TsBehaviour()
        {
            if (referenceInstances != null)
            {
                referenceInstances.Remove(referenceSelf);
            }
            this.Dispose(false);
        }

        public int GetObjectID()
        {
            if (!isObtainedObejctID)
            {
                isObtainedObejctID = true;
                objectID = this.GetInstanceID();
            }
            return objectID;
        }

        protected virtual void Awake()
        {
            GetObjectID();
            IsActivated = true;
            if ((@base & Behaviour.Args.Mono.Awake) > 0)
            {
                @base ^= Behaviour.Args.Mono.Awake;
                Invoke(Behaviour.Args.Mono.Awake, lifecycle);
            }
        }
        protected virtual void Start()
        {
            IsStarted = true;
            if ((@base & Behaviour.Args.Mono.Start) > 0)
            {
                @base ^= Behaviour.Args.Mono.Start;
                Invoke(Behaviour.Args.Mono.Start, lifecycle);
            }
        }
        protected virtual void OnEnable()
        {
            IsEnable = true;
            if ((@base & Behaviour.Args.Mono.OnEnable) > 0)
            {
                Invoke(Behaviour.Args.Mono.OnEnable, lifecycle);
            }
            if (behaviours != null)
            {
                foreach (var behaviour in behaviours)
                {
                    behaviour.enabled = true;
                }
            }
        }
        protected virtual void OnDisable()
        {
            IsEnable = false;
            if ((@base & Behaviour.Args.Mono.OnDisable) > 0)
            {
                Invoke(Behaviour.Args.Mono.OnDisable, lifecycle);
            }
            if (behaviours != null)
            {
                foreach (var behaviour in behaviours)
                {
                    behaviour.enabled = false;
                }
            }
        }
        protected virtual void OnDestroy()
        {
            IsDestroyed = true;
            if ((@base & Behaviour.Args.Mono.OnDestroy) > 0)
            {
                @base ^= Behaviour.Args.Mono.OnDestroy;
                Invoke(Behaviour.Args.Mono.OnDestroy, lifecycle);
            }
            Dispose(true);
        }

        public void CreateMono(Behaviour.Args.Mono methods, Action<Behaviour.Args.Mono> callback)
        {
            Behaviour.Args.Mono @base = default;
            foreach (var v in Behaviour.Args.Extensions.GetMonoBase())
            {
                if ((methods & v) > 0)
                {
                    methods ^= v;
                    @base |= v;
                }
            }
            if (@base > 0)
            {
                this.@base |= @base;
                if (callback != null)
                    lifecycle += callback;
                if ((@base & Behaviour.Args.Mono.Awake) > 0 && !IsDestroyed && IsActivated)
                    Invoke(Behaviour.Args.Mono.Awake, callback);
                if ((@base & Behaviour.Args.Mono.Start) > 0 && !IsDestroyed && IsStarted)
                    Invoke(Behaviour.Args.Mono.Start, callback);
                if ((@base & Behaviour.Args.Mono.OnDestroy) > 0 && IsDestroyed)
                    Invoke(Behaviour.Args.Mono.OnDestroy, callback);
                if ((@base & Behaviour.Args.Mono.OnEnable) > 0 && !IsDestroyed && IsEnable)
                    Invoke(Behaviour.Args.Mono.OnEnable, callback);
                if ((@base & Behaviour.Args.Mono.OnDisable) > 0 && !IsDestroyed && IsActivated && !IsEnable)
                    Invoke(Behaviour.Args.Mono.OnDisable, callback);
            }
            if (methods <= 0)
                return;
            Create(Factory.Mono, methods, callback);
        }
        public void CreateMonoBoolean(Behaviour.Args.MonoBoolean methods, Action<Behaviour.Args.MonoBoolean, bool> callback)
        {
            Create(Factory.MonoBoolean, methods, callback);
        }
        public void CreateMouse(Behaviour.Args.Mouse methods, Action<Behaviour.Args.Mouse> callback)
        {
            Create(Factory.Mouse, methods, callback);
        }
        public void CreateGizmos(Behaviour.Args.Gizmos methods, Action<Behaviour.Args.Gizmos> callback)
        {
            Create(Factory.Gizmos, methods, callback);
        }
        public void CreateEventSystems(Behaviour.Args.EventSystems methods, Action<Behaviour.Args.EventSystems, PointerEventData> callback)
        {
            Create(Factory.EventSystems, methods, callback);
        }
        public void CreatePhysicsCollider(Behaviour.Args.PhysicsCollider methods, Action<Behaviour.Args.PhysicsCollider, Collider> callback)
        {
            Create(Factory.PhysicsCollider, methods, callback);
        }
        public void CreatePhysicsCollider2D(Behaviour.Args.PhysicsCollider2D methods, Action<Behaviour.Args.PhysicsCollider2D, Collider2D> callback)
        {
            Create(Factory.PhysicsCollider2D, methods, callback);
        }
        public void CreatePhysicsCollision(Behaviour.Args.PhysicsCollision methods, Action<Behaviour.Args.PhysicsCollision, Collision> callback)
        {
            Create(Factory.PhysicsCollision, methods, callback);
        }
        public void CreatePhysicsCollision2D(Behaviour.Args.PhysicsCollision2D methods, Action<Behaviour.Args.PhysicsCollision2D, Collision2D> callback)
        {
            Create(Factory.PhysicsCollision2D, methods, callback);
        }

        private void Create<T, TDelegate, TComponent>(Factory.Behaviours<T, TDelegate, TComponent> behaviours, T methods, TDelegate callback)
            where T : Enum
            where TDelegate : Delegate
            where TComponent : Behaviour<TDelegate>
        {
            if (this.behaviours == null)
            {
                this.behaviours = new List<XOR.Behaviour.Behaviour>();
            }
            if (behaviours.Contains(methods))
            {
                var component = behaviours.Create(gameObject, methods, callback, Invoker.Default);
                if (component != null)
                {
                    component.ObjectID = objectID;
                    this.behaviours.Add(component);
                }
                return;
            }
            var value = behaviours.GetEnumUInt32(methods);
            foreach (var method in behaviours.Methods)
            {
                if (value <= 0)
                    break;
                if ((value & method) <= 0)
                    continue;
                var component = behaviours.Create(gameObject, method, callback, Invoker.Default);
                if (component != null)
                {
                    component.ObjectID = objectID;
                    this.behaviours.Add(component);
                }
                value ^= method;
            }
        }
        private void Invoke(Behaviour.Args.Mono method, Action<Behaviour.Args.Mono> callback)
        {
            if (callback != null)
            {
                callback(method);
            }
            else if (Invoker.Default != null && isObtainedObejctID)
            {
                Invoker.Default.Invoke(objectID, method);
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool destroy)
        {
            this.@base = 0;
            this.lifecycle = null;

            if (behaviours != null)
            {
                foreach (var behaviour in behaviours)
                {
                    behaviour.Release();
                    if (destroy) Destroy(behaviour);
                }
                behaviours.Clear();
            }
            behaviours = null;
            //如果全程gameObject.activeSelf=false或者DestroyImmediate, 则OnDestroy不会被调用
            //此时需要额外通知ts层gameObject对象进行销毁
            if (isObtainedObejctID && !IsDestroyed && Invoker.Default != null)
            {
                Invoker.Default.Destroy(objectID);
            }

            GC.SuppressFinalize(this);
        }

        public static void DisposeAll()
        {
            if (referenceInstances == null)
                return;

            TsBehaviour instance;
            foreach (var weak in referenceInstances)
            {
                if (weak.TryGetTarget(out instance))
                {
                    instance.Dispose(false);
                    instance.referenceSelf = null;
                }
            }
            referenceInstances = null;
        }
    }
    public class ModuleInfo
    {
        public string className { get; set; }
        public string moduleName { get; set; }
        public string modulePath { get; set; }
        public int line { get; set; }
        public int column { get; set; }
        public string stack { get; set; }
    }
}


