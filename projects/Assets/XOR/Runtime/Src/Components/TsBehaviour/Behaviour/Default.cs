namespace XOR.Behaviour
{
    public static class Default
    {
        protected class AwakeBehaviour : XOR.Behaviour.Mono
        {

            private void Awake()
            {
                Invoke(XOR.Behaviour.Args.Mono.Awake);
            }
        }

        protected class StartBehaviour : XOR.Behaviour.Mono
        {

            private void Start()
            {
                Invoke(XOR.Behaviour.Args.Mono.Start);
            }
        }

        protected class UpdateBehaviour : XOR.Behaviour.Mono
        {

            private void Update()
            {
                Invoke(XOR.Behaviour.Args.Mono.Update);
            }
        }

        protected class FixedUpdateBehaviour : XOR.Behaviour.Mono
        {

            private void FixedUpdate()
            {
                Invoke(XOR.Behaviour.Args.Mono.FixedUpdate);
            }
        }

        protected class LateUpdateBehaviour : XOR.Behaviour.Mono
        {

            private void LateUpdate()
            {
                Invoke(XOR.Behaviour.Args.Mono.LateUpdate);
            }
        }

        protected class OnEnableBehaviour : XOR.Behaviour.Mono
        {

            private void OnEnable()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnEnable);
            }
        }

        protected class OnDisableBehaviour : XOR.Behaviour.Mono
        {

            private void OnDisable()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnDisable);
            }
        }

        protected class OnDestroyBehaviour : XOR.Behaviour.Mono
        {

            private void OnDestroy()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnDestroy);
            }
        }

        protected class OnGUIBehaviour : XOR.Behaviour.Mono
        {

            private void OnGUI()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnGUI);
            }
        }

        protected class OnApplicationQuitBehaviour : XOR.Behaviour.Mono
        {

            private void OnApplicationQuit()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnApplicationQuit);
            }
        }

        protected class OnBecameVisibleBehaviour : XOR.Behaviour.Mono
        {

            private void OnBecameVisible()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnBecameVisible);
            }
        }

        protected class OnBecameInvisibleBehaviour : XOR.Behaviour.Mono
        {

            private void OnBecameInvisible()
            {
                Invoke(XOR.Behaviour.Args.Mono.OnBecameInvisible);
            }
        }

        protected class OnApplicationFocusBehaviour : XOR.Behaviour.MonoBoolean
        {

            private void OnApplicationFocus(System.Boolean arg0)
            {
                Invoke(XOR.Behaviour.Args.MonoBoolean.OnApplicationFocus, arg0);
            }
        }

        protected class OnApplicationPauseBehaviour : XOR.Behaviour.MonoBoolean
        {

            private void OnApplicationPause(System.Boolean arg0)
            {
                Invoke(XOR.Behaviour.Args.MonoBoolean.OnApplicationPause, arg0);
            }
        }

        protected class OnDrawGizmosSelectedBehaviour : XOR.Behaviour.Gizmos
        {

            private void OnDrawGizmosSelected()
            {
                Invoke(XOR.Behaviour.Args.Gizmos.OnDrawGizmosSelected);
            }
        }

        protected class OnSceneGUIBehaviour : XOR.Behaviour.Gizmos
        {

            private void OnSceneGUI()
            {
                Invoke(XOR.Behaviour.Args.Gizmos.OnSceneGUI);
            }
        }

        protected class MouseBehaviour127 : XOR.Behaviour.Mouse
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

        protected class OnMouseDownBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseDown()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseDown);
            }
        }

        protected class OnMouseDragBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseDrag()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseDrag);
            }
        }

        protected class OnMouseEnterBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseEnter()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseEnter);
            }
        }

        protected class OnMouseExitBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseExit()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseExit);
            }
        }

        protected class OnMouseOverBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseOver()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseOver);
            }
        }

        protected class OnMouseUpAsButtonBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseUpAsButton()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseUpAsButton);
            }
        }

        protected class OnMouseUpBehaviour : XOR.Behaviour.Mouse
        {

            private void OnMouseUp()
            {
                Invoke(XOR.Behaviour.Args.Mouse.OnMouseUp);
            }
        }

        protected class OnBeginDragBehaviour : XOR.Behaviour.EventSystems
        {

            private void OnBeginDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnBeginDrag, arg0);
            }
        }

        protected class OnDragBehaviour : XOR.Behaviour.EventSystems
        {

            private void OnDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnDrag, arg0);
            }
        }

        protected class OnEndDragBehaviour : XOR.Behaviour.EventSystems
        {

            private void OnEndDrag(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnEndDrag, arg0);
            }
        }

        protected class OnPointerClickBehaviour : XOR.Behaviour.EventSystems
        {

            private void OnPointerClick(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerClick, arg0);
            }
        }

        protected class OnPointerDownBehaviour : XOR.Behaviour.EventSystems
        {

            private void OnPointerDown(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerDown, arg0);
            }
        }

        protected class OnPointerEnterBehaviour : XOR.Behaviour.EventSystems
        {

            private void OnPointerEnter(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerEnter, arg0);
            }
        }

        protected class OnPointerExitBehaviour : XOR.Behaviour.EventSystems
        {

            private void OnPointerExit(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerExit, arg0);
            }
        }

        protected class OnPointerUpBehaviour : XOR.Behaviour.EventSystems
        {

            private void OnPointerUp(UnityEngine.EventSystems.PointerEventData arg0)
            {
                Invoke(XOR.Behaviour.Args.EventSystems.OnPointerUp, arg0);
            }
        }

        protected class PhysicsColliderBehaviour7 : XOR.Behaviour.PhysicsCollider
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

        protected class OnTriggerEnterBehaviour : XOR.Behaviour.PhysicsCollider
        {

            private void OnTriggerEnter(UnityEngine.Collider arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider.OnTriggerEnter, arg0);
            }
        }

        protected class OnTriggerStayBehaviour : XOR.Behaviour.PhysicsCollider
        {

            private void OnTriggerStay(UnityEngine.Collider arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider.OnTriggerStay, arg0);
            }
        }

        protected class OnTriggerExitBehaviour : XOR.Behaviour.PhysicsCollider
        {

            private void OnTriggerExit(UnityEngine.Collider arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider.OnTriggerExit, arg0);
            }
        }

        protected class PhysicsCollider2DBehaviour7 : XOR.Behaviour.PhysicsCollider2D
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

        protected class OnTriggerEnter2DBehaviour : XOR.Behaviour.PhysicsCollider2D
        {

            private void OnTriggerEnter2D(UnityEngine.Collider2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerEnter2D, arg0);
            }
        }

        protected class OnTriggerStay2DBehaviour : XOR.Behaviour.PhysicsCollider2D
        {

            private void OnTriggerStay2D(UnityEngine.Collider2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerStay2D, arg0);
            }
        }

        protected class OnTriggerExit2DBehaviour : XOR.Behaviour.PhysicsCollider2D
        {

            private void OnTriggerExit2D(UnityEngine.Collider2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerExit2D, arg0);
            }
        }

        protected class PhysicsCollisionBehaviour7 : XOR.Behaviour.PhysicsCollision
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

        protected class OnCollisionEnterBehaviour : XOR.Behaviour.PhysicsCollision
        {

            private void OnCollisionEnter(UnityEngine.Collision arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision.OnCollisionEnter, arg0);
            }
        }

        protected class OnCollisionStayBehaviour : XOR.Behaviour.PhysicsCollision
        {

            private void OnCollisionStay(UnityEngine.Collision arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision.OnCollisionStay, arg0);
            }
        }

        protected class OnCollisionExitBehaviour : XOR.Behaviour.PhysicsCollision
        {

            private void OnCollisionExit(UnityEngine.Collision arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision.OnCollisionExit, arg0);
            }
        }

        protected class PhysicsCollision2DBehaviour7 : XOR.Behaviour.PhysicsCollision2D
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

        protected class OnCollisionEnter2DBehaviour : XOR.Behaviour.PhysicsCollision2D
        {

            private void OnCollisionEnter2D(UnityEngine.Collision2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionEnter2D, arg0);
            }
        }

        protected class OnCollisionStay2DBehaviour : XOR.Behaviour.PhysicsCollision2D
        {

            private void OnCollisionStay2D(UnityEngine.Collision2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionStay2D, arg0);
            }
        }

        protected class OnCollisionExit2DBehaviour : XOR.Behaviour.PhysicsCollision2D
        {

            private void OnCollisionExit2D(UnityEngine.Collision2D arg0)
            {
                Invoke(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionExit2D, arg0);
            }
        }

        public static void Register()
        {

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
            XOR.Behaviour.Factory.Register<MouseBehaviour127>(XOR.Behaviour.Args.Mouse.OnMouseDown | XOR.Behaviour.Args.Mouse.OnMouseDrag | XOR.Behaviour.Args.Mouse.OnMouseEnter | XOR.Behaviour.Args.Mouse.OnMouseExit | XOR.Behaviour.Args.Mouse.OnMouseOver | XOR.Behaviour.Args.Mouse.OnMouseUpAsButton | XOR.Behaviour.Args.Mouse.OnMouseUp);
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
            XOR.Behaviour.Factory.Register<PhysicsColliderBehaviour7>(XOR.Behaviour.Args.PhysicsCollider.OnTriggerEnter | XOR.Behaviour.Args.PhysicsCollider.OnTriggerStay | XOR.Behaviour.Args.PhysicsCollider.OnTriggerExit);
            XOR.Behaviour.Factory.Register<OnTriggerEnterBehaviour>(XOR.Behaviour.Args.PhysicsCollider.OnTriggerEnter);
            XOR.Behaviour.Factory.Register<OnTriggerStayBehaviour>(XOR.Behaviour.Args.PhysicsCollider.OnTriggerStay);
            XOR.Behaviour.Factory.Register<OnTriggerExitBehaviour>(XOR.Behaviour.Args.PhysicsCollider.OnTriggerExit);
            XOR.Behaviour.Factory.Register<PhysicsCollider2DBehaviour7>(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerEnter2D | XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerStay2D | XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerExit2D);
            XOR.Behaviour.Factory.Register<OnTriggerEnter2DBehaviour>(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerEnter2D);
            XOR.Behaviour.Factory.Register<OnTriggerStay2DBehaviour>(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerStay2D);
            XOR.Behaviour.Factory.Register<OnTriggerExit2DBehaviour>(XOR.Behaviour.Args.PhysicsCollider2D.OnTriggerExit2D);
            XOR.Behaviour.Factory.Register<PhysicsCollisionBehaviour7>(XOR.Behaviour.Args.PhysicsCollision.OnCollisionEnter | XOR.Behaviour.Args.PhysicsCollision.OnCollisionStay | XOR.Behaviour.Args.PhysicsCollision.OnCollisionExit);
            XOR.Behaviour.Factory.Register<OnCollisionEnterBehaviour>(XOR.Behaviour.Args.PhysicsCollision.OnCollisionEnter);
            XOR.Behaviour.Factory.Register<OnCollisionStayBehaviour>(XOR.Behaviour.Args.PhysicsCollision.OnCollisionStay);
            XOR.Behaviour.Factory.Register<OnCollisionExitBehaviour>(XOR.Behaviour.Args.PhysicsCollision.OnCollisionExit);
            XOR.Behaviour.Factory.Register<PhysicsCollision2DBehaviour7>(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionEnter2D | XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionStay2D | XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionExit2D);
            XOR.Behaviour.Factory.Register<OnCollisionEnter2DBehaviour>(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionEnter2D);
            XOR.Behaviour.Factory.Register<OnCollisionStay2DBehaviour>(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionStay2D);
            XOR.Behaviour.Factory.Register<OnCollisionExit2DBehaviour>(XOR.Behaviour.Args.PhysicsCollision2D.OnCollisionExit2D);
        }

    }
}