namespace XOR.Behaviour.Args
{
    [Title("Logic")]
    public enum Logic : uint
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
    }

    [Title("Application")]
    public enum Application : uint
    {
        OnApplicationQuit = 1 << 0
    }

    [Title("Application - bool")]
    [Args(typeof(bool))]
    public enum ApplicationBoolean : uint
    {
        OnApplicationFocus = 1 << 0,
        OnApplicationPause = 1 << 1,
    }

    [Title("Renderer")]
    public enum Renderer : uint
    {
        OnPreCull = 1 << 0,
        OnWillRenderObject = 1 << 1,
        OnBecameVisible = 1 << 2,
        OnBecameInvisible = 1 << 3,
        OnPreRender = 1 << 4,
        OnRenderObject = 1 << 5,
        OnPostRender = 1 << 6
    }

    [Title("Edit")]
    public enum Edit : uint
    {
        OnDrawGizmos = 1 << 0,
        OnDrawGizmosSelected = 1 << 1,
        OnSceneGUI = 1 << 2,
        Reset = 1 << 3,
        OnValidate = 1 << 4
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

    [Title("BaseEvents")]
    [Args(typeof(UnityEngine.EventSystems.BaseEventData))]
    public enum BaseEvents : uint
    {
        [Impl(typeof(UnityEngine.EventSystems.ISelectHandler))]
        OnSelect = 1 << 0,
        [Impl(typeof(UnityEngine.EventSystems.IDeselectHandler))]
        OnDeselect = 1 << 1,
        [Impl(typeof(UnityEngine.EventSystems.ISubmitHandler))]
        OnSubmit = 1 << 2,
        [Impl(typeof(UnityEngine.EventSystems.ICancelHandler))]
        OnCancel = 1 << 3,
    }

    [Title("PointerEvents")]
    [Args(typeof(UnityEngine.EventSystems.PointerEventData))]
    public enum PointerEvents : uint
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

        [Impl(typeof(UnityEngine.EventSystems.IDropHandler))]
        OnDrop = 1 << 8,
        [Impl(typeof(UnityEngine.EventSystems.IScrollHandler))]
        OnScroll = 1 << 9,
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
        static Logic[] logicBase;
        public static Logic[] GetLogicBase()
        {
            if (logicBase == null) logicBase = new Logic[] {
                Logic.Awake,
                Logic.Start,
                Logic.OnDestroy,
                Logic.OnEnable,
                Logic.OnDisable,
            };
            return logicBase;
        }
    }

}
