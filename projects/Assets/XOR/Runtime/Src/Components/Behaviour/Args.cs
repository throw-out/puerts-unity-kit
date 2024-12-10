namespace XOR.Behaviour.Args
{
    [Title("Mono")]
    public enum Mono : uint
    {
        Awake = 1 << 0,
        Start = 1 << 1,
        Update = 1 << 2,
        FixedUpdate = 1 << 3,
        LateUpdate = 1 << 4,
        OnEnable = 1 << 5,
        OnDisable = 1 << 6,
        OnDestroy = 1 << 7,
        OnGUI = 1 << 8,
        OnApplicationQuit = 1 << 9,
        OnBecameVisible = 1 << 10,
        OnBecameInvisible = 1 << 11,
    }

    [Title("Mono - bool")]
    [Args(typeof(bool))]
    public enum MonoBoolean : uint
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
    public enum EventSystems : uint
    {
        [Impl(typeof(UnityEngine.EventSystems.IBeginDragHandler))]
        OnBeginDrag = 1 << 0,
        [Impl(typeof(UnityEngine.EventSystems.IDragHandler))]
        OnDrag = 1 << 1,
        [Impl(typeof(UnityEngine.EventSystems.IEndDragHandler))]
        OnEndDrag = 1 << 2,
        [Impl(typeof(UnityEngine.EventSystems.IPointerClickHandler))]
        OnPointerClick = 1 << 3,
        [Impl(typeof(UnityEngine.EventSystems.IPointerDownHandler))]
        OnPointerDown = 1 << 4,
        [Impl(typeof(UnityEngine.EventSystems.IPointerEnterHandler))]
        OnPointerEnter = 1 << 5,
        [Impl(typeof(UnityEngine.EventSystems.IPointerExitHandler))]
        OnPointerExit = 1 << 6,
        [Impl(typeof(UnityEngine.EventSystems.IPointerUpHandler))]
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

    public static class Extensions
    {
        static Mono[] monoBase;
        public static Mono[] GetMonoBase()
        {
            if (monoBase == null) monoBase = new Mono[] {
                Mono.Awake,
                Mono.Start,
                Mono.OnDestroy,
                Mono.OnEnable,
                Mono.OnDisable,
            };
            return monoBase;
        }
    }

}
