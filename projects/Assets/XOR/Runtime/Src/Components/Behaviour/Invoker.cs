using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XOR.Behaviour
{
    public interface IInvoker
    {
        void Invoke(int instanceID, Args.Mono method);
        void Invoke(int instanceID, Args.MonoBoolean method, bool data);
        void Invoke(int instanceID, Args.Gizmos method);
        void Invoke(int instanceID, Args.Mouse method);
        void Invoke(int instanceID, Args.EventSystems method, UnityEngine.EventSystems.PointerEventData data);
        void Invoke(int instanceID, Args.PhysicsCollider method, UnityEngine.Collider data);
        void Invoke(int instanceID, Args.PhysicsCollider2D method, UnityEngine.Collider2D data);
        void Invoke(int instanceID, Args.PhysicsCollision method, UnityEngine.Collision data);
        void Invoke(int instanceID, Args.PhysicsCollision2D method, UnityEngine.Collision2D data);
    }
    public class Invoker : IInvoker
    {
        public Action<int, Args.Mono> mono;
        public Action<int, Args.MonoBoolean, bool> monoBoolean;
        public Action<int, Args.Gizmos> gizmos;
        public Action<int, Args.Mouse> mouse;
        public Action<int, Args.EventSystems, PointerEventData> eventSystems;
        public Action<int, Args.PhysicsCollider, Collider> collider;
        public Action<int, Args.PhysicsCollider2D, Collider2D> collider2D;
        public Action<int, Args.PhysicsCollision, Collision> collision;
        public Action<int, Args.PhysicsCollision2D, Collision2D> collision2D;

        public void Invoke(int instanceID, Args.Mono method)
        {
            mono?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.MonoBoolean method, bool data)
        {
            monoBoolean?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.Gizmos method)
        {
            gizmos?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.Mouse method)
        {
            mouse?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.EventSystems method, PointerEventData data)
        {
            eventSystems?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.PhysicsCollider method, Collider data)
        {
            collider?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.PhysicsCollider2D method, Collider2D data)
        {
            collider2D?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.PhysicsCollision method, Collision data)
        {
            collision?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.PhysicsCollision2D method, Collision2D data)
        {
            collision2D?.Invoke(instanceID, method, data);
        }



        private static Invoker @default;
        public static Invoker Default
        {
            get => @default;
            set
            {
                @default = value;
                Register();
            }
        }
        private static void Register()
        {
            XOR.Behaviour.Factory.Clear();

            XOR.Behaviour.Default.Register();
            //注册自定义Invoker
            var generteType = Type.GetType("XOR.Behaviour.BehaviourInvokerStaticWrap", false);
            if (generteType == null)
                return;
            var registerMethod = generteType.GetMethod("Register", BindingFlags.Static | BindingFlags.Public);
            if (registerMethod == null)
                return;
            try
            {
                registerMethod.Invoke(null, null);
            }
            catch (Exception e)
            {
                XOR.Logger.LogError(e);
            }
        }
    }
    public abstract class Behaviour : MonoBehaviour
    {
        public Behaviour()
        {
            this.Init();
        }
        ~Behaviour()
        {
            Release();
        }
        public virtual void Init()
        {
        }
        public virtual void Release()
        {
        }
        public void Dispose()
        {
            Release();
        }
    }
    public abstract class Behaviour<TDelegate> : Behaviour
        where TDelegate : Delegate
    {
        public virtual TDelegate Callback { get; set; }
        public virtual IInvoker Invoker { get; set; }
    }

    [Args(typeof(Args.Mono))]
    public abstract class Mono : Behaviour<Action<Args.Mono>>
    {
        protected void Invoke(Args.Mono method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method);
        }
    }

    [Args(typeof(Args.MonoBoolean))]
    public abstract class MonoBoolean : Behaviour<Action<Args.MonoBoolean, bool>>
    {
        protected void Invoke(Args.MonoBoolean method, bool data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.Gizmos))]
    public abstract class Gizmos : Behaviour<Action<Args.Gizmos>>
    {
        protected void Invoke(Args.Gizmos method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method);
        }
    }

    [Args(typeof(Args.Mouse))]
    public abstract class Mouse : Behaviour<Action<Args.Mouse>>
    {
        protected void Invoke(Args.Mouse method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method);
        }
    }

    [Args(typeof(Args.EventSystems))]
    public abstract class EventSystems : Behaviour<Action<Args.EventSystems, UnityEngine.EventSystems.PointerEventData>>
    {
        protected void Invoke(Args.EventSystems method, UnityEngine.EventSystems.PointerEventData data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.PhysicsCollider))]
    public abstract class PhysicsCollider : Behaviour<Action<Args.PhysicsCollider, UnityEngine.Collider>>
    {
        protected void Invoke(Args.PhysicsCollider method, UnityEngine.Collider data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.PhysicsCollider2D))]
    public abstract class PhysicsCollider2D : Behaviour<Action<Args.PhysicsCollider2D, UnityEngine.Collider2D>>
    {
        protected void Invoke(Args.PhysicsCollider2D method, UnityEngine.Collider2D data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.PhysicsCollision))]
    public abstract class PhysicsCollision : Behaviour<Action<Args.PhysicsCollision, UnityEngine.Collision>>
    {
        protected void Invoke(Args.PhysicsCollision method, UnityEngine.Collision data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.PhysicsCollision2D))]
    public abstract class PhysicsCollision2D : Behaviour<Action<Args.PhysicsCollision2D, UnityEngine.Collision2D>>
    {
        protected void Invoke(Args.PhysicsCollision2D method, UnityEngine.Collision2D data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }
}