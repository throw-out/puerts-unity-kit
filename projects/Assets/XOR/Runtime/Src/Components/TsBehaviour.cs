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
        private Behaviour.Args.Logic @base;
        private Action<Behaviour.Args.Logic> lifecycle;
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
            if ((@base & Behaviour.Args.Logic.Awake) > 0)
            {
                @base ^= Behaviour.Args.Logic.Awake;
                Invoke(Behaviour.Args.Logic.Awake, lifecycle);
            }
        }
        protected virtual void Start()
        {
            IsStarted = true;
            if ((@base & Behaviour.Args.Logic.Start) > 0)
            {
                @base ^= Behaviour.Args.Logic.Start;
                Invoke(Behaviour.Args.Logic.Start, lifecycle);
            }
        }
        protected virtual void OnEnable()
        {
            IsEnable = true;
            if ((@base & Behaviour.Args.Logic.OnEnable) > 0)
            {
                Invoke(Behaviour.Args.Logic.OnEnable, lifecycle);
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
            if ((@base & Behaviour.Args.Logic.OnDisable) > 0)
            {
                Invoke(Behaviour.Args.Logic.OnDisable, lifecycle);
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
            if ((@base & Behaviour.Args.Logic.OnDestroy) > 0)
            {
                @base ^= Behaviour.Args.Logic.OnDestroy;
                Invoke(Behaviour.Args.Logic.OnDestroy, lifecycle);
            }
            Dispose(true);
        }

        public void CreateLogic(Behaviour.Args.Logic methods, Action<Behaviour.Args.Logic> callback)
        {
            Behaviour.Args.Logic @base = default;
            foreach (var v in Behaviour.Args.Extensions.GetLogicBase())
            {
                if ((methods & v) > 0)
                {
                    methods ^= v;
                    @base |= v;
                }
            }
            if (@base > 0)
            {
                GetObjectID();
                this.@base |= @base;
                if (callback != null)
                    lifecycle += callback;
                if ((@base & Behaviour.Args.Logic.Awake) > 0 && !IsDestroyed && IsActivated)
                    Invoke(Behaviour.Args.Logic.Awake, callback);
                if ((@base & Behaviour.Args.Logic.Start) > 0 && !IsDestroyed && IsStarted)
                    Invoke(Behaviour.Args.Logic.Start, callback);
                if ((@base & Behaviour.Args.Logic.OnDestroy) > 0 && IsDestroyed)
                    Invoke(Behaviour.Args.Logic.OnDestroy, callback);
                if ((@base & Behaviour.Args.Logic.OnEnable) > 0 && !IsDestroyed && IsEnable)
                    Invoke(Behaviour.Args.Logic.OnEnable, callback);
                if ((@base & Behaviour.Args.Logic.OnDisable) > 0 && !IsDestroyed && IsActivated && !IsEnable)
                    Invoke(Behaviour.Args.Logic.OnDisable, callback);
            }
            if (methods <= 0)
                return;
            Create(Factory.Logic, methods, callback);
        }
        public void CreateApplication(Behaviour.Args.Application methods, Action<Behaviour.Args.Application> callback)
        {
            Create(Factory.Application, methods, callback);
        }
        public void CreateApplicationBoolean(Behaviour.Args.ApplicationBoolean methods, Action<Behaviour.Args.ApplicationBoolean, bool> callback)
        {
            Create(Factory.ApplicationBoolean, methods, callback);
        }
        public void CreateRenderer(Behaviour.Args.Renderer methods, Action<Behaviour.Args.Renderer> callback)
        {
            Create(Factory.Renderer, methods, callback);
        }
        public void CreateMouse(Behaviour.Args.Mouse methods, Action<Behaviour.Args.Mouse> callback)
        {
            Create(Factory.Mouse, methods, callback);
        }
        public void CreateEdit(Behaviour.Args.Edit methods, Action<Behaviour.Args.Edit> callback)
        {
            Create(Factory.Edit, methods, callback);
        }
        public void CreateBaseEvents(Behaviour.Args.BaseEvents methods, Action<Behaviour.Args.BaseEvents, BaseEventData> callback)
        {
            Create(Factory.BaseEvents, methods, callback);
        }
        public void CreatePointerEvents(Behaviour.Args.PointerEvents methods, Action<Behaviour.Args.PointerEvents, PointerEventData> callback)
        {
            Create(Factory.PointerEvents, methods, callback);
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
                    component.ObjectID = GetObjectID();
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
                    component.ObjectID = GetObjectID();
                    this.behaviours.Add(component);
                }
                value ^= method;
            }
        }
        private void Invoke(Behaviour.Args.Logic method, Action<Behaviour.Args.Logic> callback)
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


