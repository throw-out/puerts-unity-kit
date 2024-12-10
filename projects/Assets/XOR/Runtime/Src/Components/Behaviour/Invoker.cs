using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XOR.Behaviour
{
    public interface IInvoker
    {
        void Invoke(int instanceID, Args.Logic method);
        void Invoke(int instanceID, Args.Application method);
        void Invoke(int instanceID, Args.ApplicationBoolean method, bool data);
        void Invoke(int instanceID, Args.Renderer method);
        void Invoke(int instanceID, Args.Mouse method);
        void Invoke(int instanceID, Args.Edit method);
        void Invoke(int instanceID, Args.BaseEvents method, UnityEngine.EventSystems.BaseEventData data);
        void Invoke(int instanceID, Args.PointerEvents method, UnityEngine.EventSystems.PointerEventData data);
        void Invoke(int instanceID, Args.PhysicsCollider method, UnityEngine.Collider data);
        void Invoke(int instanceID, Args.PhysicsCollider2D method, UnityEngine.Collider2D data);
        void Invoke(int instanceID, Args.PhysicsCollision method, UnityEngine.Collision data);
        void Invoke(int instanceID, Args.PhysicsCollision2D method, UnityEngine.Collision2D data);

        void Destroy(int instanceID);
    }
    public class Invoker : IInvoker
    {
        public Action<int, Args.Logic> logic;
        public Action<int, Args.Application> application;
        public Action<int, Args.ApplicationBoolean, bool> application2;
        public Action<int, Args.Renderer> renderer;
        public Action<int, Args.Edit> edit;
        public Action<int, Args.Mouse> mouse;
        public Action<int, Args.BaseEvents, BaseEventData> baseEvents;
        public Action<int, Args.PointerEvents, PointerEventData> pointerEvents;
        public Action<int, Args.PhysicsCollider, Collider> collider;
        public Action<int, Args.PhysicsCollider2D, Collider2D> collider2D;
        public Action<int, Args.PhysicsCollision, Collision> collision;
        public Action<int, Args.PhysicsCollision2D, Collision2D> collision2D;
        public Action<int> destroy;

        public void Invoke(int instanceID, Args.Logic method)
        {
            logic?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.Application method)
        {
            application?.Invoke(instanceID, method);
        }
        public void Invoke(int instanceID, Args.ApplicationBoolean method, bool data)
        {
            application2?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.Edit method)
        {
            edit?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.Renderer method)
        {
            renderer?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.Mouse method)
        {
            mouse?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.BaseEvents method, BaseEventData data)
        {
            baseEvents?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.PointerEvents method, PointerEventData data)
        {
            pointerEvents?.Invoke(instanceID, method, data);
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

        public void Destroy(int instanceID)
        {
            destroy?.Invoke(instanceID);
        }

        public static Invoker Default { get; set; }

        static Invoker()
        {
            Register();
        }
        public static void Register()
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
        public virtual IInvoker Invoker { get; internal set; }
        public int ObjectID { get; internal set; }
    }


    [Args(typeof(Args.Logic))]
    public abstract class Logic : Behaviour<Action<Args.Logic>>
    {
        protected void Invoke(Args.Logic method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(ObjectID, method);
        }
    }

    [Args(typeof(Args.Application))]
    public abstract class Application : Behaviour<Action<Args.Application>>
    {
        protected void Invoke(Args.Application method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(ObjectID, method);
        }
    }

    [Args(typeof(Args.ApplicationBoolean))]
    public abstract class ApplicationBoolean : Behaviour<Action<Args.ApplicationBoolean, bool>>
    {
        protected void Invoke(Args.ApplicationBoolean method, bool data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(ObjectID, method, data);
        }
    }

    [Args(typeof(Args.Edit))]
    public abstract class Edit : Behaviour<Action<Args.Edit>>
    {
        protected void Invoke(Args.Edit method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(ObjectID, method);
        }
    }

    [Args(typeof(Args.Renderer))]
    public abstract class Renderer : Behaviour<Action<Args.Renderer>>
    {
        protected void Invoke(Args.Renderer method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(ObjectID, method);
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
                Invoker.Invoke(ObjectID, method);
        }
    }

    [Args(typeof(Args.BaseEvents))]
    public abstract class BaseEvents : Behaviour<Action<Args.BaseEvents, UnityEngine.EventSystems.BaseEventData>>
    {
        protected void Invoke(Args.BaseEvents method, UnityEngine.EventSystems.BaseEventData data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(ObjectID, method, data);
        }
    }

    [Args(typeof(Args.PointerEvents))]
    public abstract class PointerEvents : Behaviour<Action<Args.PointerEvents, UnityEngine.EventSystems.PointerEventData>>
    {
        protected void Invoke(Args.PointerEvents method, UnityEngine.EventSystems.PointerEventData data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(ObjectID, method, data);
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
                Invoker.Invoke(ObjectID, method, data);
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
                Invoker.Invoke(ObjectID, method, data);
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
                Invoker.Invoke(ObjectID, method, data);
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
                Invoker.Invoke(ObjectID, method, data);
        }
    }
}