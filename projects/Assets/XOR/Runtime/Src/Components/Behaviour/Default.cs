namespace XOR.Behaviour
{
    public static class Default
    {
        public class AwakeBehaviour : XOR.Behaviour.Logic
        {

            private void Awake()
            {
                Invoke(XOR.Behaviour.Args.Logic.Awake);
            }
        }

        public class StartBehaviour : XOR.Behaviour.Logic
        {

            private void Start()
            {
                Invoke(XOR.Behaviour.Args.Logic.Start);
            }
        }

        public class UpdateBehaviour : XOR.Behaviour.Logic
        {

            private void Update()
            {
                Invoke(XOR.Behaviour.Args.Logic.Update);
            }
        }

        public class FixedUpdateBehaviour : XOR.Behaviour.Logic
        {

            private void FixedUpdate()
            {
                Invoke(XOR.Behaviour.Args.Logic.FixedUpdate);
            }
        }

        public class LateUpdateBehaviour : XOR.Behaviour.Logic
        {

            private void LateUpdate()
            {
                Invoke(XOR.Behaviour.Args.Logic.LateUpdate);
            }
        }

        public class OnEnableBehaviour : XOR.Behaviour.Logic
        {

            private void OnEnable()
            {
                Invoke(XOR.Behaviour.Args.Logic.OnEnable);
            }
        }

        public class OnDisableBehaviour : XOR.Behaviour.Logic
        {

            private void OnDisable()
            {
                Invoke(XOR.Behaviour.Args.Logic.OnDisable);
            }
        }

        public class OnDestroyBehaviour : XOR.Behaviour.Logic
        {

            private void OnDestroy()
            {
                Invoke(XOR.Behaviour.Args.Logic.OnDestroy);
            }
        }

        public class OnGUIBehaviour : XOR.Behaviour.Logic
        {

            private void OnGUI()
            {
                Invoke(XOR.Behaviour.Args.Logic.OnGUI);
            }
        }

        public class OnApplicationQuitBehaviour : XOR.Behaviour.Application
        {

            private void OnApplicationQuit()
            {
                Invoke(XOR.Behaviour.Args.Application.OnApplicationQuit);
            }
        }

        public class OnApplicationFocusBehaviour : XOR.Behaviour.ApplicationBoolean
        {

            private void OnApplicationFocus(System.Boolean arg0)
            {
                Invoke(XOR.Behaviour.Args.ApplicationBoolean.OnApplicationFocus, arg0);
            }
        }

        public class OnApplicationPauseBehaviour : XOR.Behaviour.ApplicationBoolean
        {

            private void OnApplicationPause(System.Boolean arg0)
            {
                Invoke(XOR.Behaviour.Args.ApplicationBoolean.OnApplicationPause, arg0);
            }
        }

        public class OnDrawGizmosBehaviour : XOR.Behaviour.Edit
        {

            private void OnDrawGizmos()
            {
                Invoke(XOR.Behaviour.Args.Edit.OnDrawGizmos);
            }
        }

        public class OnDrawGizmosSelectedBehaviour : XOR.Behaviour.Edit
        {

            private void OnDrawGizmosSelected()
            {
                Invoke(XOR.Behaviour.Args.Edit.OnDrawGizmosSelected);
            }
        }

        public class OnSceneGUIBehaviour : XOR.Behaviour.Edit
        {

            private void OnSceneGUI()
            {
                Invoke(XOR.Behaviour.Args.Edit.OnSceneGUI);
            }
        }

        public class ResetBehaviour : XOR.Behaviour.Edit
        {

            private void Reset()
            {
                Invoke(XOR.Behaviour.Args.Edit.Reset);
            }
        }

        public class OnValidateBehaviour : XOR.Behaviour.Edit
        {

            private void OnValidate()
            {
                Invoke(XOR.Behaviour.Args.Edit.OnValidate);
            }
        }

        public class OnPreCullBehaviour : XOR.Behaviour.Renderer
        {

            private void OnPreCull()
            {
                Invoke(XOR.Behaviour.Args.Renderer.OnPreCull);
            }
        }

        public class OnWillRenderObjectBehaviour : XOR.Behaviour.Renderer
        {

            private void OnWillRenderObject()
            {
                Invoke(XOR.Behaviour.Args.Renderer.OnWillRenderObject);
            }
        }

        public class OnBecameVisibleBehaviour : XOR.Behaviour.Renderer
        {

            private void OnBecameVisible()
            {
                Invoke(XOR.Behaviour.Args.Renderer.OnBecameVisible);
            }
        }

        public class OnBecameInvisibleBehaviour : XOR.Behaviour.Renderer
        {

            private void OnBecameInvisible()
            {
                Invoke(XOR.Behaviour.Args.Renderer.OnBecameInvisible);
            }
        }

        public class OnPreRenderBehaviour : XOR.Behaviour.Renderer
        {

            private void OnPreRender()
            {
                Invoke(XOR.Behaviour.Args.Renderer.OnPreRender);
            }
        }

        public class OnRenderObjectBehaviour : XOR.Behaviour.Renderer
        {

            private void OnRenderObject()
            {
                Invoke(XOR.Behaviour.Args.Renderer.OnRenderObject);
            }
        }

        public class OnPostRenderBehaviour : XOR.Behaviour.Renderer
        {

            private void OnPostRender()
            {
                Invoke(XOR.Behaviour.Args.Renderer.OnPostRender);
            }
        }

        public class OnMouseDownBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseDown()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseDown);
            }
        }

        public class OnMouseDragBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseDrag()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseDrag);
            }
        }

        public class OnMouseEnterBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseEnter()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseEnter);
            }
        }

        public class OnMouseExitBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseExit()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseExit);
            }
        }

        public class OnMouseOverBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseOver()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseOver);
            }
        }

        public class OnMouseUpAsButtonBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseUpAsButton()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseUpAsButton);
            }
        }

        public class OnMouseUpBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseUp()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseUp);
            }
        }

        public class MouseBehaviour127 : XOR.Behaviour.Mouse
        {

            private void OnMouseDown()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseDown);
            }
            private void OnMouseDrag()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseDrag);
            }
            private void OnMouseEnter()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseEnter);
            }
            private void OnMouseExit()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseExit);
            }
            private void OnMouseOver()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseOver);
            }
            private void OnMouseUpAsButton()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseUpAsButton);
            }
            private void OnMouseUp()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseUp);
            }
        }

        public class OnSelectBehaviour : XOR.Behaviour.BaseEvents, UnityEngine.EventSystems.ISelectHandler
        {

            public void OnSelect(UnityEngine.EventSystems.BaseEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.BaseEvents.OnSelect, arg0);
            }
        }

        public class OnDeselectBehaviour : XOR.Behaviour.BaseEvents, UnityEngine.EventSystems.IDeselectHandler
        {

            public void OnDeselect(UnityEngine.EventSystems.BaseEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.BaseEvents.OnDeselect, arg0);
            }
        }

        public class OnSubmitBehaviour : XOR.Behaviour.BaseEvents, UnityEngine.EventSystems.ISubmitHandler
        {

            public void OnSubmit(UnityEngine.EventSystems.BaseEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.BaseEvents.OnSubmit, arg0);
            }
        }

        public class OnCancelBehaviour : XOR.Behaviour.BaseEvents, UnityEngine.EventSystems.ICancelHandler
        {

            public void OnCancel(UnityEngine.EventSystems.BaseEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.BaseEvents.OnCancel, arg0);
            }
        }

        public class OnBeginDragBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IBeginDragHandler
        {

            public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnBeginDrag, arg0);
            }
        }

        public class OnDragBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IDragHandler
        {

            public void OnDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnDrag, arg0);
            }
        }

        public class OnEndDragBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IEndDragHandler
        {

            public void OnEndDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnEndDrag, arg0);
            }
        }

        public class OnPointerClickBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IPointerClickHandler
        {

            public void OnPointerClick(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnPointerClick, arg0);
            }
        }

        public class OnPointerDownBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IPointerDownHandler
        {

            public void OnPointerDown(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnPointerDown, arg0);
            }
        }

        public class OnPointerEnterBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IPointerEnterHandler
        {

            public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnPointerEnter, arg0);
            }
        }

        public class OnPointerExitBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IPointerExitHandler
        {

            public void OnPointerExit(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnPointerExit, arg0);
            }
        }

        public class OnPointerUpBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IPointerUpHandler
        {

            public void OnPointerUp(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnPointerUp, arg0);
            }
        }

        public class OnDropBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IDropHandler
        {

            public void OnDrop(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnDrop, arg0);
            }
        }

        public class OnScrollBehaviour : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IScrollHandler
        {

            public void OnScroll(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnScroll, arg0);
            }
        }

        public class PointerEventsBehaviour7 : XOR.Behaviour.PointerEvents, UnityEngine.EventSystems.IBeginDragHandler, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IEndDragHandler
        {

            public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnBeginDrag, arg0);
            }
            public void OnDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnDrag, arg0);
            }
            public void OnEndDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.PointerEvents.OnEndDrag, arg0);
            }
        }

        public class OnTriggerEnterBehaviour : XOR.Behaviour.PhysicsCollider
        {

            private void OnTriggerEnter(UnityEngine.Collider arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider.OnTriggerEnter, arg0);
            }
        }

        public class OnTriggerStayBehaviour : XOR.Behaviour.PhysicsCollider
        {

            private void OnTriggerStay(UnityEngine.Collider arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider.OnTriggerStay, arg0);
            }
        }

        public class OnTriggerExitBehaviour : XOR.Behaviour.PhysicsCollider
        {

            private void OnTriggerExit(UnityEngine.Collider arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider.OnTriggerExit, arg0);
            }
        }

        public class PhysicsColliderBehaviour7 : XOR.Behaviour.PhysicsCollider
        {

            private void OnTriggerEnter(UnityEngine.Collider arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider.OnTriggerEnter, arg0);
            }
            private void OnTriggerStay(UnityEngine.Collider arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider.OnTriggerStay, arg0);
            }
            private void OnTriggerExit(UnityEngine.Collider arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider.OnTriggerExit, arg0);
            }
        }

        public class OnTriggerEnter2DBehaviour : XOR.Behaviour.PhysicsCollider2D
        {

            private void OnTriggerEnter2D(UnityEngine.Collider2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerEnter2D, arg0);
            }
        }

        public class OnTriggerStay2DBehaviour : XOR.Behaviour.PhysicsCollider2D
        {

            private void OnTriggerStay2D(UnityEngine.Collider2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerStay2D, arg0);
            }
        }

        public class OnTriggerExit2DBehaviour : XOR.Behaviour.PhysicsCollider2D
        {

            private void OnTriggerExit2D(UnityEngine.Collider2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerExit2D, arg0);
            }
        }

        public class PhysicsCollider2DBehaviour7 : XOR.Behaviour.PhysicsCollider2D
        {

            private void OnTriggerEnter2D(UnityEngine.Collider2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerEnter2D, arg0);
            }
            private void OnTriggerStay2D(UnityEngine.Collider2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerStay2D, arg0);
            }
            private void OnTriggerExit2D(UnityEngine.Collider2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerExit2D, arg0);
            }
        }

        public class OnCollisionEnterBehaviour : XOR.Behaviour.PhysicsCollision
        {

            private void OnCollisionEnter(UnityEngine.Collision arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision.OnCollisionEnter, arg0);
            }
        }

        public class OnCollisionStayBehaviour : XOR.Behaviour.PhysicsCollision
        {

            private void OnCollisionStay(UnityEngine.Collision arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision.OnCollisionStay, arg0);
            }
        }

        public class OnCollisionExitBehaviour : XOR.Behaviour.PhysicsCollision
        {

            private void OnCollisionExit(UnityEngine.Collision arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision.OnCollisionExit, arg0);
            }
        }

        public class PhysicsCollisionBehaviour7 : XOR.Behaviour.PhysicsCollision
        {

            private void OnCollisionEnter(UnityEngine.Collision arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision.OnCollisionEnter, arg0);
            }
            private void OnCollisionStay(UnityEngine.Collision arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision.OnCollisionStay, arg0);
            }
            private void OnCollisionExit(UnityEngine.Collision arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision.OnCollisionExit, arg0);
            }
        }

        public class OnCollisionEnter2DBehaviour : XOR.Behaviour.PhysicsCollision2D
        {

            private void OnCollisionEnter2D(UnityEngine.Collision2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionEnter2D, arg0);
            }
        }

        public class OnCollisionStay2DBehaviour : XOR.Behaviour.PhysicsCollision2D
        {

            private void OnCollisionStay2D(UnityEngine.Collision2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionStay2D, arg0);
            }
        }

        public class OnCollisionExit2DBehaviour : XOR.Behaviour.PhysicsCollision2D
        {

            private void OnCollisionExit2D(UnityEngine.Collision2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionExit2D, arg0);
            }
        }

        public class PhysicsCollision2DBehaviour7 : XOR.Behaviour.PhysicsCollision2D
        {

            private void OnCollisionEnter2D(UnityEngine.Collision2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionEnter2D, arg0);
            }
            private void OnCollisionStay2D(UnityEngine.Collision2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionStay2D, arg0);
            }
            private void OnCollisionExit2D(UnityEngine.Collision2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionExit2D, arg0);
            }
        }

        public static void Register()
        {

            XOR.Behaviour.Factory.Register<MouseBehaviour127>(XOR.Behaviour.Args.Mouse.OnMouseDown | XOR.Behaviour.Args.Mouse.OnMouseDrag | XOR.Behaviour.Args.Mouse.OnMouseEnter | XOR.Behaviour.Args.Mouse.OnMouseExit | XOR.Behaviour.Args.Mouse.OnMouseOver | XOR.Behaviour.Args.Mouse.OnMouseUpAsButton | XOR.Behaviour.Args.Mouse.OnMouseUp);
            XOR.Behaviour.Factory.Register<PointerEventsBehaviour7>(XOR.Behaviour.Args.PointerEvents.OnBeginDrag | XOR.Behaviour.Args.PointerEvents.OnDrag | XOR.Behaviour.Args.PointerEvents.OnEndDrag);
            XOR.Behaviour.Factory.Register<PhysicsColliderBehaviour7>(XOR.Behaviour.Args.PhysicsCollider.OnTriggerEnter | XOR.Behaviour.Args.PhysicsCollider.OnTriggerStay | XOR.Behaviour.Args.PhysicsCollider.OnTriggerExit);
            XOR.Behaviour.Factory.Register<PhysicsCollider2DBehaviour7>(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerEnter2D | XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerStay2D | XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerExit2D);
            XOR.Behaviour.Factory.Register<PhysicsCollisionBehaviour7>(XOR.Behaviour.Args.PhysicsCollision.OnCollisionEnter | XOR.Behaviour.Args.PhysicsCollision.OnCollisionStay | XOR.Behaviour.Args.PhysicsCollision.OnCollisionExit);
            XOR.Behaviour.Factory.Register<PhysicsCollision2DBehaviour7>(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionEnter2D | XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionStay2D | XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionExit2D);
            XOR.Behaviour.Factory.Register<AwakeBehaviour>(XOR.Behaviour.Args.Logic.Awake);
            XOR.Behaviour.Factory.Register<StartBehaviour>(XOR.Behaviour.Args.Logic.Start);
            XOR.Behaviour.Factory.Register<UpdateBehaviour>(XOR.Behaviour.Args.Logic.Update);
            XOR.Behaviour.Factory.Register<FixedUpdateBehaviour>(XOR.Behaviour.Args.Logic.FixedUpdate);
            XOR.Behaviour.Factory.Register<LateUpdateBehaviour>(XOR.Behaviour.Args.Logic.LateUpdate);
            XOR.Behaviour.Factory.Register<OnEnableBehaviour>(XOR.Behaviour.Args.Logic.OnEnable);
            XOR.Behaviour.Factory.Register<OnDisableBehaviour>(XOR.Behaviour.Args.Logic.OnDisable);
            XOR.Behaviour.Factory.Register<OnDestroyBehaviour>(XOR.Behaviour.Args.Logic.OnDestroy);
            XOR.Behaviour.Factory.Register<OnGUIBehaviour>(XOR.Behaviour.Args.Logic.OnGUI);
            XOR.Behaviour.Factory.Register<OnApplicationQuitBehaviour>(XOR.Behaviour.Args.Application.OnApplicationQuit);
            XOR.Behaviour.Factory.Register<OnApplicationFocusBehaviour>(XOR.Behaviour.Args.ApplicationBoolean.OnApplicationFocus);
            XOR.Behaviour.Factory.Register<OnApplicationPauseBehaviour>(XOR.Behaviour.Args.ApplicationBoolean.OnApplicationPause);
            XOR.Behaviour.Factory.Register<OnDrawGizmosBehaviour>(XOR.Behaviour.Args.Edit.OnDrawGizmos);
            XOR.Behaviour.Factory.Register<OnDrawGizmosSelectedBehaviour>(XOR.Behaviour.Args.Edit.OnDrawGizmosSelected);
            XOR.Behaviour.Factory.Register<OnSceneGUIBehaviour>(XOR.Behaviour.Args.Edit.OnSceneGUI);
            XOR.Behaviour.Factory.Register<ResetBehaviour>(XOR.Behaviour.Args.Edit.Reset);
            XOR.Behaviour.Factory.Register<OnValidateBehaviour>(XOR.Behaviour.Args.Edit.OnValidate);
            XOR.Behaviour.Factory.Register<OnPreCullBehaviour>(XOR.Behaviour.Args.Renderer.OnPreCull);
            XOR.Behaviour.Factory.Register<OnWillRenderObjectBehaviour>(XOR.Behaviour.Args.Renderer.OnWillRenderObject);
            XOR.Behaviour.Factory.Register<OnBecameVisibleBehaviour>(XOR.Behaviour.Args.Renderer.OnBecameVisible);
            XOR.Behaviour.Factory.Register<OnBecameInvisibleBehaviour>(XOR.Behaviour.Args.Renderer.OnBecameInvisible);
            XOR.Behaviour.Factory.Register<OnPreRenderBehaviour>(XOR.Behaviour.Args.Renderer.OnPreRender);
            XOR.Behaviour.Factory.Register<OnRenderObjectBehaviour>(XOR.Behaviour.Args.Renderer.OnRenderObject);
            XOR.Behaviour.Factory.Register<OnPostRenderBehaviour>(XOR.Behaviour.Args.Renderer.OnPostRender);
            XOR.Behaviour.Factory.Register<OnMouseDownBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseDown);
            XOR.Behaviour.Factory.Register<OnMouseDragBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseDrag);
            XOR.Behaviour.Factory.Register<OnMouseEnterBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseEnter);
            XOR.Behaviour.Factory.Register<OnMouseExitBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseExit);
            XOR.Behaviour.Factory.Register<OnMouseOverBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseOver);
            XOR.Behaviour.Factory.Register<OnMouseUpAsButtonBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseUpAsButton);
            XOR.Behaviour.Factory.Register<OnMouseUpBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseUp);
            XOR.Behaviour.Factory.Register<OnSelectBehaviour>(XOR.Behaviour.Args.BaseEvents.OnSelect);
            XOR.Behaviour.Factory.Register<OnDeselectBehaviour>(XOR.Behaviour.Args.BaseEvents.OnDeselect);
            XOR.Behaviour.Factory.Register<OnSubmitBehaviour>(XOR.Behaviour.Args.BaseEvents.OnSubmit);
            XOR.Behaviour.Factory.Register<OnCancelBehaviour>(XOR.Behaviour.Args.BaseEvents.OnCancel);
            XOR.Behaviour.Factory.Register<OnBeginDragBehaviour>(XOR.Behaviour.Args.PointerEvents.OnBeginDrag);
            XOR.Behaviour.Factory.Register<OnDragBehaviour>(XOR.Behaviour.Args.PointerEvents.OnDrag);
            XOR.Behaviour.Factory.Register<OnEndDragBehaviour>(XOR.Behaviour.Args.PointerEvents.OnEndDrag);
            XOR.Behaviour.Factory.Register<OnPointerClickBehaviour>(XOR.Behaviour.Args.PointerEvents.OnPointerClick);
            XOR.Behaviour.Factory.Register<OnPointerDownBehaviour>(XOR.Behaviour.Args.PointerEvents.OnPointerDown);
            XOR.Behaviour.Factory.Register<OnPointerEnterBehaviour>(XOR.Behaviour.Args.PointerEvents.OnPointerEnter);
            XOR.Behaviour.Factory.Register<OnPointerExitBehaviour>(XOR.Behaviour.Args.PointerEvents.OnPointerExit);
            XOR.Behaviour.Factory.Register<OnPointerUpBehaviour>(XOR.Behaviour.Args.PointerEvents.OnPointerUp);
            XOR.Behaviour.Factory.Register<OnDropBehaviour>(XOR.Behaviour.Args.PointerEvents.OnDrop);
            XOR.Behaviour.Factory.Register<OnScrollBehaviour>(XOR.Behaviour.Args.PointerEvents.OnScroll);
            XOR.Behaviour.Factory.Register<OnTriggerEnterBehaviour>(XOR.Behaviour.Args.PhysicsCollider.OnTriggerEnter);
            XOR.Behaviour.Factory.Register<OnTriggerStayBehaviour>(XOR.Behaviour.Args.PhysicsCollider.OnTriggerStay);
            XOR.Behaviour.Factory.Register<OnTriggerExitBehaviour>(XOR.Behaviour.Args.PhysicsCollider.OnTriggerExit);
            XOR.Behaviour.Factory.Register<OnTriggerEnter2DBehaviour>(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerEnter2D);
            XOR.Behaviour.Factory.Register<OnTriggerStay2DBehaviour>(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerStay2D);
            XOR.Behaviour.Factory.Register<OnTriggerExit2DBehaviour>(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerExit2D);
            XOR.Behaviour.Factory.Register<OnCollisionEnterBehaviour>(XOR.Behaviour.Args.PhysicsCollision.OnCollisionEnter);
            XOR.Behaviour.Factory.Register<OnCollisionStayBehaviour>(XOR.Behaviour.Args.PhysicsCollision.OnCollisionStay);
            XOR.Behaviour.Factory.Register<OnCollisionExitBehaviour>(XOR.Behaviour.Args.PhysicsCollision.OnCollisionExit);
            XOR.Behaviour.Factory.Register<OnCollisionEnter2DBehaviour>(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionEnter2D);
            XOR.Behaviour.Factory.Register<OnCollisionStay2DBehaviour>(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionStay2D);
            XOR.Behaviour.Factory.Register<OnCollisionExit2DBehaviour>(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionExit2D);
        }

    }
}