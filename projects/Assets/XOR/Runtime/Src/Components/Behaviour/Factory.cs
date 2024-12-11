using System;
using System.Collections.Generic;
using UnityEngine;

namespace XOR.Behaviour
{
    public static class Factory
    {
        public static readonly Behaviours<Args.Logic, Action<Args.Logic>, Logic> Logic
            = new Behaviours<Args.Logic, Action<Args.Logic>, Logic>(EnumConvert.LogicToUInt32);
        public static readonly Behaviours<Args.Application, Action<Args.Application>, Application> Application
            = new Behaviours<Args.Application, Action<Args.Application>, Application>(EnumConvert.ApplicationToUInt32);
        public static readonly Behaviours<Args.ApplicationBoolean, Action<Args.ApplicationBoolean, bool>, ApplicationBoolean> ApplicationBoolean
            = new Behaviours<Args.ApplicationBoolean, Action<Args.ApplicationBoolean, bool>, ApplicationBoolean>(EnumConvert.ApplicationBooleanToUInt32);
        public static readonly Behaviours<Args.Renderer, Action<Args.Renderer>, Renderer> Renderer
            = new Behaviours<Args.Renderer, Action<Args.Renderer>, Renderer>(EnumConvert.RendererToUInt32);
        public static readonly Behaviours<Args.Mouse, Action<Args.Mouse>, Mouse> Mouse
            = new Behaviours<Args.Mouse, Action<Args.Mouse>, Mouse>(EnumConvert.MouseToUInt32);
        public static readonly Behaviours<Args.Edit, Action<Args.Edit>, Edit> Edit
            = new Behaviours<Args.Edit, Action<Args.Edit>, Edit>(EnumConvert.EditToUInt32);
        public static readonly Behaviours<Args.BaseEvents, Action<Args.BaseEvents, UnityEngine.EventSystems.BaseEventData>, BaseEvents> BaseEvents
            = new Behaviours<Args.BaseEvents, Action<Args.BaseEvents, UnityEngine.EventSystems.BaseEventData>, BaseEvents>(EnumConvert.BaseEventsToUInt32);
        public static readonly Behaviours<Args.PointerEvents, Action<Args.PointerEvents, UnityEngine.EventSystems.PointerEventData>, PointerEvents> PointerEvents
            = new Behaviours<Args.PointerEvents, Action<Args.PointerEvents, UnityEngine.EventSystems.PointerEventData>, PointerEvents>(EnumConvert.PointerEventsToUInt32);
        public static readonly Behaviours<Args.PhysicsCollider, Action<Args.PhysicsCollider, Collider>, PhysicsCollider> PhysicsCollider
            = new Behaviours<Args.PhysicsCollider, Action<Args.PhysicsCollider, Collider>, PhysicsCollider>(EnumConvert.PhysicsColliderToUInt32);
        public static readonly Behaviours<Args.PhysicsCollider2D, Action<Args.PhysicsCollider2D, Collider2D>, PhysicsCollider2D> PhysicsCollider2D
            = new Behaviours<Args.PhysicsCollider2D, Action<Args.PhysicsCollider2D, Collider2D>, PhysicsCollider2D>(EnumConvert.PhysicsCollider2DToUInt32);
        public static readonly Behaviours<Args.PhysicsCollision, Action<Args.PhysicsCollision, Collision>, PhysicsCollision> PhysicsCollision
            = new Behaviours<Args.PhysicsCollision, Action<Args.PhysicsCollision, Collision>, PhysicsCollision>(EnumConvert.PhysicsCollisionToUInt32);
        public static readonly Behaviours<Args.PhysicsCollision2D, Action<Args.PhysicsCollision2D, Collision2D>, PhysicsCollision2D> PhysicsCollision2D
            = new Behaviours<Args.PhysicsCollision2D, Action<Args.PhysicsCollision2D, Collision2D>, PhysicsCollision2D>(EnumConvert.PhysicsCollision2DToUInt32);


        public static void Register<T>(Args.Logic method)
            where T : Logic
        {
            Logic.Add<T>(method);
        }
        public static void Register<T>(Args.Application method)
            where T : Application
        {
            Application.Add<T>(method);
        }
        public static void Register<T>(Args.ApplicationBoolean method)
            where T : ApplicationBoolean
        {
            ApplicationBoolean.Add<T>(method);
        }
        public static void Register<T>(Args.Renderer method)
            where T : Renderer
        {
            Renderer.Add<T>(method);
        }
        public static void Register<T>(Args.Edit method)
            where T : Edit
        {
            Edit.Add<T>(method);
        }
        public static void Register<T>(Args.Mouse method)
            where T : Mouse
        {
            Mouse.Add<T>(method);
        }
        public static void Register<T>(Args.BaseEvents method)
            where T : BaseEvents
        {
            BaseEvents.Add<T>(method);
        }
        public static void Register<T>(Args.PointerEvents method)
            where T : PointerEvents
        {
            PointerEvents.Add<T>(method);
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
            Logic.Clear();
            Application.Clear();
            ApplicationBoolean.Clear();
            Renderer.Clear();
            Mouse.Clear();
            Edit.Clear();
            BaseEvents.Clear();
            PointerEvents.Clear();
            PhysicsCollider.Clear();
            PhysicsCollider2D.Clear();
            PhysicsCollision.Clear();
            PhysicsCollision2D.Clear();
        }

        public static bool Contains<T>(T value)
            where T : Enum
        {
            if (value is Args.Logic v1)
            {
                return Logic.Contains(v1);
            }
            if (value is Args.ApplicationBoolean v2)
            {
                return ApplicationBoolean.Contains(v2);
            }
            if (value is Args.Mouse v3)
            {
                return Mouse.Contains(v3);
            }
            if (value is Args.Edit v4)
            {
                return Edit.Contains(v4);
            }
            if (value is Args.PointerEvents v5)
            {
                return PointerEvents.Contains(v5);
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
            if (value is Args.Application v10)
            {
                return Application.Contains(v10);
            }
            if (value is Args.Renderer v11)
            {
                return Renderer.Contains(v11);
            }
            if (value is Args.BaseEvents v12)
            {
                return BaseEvents.Contains(v12);
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

        public delegate bool EnumUnderlyingGetter<T>(T flag, out uint v) where T : Enum;
        static class EnumConvert
        {
            public static bool LogicToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.Logic v1)
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
            public static bool ApplicationToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.Application v1)
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
            public static bool ApplicationBooleanToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.ApplicationBoolean v1)
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
            public static bool RendererToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.Renderer v1)
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
            public static bool MouseToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.Mouse v1)
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
            public static bool EditToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.Edit v1)
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
            public static bool BaseEventsToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.BaseEvents v1)
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
            public static bool PointerEventsToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.PointerEvents v1)
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
            public static bool PhysicsColliderToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.PhysicsCollider v1)
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
            public static bool PhysicsCollider2DToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.PhysicsCollider2D v1)
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
            public static bool PhysicsCollisionToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.PhysicsCollision v1)
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
            public static bool PhysicsCollision2DToUInt32<T>(T flag, out uint v)
                where T : Enum
            {
                if (flag is Args.PhysicsCollision2D v1)
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

            public static uint ToUInt32<T>(T flag)
                where T : Enum
            {
                uint value = default;
                if (!(
                    LogicToUInt32(flag, out value) ||
                    ApplicationToUInt32(flag, out value) ||
                    ApplicationBooleanToUInt32(flag, out value) ||
                    RendererToUInt32(flag, out value) ||
                    MouseToUInt32(flag, out value) ||
                    EditToUInt32(flag, out value) ||
                    BaseEventsToUInt32(flag, out value) ||
                    PointerEventsToUInt32(flag, out value) ||
                    PhysicsColliderToUInt32(flag, out value) ||
                    PhysicsCollider2DToUInt32(flag, out value) ||
                    PhysicsCollisionToUInt32(flag, out value) ||
                    PhysicsCollision2DToUInt32(flag, out value)
                ))
                {
                    value = Convert.ToUInt32(flag);
                }
                return value;
            }
        }
    }
}