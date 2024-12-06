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
        private int instanceID;
        private List<Behaviour.Behaviour> behaviours;
        private Action<Behaviour.Args.Mono> awakeCallback;       //Awake回调
        private Action<Behaviour.Args.Mono> startCallback;       //Start回调
        private Action<Behaviour.Args.Mono> destroyCallback;     //OnDestroy回调
        private Action<Behaviour.Args.Mono> enableCallback;      //OnEnable回调
        private Action<Behaviour.Args.Mono> disableCallback;     //OnDisable回调
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
        protected virtual void Awake()
        {
            instanceID = gameObject.GetInstanceID();
            IsActivated = true;
            awakeCallback?.Invoke(Behaviour.Args.Mono.Awake);
            awakeCallback = null;
        }
        protected virtual void Start()
        {
            IsStarted = true;
            startCallback?.Invoke(Behaviour.Args.Mono.Start);
            startCallback = null;
        }
        protected virtual void OnEnable()
        {
            IsEnable = true;
            enableCallback?.Invoke(Behaviour.Args.Mono.OnEnable);
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
            disableCallback?.Invoke(Behaviour.Args.Mono.OnDisable);
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
            destroyCallback?.Invoke(Behaviour.Args.Mono.OnDestroy);
            destroyCallback = null;
            Dispose(true);
        }

        public void CreateMono(Behaviour.Args.Mono methods, Action<Behaviour.Args.Mono> callback)
        {
            if ((methods & Behaviour.Args.Mono.Awake) > 0)
            {
                methods ^= Behaviour.Args.Mono.Awake;
                if (IsActivated && !IsDestroyed)
                    Invoke(Behaviour.Args.Mono.Awake, callback);
                else
                    awakeCallback += callback;
            }
            if ((methods & Behaviour.Args.Mono.Start) > 0)
            {
                methods ^= Behaviour.Args.Mono.Start;
                if (IsStarted && !IsDestroyed)
                    Invoke(Behaviour.Args.Mono.Start, callback);
                else
                    startCallback += callback;
            }
            if ((methods & Behaviour.Args.Mono.OnDestroy) > 0)
            {
                methods ^= Behaviour.Args.Mono.OnDestroy;
                if (IsDestroyed)
                    Invoke(Behaviour.Args.Mono.OnDestroy, callback);
                else
                    destroyCallback += callback;
            }
            if ((methods & Behaviour.Args.Mono.OnEnable) > 0)
            {
                methods ^= Behaviour.Args.Mono.OnEnable;
                if (IsEnable && !IsDestroyed)
                    Invoke(Behaviour.Args.Mono.OnEnable, callback);
                else
                    enableCallback += callback;
            }
            if ((methods & Behaviour.Args.Mono.OnDisable) > 0)
            {
                methods ^= Behaviour.Args.Mono.OnDisable;
                if (IsActivated && !IsEnable && !IsDestroyed)
                    Invoke(Behaviour.Args.Mono.OnDisable, callback);
                else
                    disableCallback += callback;
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
            if (behaviours.Contains(methods))
            {
                var component = behaviours.Create(gameObject, methods, callback, Invoker.Default);
                this.behaviours.Add(component);
                return;
            }
            var value = behaviours.GetEnumUInt32(methods);
            foreach (var method in behaviours.Methods)
            {
                if (value <= 0)
                    break;
                if ((value & method) <= 0)
                    continue;
                behaviours.Create(gameObject, method, callback, Invoker.Default);
                value ^= method;
            }
        }
        private void Invoke(Behaviour.Args.Mono method, Action<Behaviour.Args.Mono> callback)
        {
            if (callback != null)
            {
                callback(Behaviour.Args.Mono.Awake);
            }
            else if (Invoker.Default != null && IsActivated)
            {
                Invoker.Default.Invoke(instanceID, Behaviour.Args.Mono.Awake);
            }
        }


        public virtual void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool destroy)
        {
            this.awakeCallback = null;
            this.startCallback = null;
            this.destroyCallback = null;
            this.enableCallback = null;
            this.disableCallback = null;

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


