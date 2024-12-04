
using System;
using System.Collections.Generic;

namespace XOR.Behaviour.Args
{
    [Title("Behaviour")]
    public enum Behaviour : uint
    {
        Update = 1 << 0,
        FixedUpdate = 1 << 1,
        LateUpdate = 1 << 2,
        OnEnable = 1 << 3,
        OnDisable = 1 << 4,
        OnDestroy = 1 << 5,
        OnGUI = 1 << 6,
        OnApplicationQuit = 1 << 7,
        OnBecameVisible = 1 << 8,
        OnBecameInvisible = 1 << 9,
    }

    [Title("Behaviour - bool")]
    [Args(typeof(bool))]
    public enum BehaviourBoolean : uint
    {
        OnApplicationFocus = 1 << 0,
        OnApplicationPause = 1 << 1,
    }

    [Title("Gizmos")]
    public enum Gizmos : uint
    {
        OnDrawGizmosSelected = 1 << 0,
        OnSceneGUI = 1 << 1,
    }

    [Title("MouseEvents")]
    public enum Mouse : uint
    {
        OnMouseDown = 1 << 0,
        OnMouseDrag = 1 << 1,
        OnMouseEnter = 1 << 2,
        OnMouseExit = 1 << 3,
        OnMouseOver = 1 << 4,
        OnMouseUpAsButton = 1 << 5,
        OnMouseUp = 1 << 6,
    }

    [Title("EventSystems")]
    [Args(typeof(UnityEngine.EventSystems.PointerEventData))]
    public enum EventSystemsPointerEventData : uint
    {
        OnBeginDrag = 1 << 0,
        OnDrag = 1 << 1,
        OnEndDrag = 1 << 2,
        OnPointerClick = 1 << 3,
        OnPointerDown = 1 << 4,
        OnPointerEnter = 1 << 5,
        OnPointerExit = 1 << 6,
        OnPointerUp = 1 << 7,
    }

    [Title("Physics - Collider")]
    [Args(typeof(UnityEngine.Collider))]
    public enum PhysicsCollider : uint
    {
        OnTriggerEnter = 1 << 0,
        OnTriggerStay = 1 << 1,
        OnTriggerExit = 1 << 2,
    }

    [Title("Physics - Collider2D")]
    [Args(typeof(UnityEngine.Collider2D))]
    public enum PhysicsCollider2D : uint
    {
        OnTriggerEnter2D = 1 << 0,
        OnTriggerStay2D = 1 << 1,
        OnTriggerExit2D = 1 << 2,
    }

    [Title("Physics - Collision")]
    [Args(typeof(UnityEngine.Collision))]
    public enum PhysicsCollision : uint
    {
        OnCollisionEnter = 1 << 0,
        OnCollisionStay = 1 << 1,
        OnCollisionExit = 1 << 2,
    }

    [Title("Physics - Collision2D")]
    [Args(typeof(UnityEngine.Collision2D))]
    public enum PhysicsCollision2D : uint
    {
        OnCollisionEnter2D = 1 << 0,
        OnCollisionStay2D = 1 << 1,
        OnCollisionExit2D = 1 << 2,
    }

    internal static class Extensions
    {
        static Dictionary<Type, object> valuesDict = new Dictionary<Type, object>();
        public static T[] Everything<T>()
            where T : Enum
        {
            if (valuesDict.TryGetValue(typeof(T), out var values))
            {
                return (T[])values;
            }

            
            return null;
        }
    }

}
