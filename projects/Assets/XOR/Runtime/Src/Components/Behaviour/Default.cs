namespace XOR.Behaviour
{
    public static class Default
    {
        public class AwakeBehaviour : XOR.Behaviour.Mono
        {

            private void Awake()
            {
                Invoke(XOR.Behaviour.Args.Mono.Awake);
            }
        }

        public class StartBehaviour : XOR.Behaviour.Mono
        {

            private void Start()
            {
                Invoke(XOR.Behaviour.Args.Mono.Start);
            }
        }

        public class UpdateBehaviour : XOR.Behaviour.Mono
        {

            private void Update()
            {
                Invoke(XOR.Behaviour.Args.Mono.Update);
            }
        }

        public class FixedUpdateBehaviour : XOR.Behaviour.Mono
        {

            private void FixedUpdate()
            {
                Invoke(XOR.Behaviour.Args.Mono.FixedUpdate);
            }
        }

        public class LateUpdateBehaviour : XOR.Behaviour.Mono
        {

            private void LateUpdate()
            {
                Invoke(XOR.Behaviour.Args.Mono.LateUpdate);
            }
        }

        public class OnEnableBehaviour : XOR.Behaviour.Mono
        {

            private void OnEnable()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnEnable);
            }
        }

        public class OnDisableBehaviour : XOR.Behaviour.Mono
        {

            private void OnDisable()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnDisable);
            }
        }

        public class OnDestroyBehaviour : XOR.Behaviour.Mono
        {

            private void OnDestroy()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnDestroy);
            }
        }

        public class OnGUIBehaviour : XOR.Behaviour.Mono
        {

            private void OnGUI()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnGUI);
            }
        }

        public class OnApplicationQuitBehaviour : XOR.Behaviour.Mono
        {

            private void OnApplicationQuit()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnApplicationQuit);
            }
        }

        public class OnBecameVisibleBehaviour : XOR.Behaviour.Mono
        {

            private void OnBecameVisible()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnBecameVisible);
            }
        }

        public class OnBecameInvisibleBehaviour : XOR.Behaviour.Mono
        {

            private void OnBecameInvisible()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnBecameInvisible);
            }
        }

        public class OnApplicationFocusBehaviour : XOR.Behaviour.MonoBoolean
        {

            private void OnApplicationFocus(System.Boolean arg0)
            {
                Invoke(XOR.Behaviour.Args.MonoBoolean.OnApplicationFocus, arg0);
            }
        }

        public class OnApplicationPauseBehaviour : XOR.Behaviour.MonoBoolean
        {

            private void OnApplicationPause(System.Boolean arg0)
            {
                Invoke(XOR.Behaviour.Args.MonoBoolean.OnApplicationPause, arg0);
            }
        }

        public class OnDrawGizmosSelectedBehaviour : XOR.Behaviour.Gizmos
        {

            private void OnDrawGizmosSelected()
            {
                Invoke(XOR.Behaviour.Args.Gizmos.OnDrawGizmosSelected);
            }
        }

        public class OnSceneGUIBehaviour : XOR.Behaviour.Gizmos
        {

            private void OnSceneGUI()
            {
                Invoke(XOR.Behaviour.Args.Gizmos.OnSceneGUI);
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

        public class OnBeginDragBehaviour : XOR.Behaviour.EventSystems, UnityEngine.EventSystems.IBeginDragHandler
        {

            public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnBeginDrag, arg0);
            }
        }

        public class OnDragBehaviour : XOR.Behaviour.EventSystems, UnityEngine.EventSystems.IDragHandler
        {

            public void OnDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnDrag, arg0);
            }
        }

        public class OnEndDragBehaviour : XOR.Behaviour.EventSystems, UnityEngine.EventSystems.IEndDragHandler
        {

            public void OnEndDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnEndDrag, arg0);
            }
        }

        public class OnPointerClickBehaviour : XOR.Behaviour.EventSystems, UnityEngine.EventSystems.IPointerClickHandler
        {

            public void OnPointerClick(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerClick, arg0);
            }
        }

        public class OnPointerDownBehaviour : XOR.Behaviour.EventSystems, UnityEngine.EventSystems.IPointerDownHandler
        {

            public void OnPointerDown(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerDown, arg0);
            }
        }

        public class OnPointerEnterBehaviour : XOR.Behaviour.EventSystems, UnityEngine.EventSystems.IPointerEnterHandler
        {

            public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerEnter, arg0);
            }
        }

        public class OnPointerExitBehaviour : XOR.Behaviour.EventSystems, UnityEngine.EventSystems.IPointerExitHandler
        {

            public void OnPointerExit(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerExit, arg0);
            }
        }

        public class OnPointerUpBehaviour : XOR.Behaviour.EventSystems, UnityEngine.EventSystems.IPointerUpHandler
        {

            public void OnPointerUp(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerUp, arg0);
            }
        }

        public class EventSystemsBehaviour7 : XOR.Behaviour.EventSystems, UnityEngine.EventSystems.IBeginDragHandler, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IEndDragHandler
        {

            public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnBeginDrag, arg0);
            }
            public void OnDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnDrag, arg0);
            }
            public void OnEndDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnEndDrag, arg0);
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
            XOR.Behaviour.Factory.Register<EventSystemsBehaviour7>(XOR.Behaviour.Args.EventSystems.OnBeginDrag | XOR.Behaviour.Args.EventSystems.OnDrag | XOR.Behaviour.Args.EventSystems.OnEndDrag);
            XOR.Behaviour.Factory.Register<PhysicsColliderBehaviour7>(XOR.Behaviour.Args.PhysicsCollider.OnTriggerEnter | XOR.Behaviour.Args.PhysicsCollider.OnTriggerStay | XOR.Behaviour.Args.PhysicsCollider.OnTriggerExit);
            XOR.Behaviour.Factory.Register<PhysicsCollider2DBehaviour7>(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerEnter2D | XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerStay2D | XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerExit2D);
            XOR.Behaviour.Factory.Register<PhysicsCollisionBehaviour7>(XOR.Behaviour.Args.PhysicsCollision.OnCollisionEnter | XOR.Behaviour.Args.PhysicsCollision.OnCollisionStay | XOR.Behaviour.Args.PhysicsCollision.OnCollisionExit);
            XOR.Behaviour.Factory.Register<PhysicsCollision2DBehaviour7>(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionEnter2D | XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionStay2D | XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionExit2D);
            XOR.Behaviour.Factory.Register<AwakeBehaviour>(XOR.Behaviour.Args.Mono.Awake);
            XOR.Behaviour.Factory.Register<StartBehaviour>(XOR.Behaviour.Args.Mono.Start);
            XOR.Behaviour.Factory.Register<UpdateBehaviour>(XOR.Behaviour.Args.Mono.Update);
            XOR.Behaviour.Factory.Register<FixedUpdateBehaviour>(XOR.Behaviour.Args.Mono.FixedUpdate);
            XOR.Behaviour.Factory.Register<LateUpdateBehaviour>(XOR.Behaviour.Args.Mono.LateUpdate);
            XOR.Behaviour.Factory.Register<OnEnableBehaviour>(XOR.Behaviour.Args.Mono.OnEnable);
            XOR.Behaviour.Factory.Register<OnDisableBehaviour>(XOR.Behaviour.Args.Mono.OnDisable);
            XOR.Behaviour.Factory.Register<OnDestroyBehaviour>(XOR.Behaviour.Args.Mono.OnDestroy);
            XOR.Behaviour.Factory.Register<OnGUIBehaviour>(XOR.Behaviour.Args.Mono.OnGUI);
            XOR.Behaviour.Factory.Register<OnApplicationQuitBehaviour>(XOR.Behaviour.Args.Mono.OnApplicationQuit);
            XOR.Behaviour.Factory.Register<OnBecameVisibleBehaviour>(XOR.Behaviour.Args.Mono.OnBecameVisible);
            XOR.Behaviour.Factory.Register<OnBecameInvisibleBehaviour>(XOR.Behaviour.Args.Mono.OnBecameInvisible);
            XOR.Behaviour.Factory.Register<OnApplicationFocusBehaviour>(XOR.Behaviour.Args.MonoBoolean.OnApplicationFocus);
            XOR.Behaviour.Factory.Register<OnApplicationPauseBehaviour>(XOR.Behaviour.Args.MonoBoolean.OnApplicationPause);
            XOR.Behaviour.Factory.Register<OnDrawGizmosSelectedBehaviour>(XOR.Behaviour.Args.Gizmos.OnDrawGizmosSelected);
            XOR.Behaviour.Factory.Register<OnSceneGUIBehaviour>(XOR.Behaviour.Args.Gizmos.OnSceneGUI);
            XOR.Behaviour.Factory.Register<OnMouseDownBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseDown);
            XOR.Behaviour.Factory.Register<OnMouseDragBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseDrag);
            XOR.Behaviour.Factory.Register<OnMouseEnterBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseEnter);
            XOR.Behaviour.Factory.Register<OnMouseExitBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseExit);
            XOR.Behaviour.Factory.Register<OnMouseOverBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseOver);
            XOR.Behaviour.Factory.Register<OnMouseUpAsButtonBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseUpAsButton);
            XOR.Behaviour.Factory.Register<OnMouseUpBehaviour>(XOR.Behaviour.Args.Mouse.OnMouseUp);
            XOR.Behaviour.Factory.Register<OnBeginDragBehaviour>(XOR.Behaviour.Args.EventSystems.OnBeginDrag);
            XOR.Behaviour.Factory.Register<OnDragBehaviour>(XOR.Behaviour.Args.EventSystems.OnDrag);
            XOR.Behaviour.Factory.Register<OnEndDragBehaviour>(XOR.Behaviour.Args.EventSystems.OnEndDrag);
            XOR.Behaviour.Factory.Register<OnPointerClickBehaviour>(XOR.Behaviour.Args.EventSystems.OnPointerClick);
            XOR.Behaviour.Factory.Register<OnPointerDownBehaviour>(XOR.Behaviour.Args.EventSystems.OnPointerDown);
            XOR.Behaviour.Factory.Register<OnPointerEnterBehaviour>(XOR.Behaviour.Args.EventSystems.OnPointerEnter);
            XOR.Behaviour.Factory.Register<OnPointerExitBehaviour>(XOR.Behaviour.Args.EventSystems.OnPointerExit);
            XOR.Behaviour.Factory.Register<OnPointerUpBehaviour>(XOR.Behaviour.Args.EventSystems.OnPointerUp);
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