using System;
using System.Collections.Generic;
using UnityEngine;

namespace XOR.Behaviour
{
    public static class Factory
    {
        public static readonly Behaviours<Args.Mono, Action<Args.Mono>, Mono> Mono
            = new Behaviours<Args.Mono, Action<Args.Mono>, Mono>(EnumConvert.MonoToUInt32);
        public static readonly Behaviours<Args.MonoBoolean, Action<Args.MonoBoolean, bool>, MonoBoolean> MonoBoolean
            = new Behaviours<Args.MonoBoolean, Action<Args.MonoBoolean, bool>, MonoBoolean>(EnumConvert.MonoBooleanToUInt32);
        public static readonly Behaviours<Args.Mouse, Action<Args.Mouse>, Mouse> Mouse
            = new Behaviours<Args.Mouse, Action<Args.Mouse>, Mouse>(EnumConvert.MouseToUInt32);
        public static readonly Behaviours<Args.Gizmos, Action<Args.Gizmos>, Gizmos> Gizmos
            = new Behaviours<Args.Gizmos, Action<Args.Gizmos>, Gizmos>(EnumConvert.GizmosToUInt32);
        public static readonly Behaviours<Args.EventSystems, Action<Args.EventSystems, UnityEngine.EventSystems.PointerEventData>, EventSystems> EventSystems
            = new Behaviours<Args.EventSystems, Action<Args.EventSystems, UnityEngine.EventSystems.PointerEventData>, EventSystems>(EnumConvert.EventSystemsToUInt32);
        public static readonly Behaviours<Args.PhysicsCollider, Action<Args.PhysicsCollider, Collider>, PhysicsCollider> PhysicsCollider
            = new Behaviours<Args.PhysicsCollider, Action<Args.PhysicsCollider, Collider>, PhysicsCollider>(EnumConvert.PhysicsColliderToUInt32);
        public static readonly Behaviours<Args.PhysicsCollider2D, Action<Args.PhysicsCollider2D, Collider2D>, PhysicsCollider2D> PhysicsCollider2D
            = new Behaviours<Args.PhysicsCollider2D, Action<Args.PhysicsCollider2D, Collider2D>, PhysicsCollider2D>(EnumConvert.PhysicsCollider2DToUInt32);
        public static readonly Behaviours<Args.PhysicsCollision, Action<Args.PhysicsCollision, Collision>, PhysicsCollision> PhysicsCollision
            = new Behaviours<Args.PhysicsCollision, Action<Args.PhysicsCollision, Collision>, PhysicsCollision>(EnumConvert.PhysicsCollisionToUInt32);
        public static readonly Behaviours<Args.PhysicsCollision2D, Action<Args.PhysicsCollision2D, Collision2D>, PhysicsCollision2D> PhysicsCollision2D
            = new Behaviours<Args.PhysicsCollision2D, Action<Args.PhysicsCollision2D, Collision2D>, PhysicsCollision2D>(EnumConvert.PhysicsCollision2DToUInt32);


        public static void Register<T>(Args.Mono method)
            where T : Mono
        {
            Mono.Add<T>(method);
        }
        public static void Register<T>(Args.MonoBoolean method)
            where T : MonoBoolean
        {
            MonoBoolean.Add<T>(method);
        }
        public static void Register<T>(Args.Gizmos method)
            where T : Gizmos
        {
            Gizmos.Add<T>(method);
        }
        public static void Register<T>(Args.Mouse method)
            where T : Mouse
        {
            Mouse.Add<T>(method);
        }
        public static void Register<T>(Args.EventSystems method)
            where T : EventSystems
        {
            EventSystems.Add<T>(method);
        }
        public static void Register<T>(Args.PhysicsCollider method)
            where T : PhysicsCollider
        {
            PhysicsCollider.Add<T>(method);
        }
        public static void Register<T>(Args.PhysicsCollider2D method)
            where T : PhysicsCollider2D
        {
            PhysicsCollider2D.Add<T>(method);
        }
        public static void Register<T>(Args.PhysicsCollision method)
            where T : PhysicsCollision
        {
            PhysicsCollision.Add<T>(method);
        }
        public static void Register<T>(Args.PhysicsCollision2D method)
            where T : PhysicsCollision2D
        {
            PhysicsCollision2D.Add<T>(method);
        }

        public static void Clear()
        {
            Mono.Clear();
            MonoBoolean.Clear();
            Mouse.Clear();
            Gizmos.Clear();
            EventSystems.Clear();
            PhysicsCollider.Clear();
            PhysicsCollider2D.Clear();
            PhysicsCollision.Clear();
            PhysicsCollision2D.Clear();
        }

        public static bool Contains<T>(T value)
            where T : Enum
        {
            if (value is Args.Mono v1)
            {
                return Mono.Contains(v1);
            }
            if (value is Args.MonoBoolean v2)
            {
                return MonoBoolean.Contains(v2);
            }
            if (value is Args.Mouse v3)
            {
                return Mouse.Contains(v3);
            }
            if (value is Args.Gizmos v4)
            {
                return Gizmos.Contains(v4);
            }
            if (value is Args.EventSystems v5)
            {
                return EventSystems.Contains(v5);
            }
            if (value is Args.PhysicsCollider v6)
            {
                return PhysicsCollider.Contains(v6);
            }
            if (value is Args.PhysicsCollider2D v7)
            {
                return PhysicsCollider2D.Contains(v7);
            }
            if (value is Args.PhysicsCollision v8)
            {
                return PhysicsCollision.Contains(v8);
            }
            if (value is Args.PhysicsCollision2D v9)
            {
                return PhysicsCollision2D.Contains(v9);
            }
            return false;
        }

        public class Behaviours<T, TDelegate, TComponent>
            where T : Enum
            where TDelegate : Delegate
            where TComponent : Behaviour<TDelegate>
        {
            private readonly Dictionary<uint, Type> dict = new Dictionary<uint, Type>();
            private readonly EnumUnderlyingGetter<T> enumGetter = null;
            public IEnumerable<uint> Methods => dict.Keys;

            public Behaviours(EnumUnderlyingGetter<T> enumGetter = null)
            {
                this.enumGetter = enumGetter;
            }

            public void Add<TComponentImpl>(T method)
                where TComponentImpl : TComponent
            {
                dict[GetEnumUInt32(method)] = typeof(TComponentImpl);
            }
            public void Clear()
            {
                dict.Clear();
            }
            public bool Contains(T method)
            {
                return dict.ContainsKey(GetEnumUInt32(method));
            }
            public TComponent Create(GameObject go, T method, TDelegate callback, IInvoker invoker)
            {
                return Create(go, GetEnumUInt32(method), callback, invoker);
            }
            public TComponent Create(GameObject go, uint method, TDelegate callback, IInvoker invoker)
            {
                if (!dict.TryGetValue(method, out var type))
                {
                    return null;
                }
                var component = go.AddComponent(type) as TComponent;
                component.Callback = callback;
                component.Invoker = invoker;
                return component;
            }

            public uint GetEnumUInt32(T method)
            {
                if (enumGetter != null && enumGetter(method, out uint v))
                {
                    return v;
                }
                return EnumConvert.ToUInt32(method);
            }
        }

        public delegate bool EnumUnderlyingGetter<T>(T @enum, out uint v) where T : Enum;
        static class EnumConvert
        {
            public static bool MonoToUInt32<T>(T @enum, out uint v)
                where T : Enum
            {
                if (@enum is Args.Mono v1)
                {
                    v = (uint)v1;
                    return true;
                }
                else
                {
                    v = default;
                    return false;
                }
            }
            public static bool MonoBooleanToUInt32<T>(T @enum, out uint v)
                where T : Enum
            {
                if (@enum is Args.MonoBoolean v1)
                {
                    v = (uint)v1;
                    return true;
                }
                else
                {
                    v = default;
                    return false;
                }
            }
            public static bool MouseToUInt32<T>(T @enum, out uint v)
                where T : Enum
            {
                if (@enum is Args.Mouse v1)
                {
                    v = (uint)v1;
                    return true;
                }
                else
                {
                    v = default;
                    return false;
                }
            }
            public static bool GizmosToUInt32<T>(T @enum, out uint v)
                where T : Enum
            {
                if (@enum is Args.Gizmos v1)
                {
                    v = (uint)v1;
                    return true;
                }
                else
                {
                    v = default;
                    return false;
                }
            }
            public static bool EventSystemsToUInt32<T>(T @enum, out uint v)
                where T : Enum
            {
                if (@enum is Args.EventSystems v1)
                {
                    v = (uint)v1;
                    return true;
                }
                else
                {
                    v = default;
                    return false;
                }
            }
            public static bool PhysicsColliderToUInt32<T>(T @enum, out uint v)
                where T : Enum
            {
                if (@enum is Args.PhysicsCollider v1)
                {
                    v = (uint)v1;
                    return true;
                }
                else
                {
                    v = default;
                    return false;
                }
            }
            public static bool PhysicsCollider2DToUInt32<T>(T @enum, out uint v)
                where T : Enum
            {
                if (@enum is Args.PhysicsCollider2D v1)
                {
                    v = (uint)v1;
                    return true;
                }
                else
                {
                    v = default;
                    return false;
                }
            }
            public static bool PhysicsCollisionToUInt32<T>(T @enum, out uint v)
                where T : Enum
            {
                if (@enum is Args.PhysicsCollision v1)
                {
                    v = (uint)v1;
                    return true;
                }
                else
                {
                    v = default;
                    return false;
                }
            }
            public static bool PhysicsCollision2DToUInt32<T>(T @enum, out uint v)
                where T : Enum
            {
                if (@enum is Args.PhysicsCollision2D v1)
                {
                    v = (uint)v1;
                    return true;
                }
                else
                {
                    v = default;
                    return false;
                }
            }

            public static uint ToUInt32<T>(T @enum)
                where T : Enum
            {
                uint value = default;
                if (!(
                    MonoToUInt32(@enum, out value) ||
                    MonoBooleanToUInt32(@enum, out value) ||
                    MouseToUInt32(@enum, out value) ||
                    GizmosToUInt32(@enum, out value) ||
                    EventSystemsToUInt32(@enum, out value) ||
                    PhysicsColliderToUInt32(@enum, out value) ||
                    PhysicsCollider2DToUInt32(@enum, out value) ||
                    PhysicsCollisionToUInt32(@enum, out value) ||
                    PhysicsCollision2DToUInt32(@enum, out value)
                ))
                {
                    value = Convert.ToUInt32(@enum);
                }
                return value;
            }
        }
    }
}