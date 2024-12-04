using System;
using System.Collections.Generic;
using UnityEngine;

namespace XOR.Behaviour
{
    public static class Factory
    {
        private static readonly Dictionary<Enum, Type> behaviours = new Dictionary<Enum, Type>();

        public static void Register<T>(Args.Behaviour method)
            where T : Behaviour
        {
            behaviours[method] = typeof(T);
        }
        public static void Register<T>(Args.BehaviourBoolean method)
            where T : BehaviourBoolean
        {
            behaviours[method] = typeof(T);
        }
        public static void Register<T>(Args.Gizmos method)
            where T : Gizmos
        {
            behaviours[method] = typeof(T);
        }
        public static void Register<T>(Args.Mouse method)
            where T : Mouse
        {
            behaviours[method] = typeof(T);
        }
        public static void Register<T>(Args.EventSystemsPointerEventData method)
            where T : EventSystemsPointerEventData
        {
            behaviours[method] = typeof(T);
        }
        public static void Register<T>(Args.PhysicsCollider method)
            where T : PhysicsCollider
        {
            behaviours[method] = typeof(T);
        }
        public static void Register<T>(Args.PhysicsCollider2D method)
            where T : PhysicsCollider2D
        {
            behaviours[method] = typeof(T);
        }
        public static void Register<T>(Args.PhysicsCollision method)
            where T : PhysicsCollision
        {
            behaviours[method] = typeof(T);
        }
        public static void Register<T>(Args.PhysicsCollision2D method)
            where T : PhysicsCollision2D
        {
            behaviours[method] = typeof(T);
        }

        public static void ClearRegister()
        {
            behaviours.Clear();
        }
        public static bool HasRegister(Enum method)
        {
            return behaviours.ContainsKey(method);
        }

        public static Behaviour Create(GameObject go, Args.Behaviour method, Action<Args.Behaviour> callback, IInvoker invoker)
        {
            var component = CreateComponent<Behaviour>(go, method);
            if (component == null)
                return null;
            component.Callback = callback;
            component.Invoker = invoker;
            return component;
        }
        public static BehaviourBoolean Create(GameObject go, Args.BehaviourBoolean method, Action<Args.BehaviourBoolean, bool> callback, IInvoker invoker)
        {
            var component = CreateComponent<BehaviourBoolean>(go, method);
            if (component == null)
                return null;
            component.Callback = callback;
            component.Invoker = invoker;
            return component;
        }
        public static Gizmos Create(GameObject go, Args.Gizmos method, Action<Args.Gizmos> callback, IInvoker invoker)
        {
            var component = CreateComponent<Gizmos>(go, method);
            if (component == null)
                return null;
            component.Callback = callback;
            component.Invoker = invoker;
            return component;
        }
        public static Mouse Create(GameObject go, Args.Mouse method, Action<Args.Mouse> callback, IInvoker invoker)
        {
            var component = CreateComponent<Mouse>(go, method);
            if (component == null)
                return null;
            component.Callback = callback;
            component.Invoker = invoker;
            return component;
        }
        public static EventSystemsPointerEventData Create(GameObject go, Args.EventSystemsPointerEventData method, Action<Args.EventSystemsPointerEventData, UnityEngine.EventSystems.PointerEventData> callback, IInvoker invoker)
        {
            var component = CreateComponent<EventSystemsPointerEventData>(go, method);
            if (component == null)
                return null;
            component.Callback = callback;
            component.Invoker = invoker;
            return component;
        }
        public static PhysicsCollider Create(GameObject go, Args.PhysicsCollider method, Action<Args.PhysicsCollider, UnityEngine.Collider> callback, IInvoker invoker)
        {
            var component = CreateComponent<PhysicsCollider>(go, method);
            if (component == null)
                return null;
            component.Callback = callback;
            component.Invoker = invoker;
            return component;
        }
        public static PhysicsCollider2D Create(GameObject go, Args.PhysicsCollider2D method, Action<Args.PhysicsCollider2D, UnityEngine.Collider2D> callback, IInvoker invoker)
        {
            var component = CreateComponent<PhysicsCollider2D>(go, method);
            if (component == null)
                return null;
            component.Callback = callback;
            component.Invoker = invoker;
            return component;
        }
        public static PhysicsCollision Create(GameObject go, Args.PhysicsCollision method, Action<Args.PhysicsCollision, UnityEngine.Collision> callback, IInvoker invoker)
        {
            var component = CreateComponent<PhysicsCollision>(go, method);
            if (component == null)
                return null;
            component.Callback = callback;
            component.Invoker = invoker;
            return component;
        }
        public static PhysicsCollision2D Create(GameObject go, Args.PhysicsCollision2D method, Action<Args.PhysicsCollision2D, UnityEngine.Collision2D> callback, IInvoker invoker)
        {
            var component = CreateComponent<PhysicsCollision2D>(go, method);
            if (component == null)
                return null;
            component.Callback = callback;
            component.Invoker = invoker;
            return component;
        }

        public static T CreateComponent<T>(GameObject go, Enum method)
            where T : Component
        {
            if (!behaviours.TryGetValue(method, out var type))
            {
                return null;
            }
            return go.AddComponent(type) as T;
        }
    }
}