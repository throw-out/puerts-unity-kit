using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XOR.Behaviour
{
    public interface IInvoker
    {
        void Invoke(int instanceID, Args.Behaviour method);
        void Invoke(int instanceID, Args.BehaviourBoolean method, bool data);
        void Invoke(int instanceID, Args.Gizmos method);
        void Invoke(int instanceID, Args.Mouse method);
        void Invoke(int instanceID, Args.EventSystemsPointerEventData method, UnityEngine.EventSystems.PointerEventData data);
        void Invoke(int instanceID, Args.PhysicsCollider method, UnityEngine.Collider data);
        void Invoke(int instanceID, Args.PhysicsCollider2D method, UnityEngine.Collider2D data);
        void Invoke(int instanceID, Args.PhysicsCollision method, UnityEngine.Collision data);
        void Invoke(int instanceID, Args.PhysicsCollision2D method, UnityEngine.Collision2D data);
    }
    public class Invoker : IInvoker
    {
        public Action<int, Args.Behaviour> callback1;
        public Action<int, Args.BehaviourBoolean, bool> callback2;
        public Action<int, Args.Gizmos> callback3;
        public Action<int, Args.Mouse> callback4;
        public Action<int, Args.EventSystemsPointerEventData, PointerEventData> callback5;
        public Action<int, Args.PhysicsCollider, Collider> callback6;
        public Action<int, Args.PhysicsCollider2D, Collider2D> callback7;
        public Action<int, Args.PhysicsCollision, Collision> callback8;
        public Action<int, Args.PhysicsCollision2D, Collision2D> callback9;

        public void Invoke(int instanceID, Args.Behaviour method)
        {
            callback1?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.BehaviourBoolean method, bool data)
        {
            callback2?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.Gizmos method)
        {
            callback3?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.Mouse method)
        {
            callback4?.Invoke(instanceID, method);
        }

        public void Invoke(int instanceID, Args.EventSystemsPointerEventData method, PointerEventData data)
        {
            callback5?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.PhysicsCollider method, Collider data)
        {
            callback6?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.PhysicsCollider2D method, Collider2D data)
        {
            callback7?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.PhysicsCollision method, Collision data)
        {
            callback8?.Invoke(instanceID, method, data);
        }

        public void Invoke(int instanceID, Args.PhysicsCollision2D method, Collision2D data)
        {
            callback9?.Invoke(instanceID, method, data);
        }

        public static Invoker Default { get; set; }
    }

    [Args(typeof(Args.Behaviour))]
    public abstract class Behaviour : MonoBehaviour
    {
        public virtual Action<Args.Behaviour> Callback { get; set; }
        public IInvoker Invoker { get; set; }
        protected void Invoke(Args.Behaviour method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method);
        }
    }

    [Args(typeof(Args.BehaviourBoolean))]
    public abstract class BehaviourBoolean : MonoBehaviour
    {
        public virtual Action<Args.BehaviourBoolean, bool> Callback { get; set; }
        public IInvoker Invoker { get; set; }
        protected void Invoke(Args.BehaviourBoolean method, bool data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.Gizmos))]
    public abstract class Gizmos : MonoBehaviour
    {
        public virtual Action<Args.Gizmos> Callback { get; set; }
        public IInvoker Invoker { get; set; }
        protected void Invoke(Args.Gizmos method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method);
        }
    }

    [Args(typeof(Args.Mouse))]
    public abstract class Mouse : MonoBehaviour
    {
        public virtual Action<Args.Mouse> Callback { get; set; }
        public IInvoker Invoker { get; set; }
        protected void Invoke(Args.Mouse method)
        {
            if (Callback != null)
                Callback(method);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method);
        }
    }

    [Args(typeof(Args.EventSystemsPointerEventData))]
    public abstract class EventSystemsPointerEventData : MonoBehaviour
    {
        public virtual Action<Args.EventSystemsPointerEventData, UnityEngine.EventSystems.PointerEventData> Callback { get; set; }
        public IInvoker Invoker { get; set; }
        protected void Invoke(Args.EventSystemsPointerEventData method, UnityEngine.EventSystems.PointerEventData data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.PhysicsCollider))]
    public abstract class PhysicsCollider : MonoBehaviour
    {
        public virtual Action<Args.PhysicsCollider, UnityEngine.Collider> Callback { get; set; }
        public IInvoker Invoker { get; set; }
        protected void Invoke(Args.PhysicsCollider method, UnityEngine.Collider data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.PhysicsCollider2D))]
    public abstract class PhysicsCollider2D : MonoBehaviour
    {
        public virtual Action<Args.PhysicsCollider2D, UnityEngine.Collider2D> Callback { get; set; }
        public IInvoker Invoker { get; set; }
        protected void Invoke(Args.PhysicsCollider2D method, UnityEngine.Collider2D data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.PhysicsCollision))]
    public abstract class PhysicsCollision : MonoBehaviour
    {
        public virtual Action<Args.PhysicsCollision, UnityEngine.Collision> Callback { get; set; }
        public IInvoker Invoker { get; set; }
        protected void Invoke(Args.PhysicsCollision method, UnityEngine.Collision data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }

    [Args(typeof(Args.PhysicsCollision2D))]
    public abstract class PhysicsCollision2D : MonoBehaviour
    {
        public virtual Action<Args.PhysicsCollision2D, UnityEngine.Collision2D> Callback { get; set; }
        public IInvoker Invoker { get; set; }
        protected void Invoke(Args.PhysicsCollision2D method, UnityEngine.Collision2D data)
        {
            if (Callback != null)
                Callback(method, data);
            else if (Invoker != null)
                Invoker.Invoke(gameObject.GetInstanceID(), method, data);
        }
    }
}