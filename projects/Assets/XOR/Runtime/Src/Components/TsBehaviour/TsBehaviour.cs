using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XOR
{
    //[DisallowMultipleComponent]
    public class TsBehaviour : MonoBehaviour, IDisposable
    {
        private static HashSet<WeakReference<TsBehaviour>> referenceInstances;
        private WeakReference<TsBehaviour> referenceSelf;
        #region Editor 
#if UNITY_EDITOR
        public ModuleInfo Module { get; set; }
#endif
        #endregion

        //Unity 接口组件
        private Dictionary<string, Proxy> proxies;
        private Action awakeCallback;       //Awake回调
        private Action startCallback;       //Start回调
        private Action destroyCallback;     //OnDestroy回调
        private Action enableCallback;      //OnEnable回调
        private Action disableCallback;     //OnDisable回调
        public bool IsActivated { get; private set; } = false;      //是否已执行Awake回调
        public bool IsStarted { get; private set; } = false;        //是否已执行Start回调
        public bool IsDestroyed { get; private set; } = false;      //是否已执行OnDestroy回调
        public bool IsEnable { get; private set; } = false;

        public TsBehaviour()
        {
            if (referenceInstances == null)
                referenceInstances = new HashSet<WeakReference<TsBehaviour>>();

            referenceSelf = new WeakReference<TsBehaviour>(this);
            referenceInstances.Add(referenceSelf);
        }
        ~TsBehaviour()
        {
            if (referenceInstances != null)
            {
                referenceInstances.Remove(referenceSelf);
            }
            this.Dispose(false);
        }
        protected virtual void Awake()
        {
            IsActivated = true;
            awakeCallback?.Invoke();
            awakeCallback = null;
        }
        protected virtual void Start()
        {
            IsStarted = true;
            startCallback?.Invoke();
            startCallback = null;
        }
        protected virtual void OnEnable()
        {
            IsEnable = true;
            enableCallback?.Invoke();
            if (proxies != null)
            {
                foreach (var proxy in proxies)
                {
                    proxy.Value.enabled = true;
                }
            }
        }
        protected virtual void OnDisable()
        {
            IsEnable = false;
            disableCallback?.Invoke();
            if (proxies != null)
            {
                foreach (var proxy in proxies)
                {
                    proxy.Value.enabled = false;
                }
            }
        }
        protected virtual void OnDestroy()
        {
            IsDestroyed = true;
            destroyCallback?.Invoke();
            destroyCallback = null;
            Dispose(true);
        }

        public bool RemoveProxy(string name)
        {
            Proxy proxy = null;
            if (proxies != null && proxies.TryGetValue(name, out proxy))
            {
                proxies.Remove(name);
                proxy.Release();
                Destroy(proxy);
            }
            return proxy != null;
        }
        public void AddProxy(string name, Proxy proxy)
        {
            if (RemoveProxy(name))
                Debug.LogWarning(string.Format("Repeat add proxy: ", this.name, name));
            if (proxies == null)
            {
                proxies = new Dictionary<string, Proxy>();
            }
            proxies.Add(name, proxy);
        }
        public Proxy GetProxy(string name)
        {
            Proxy proxy = null;
            if (this.proxies != null)
                this.proxies.TryGetValue(name, out proxy);
            return proxy;
        }

        public ProxyAction CreateProxy(string name, Action callback)
        {
            ProxyAction proxy = this.GetProxy(name) as ProxyAction;
            if (proxy != null)
            {
                proxy.callback += callback;
                return proxy;
            }
            switch (name)
            {
                case "Awake":
                    if (this.IsActivated)
                        callback?.Invoke();
                    else
                        this.awakeCallback += callback;
                    break;
                case "Start":
                    if (this.IsStarted)
                        callback?.Invoke();
                    else
                        this.startCallback += callback;
                    break;
                case "OnDestroy":
                    if (this.IsDestroyed)
                        callback?.Invoke();
                    else
                        this.destroyCallback += callback;
                    break;
                case "OnEnable":
                    //proxy = gameObject.AddComponent<OnEnableProxy>();
                    this.enableCallback += callback;
                    if (this.IsEnable && !this.IsDestroyed)
                        callback?.Invoke();
                    break;
                case "OnDisable":
                    //proxy = gameObject.AddComponent<OnDisableProxy>();
                    this.disableCallback += callback;
                    if (!this.IsEnable && !this.IsDestroyed)
                        callback?.Invoke();
                    break;
                case "Update":
                    proxy = gameObject.AddComponent<UpdateProxy>();
                    break;
                case "FixedUpdate":
                    proxy = gameObject.AddComponent<FixedUpdateProxy>();
                    break;
                case "LateUpdate":
                    proxy = gameObject.AddComponent<LateUpdateProxy>();
                    break;
                case "OnApplicationQuit":
                    proxy = gameObject.AddComponent<OnApplicationQuitProxy>();
                    break;
                case "OnGUI":
                    proxy = gameObject.AddComponent<OnGUIProxy>();
                    break;

                case "OnDrawGizmosSelected":
                    proxy = gameObject.AddComponent<OnDrawGizmosSelectedProxy>();
                    break;
                case "OnSceneGUI":
                    proxy = gameObject.AddComponent<OnSceneGUIProxy>();
                    break;

                case "OnMouseDown":
                    proxy = gameObject.AddComponent<OnMouseDownProxy>();
                    break;
                case "OnMouseDrag":
                    proxy = gameObject.AddComponent<OnMouseDragProxy>();
                    break;
                case "OnMouseEnter":
                    proxy = gameObject.AddComponent<OnMouseEnterProxy>();
                    break;
                case "OnMouseExit":
                    proxy = gameObject.AddComponent<OnMouseExitProxy>();
                    break;
                case "OnMouseOver":
                    proxy = gameObject.AddComponent<OnMouseOverProxy>();
                    break;
                case "OnMouseUp":
                    proxy = gameObject.AddComponent<OnMouseUpProxy>();
                    break;
                case "OnMouseUpAsButton":
                    proxy = gameObject.AddComponent<OnMouseUpAsButtonProxy>();
                    break;
            }
            if (proxy != null)
            {
                proxy.callback = callback;
                AddProxy(name, proxy);
            }
            return proxy;
        }
        public ProxyAction<bool> CreateProxyForBool(string name, Action<bool> callback)
        {
            ProxyAction<bool> proxy = this.GetProxy(name) as ProxyAction<bool>;
            if (proxy != null)
            {
                proxy.callback += callback;
                return proxy;
            }
            switch (name)
            {
                case "OnApplicationFocus":
                    proxy = gameObject.AddComponent<OnApplicationFocusProxy>();
                    break;
                case "OnApplicationPause":
                    proxy = gameObject.AddComponent<OnApplicationPauseProxy>();
                    break;
                case "OnBecameVisible":
                    proxy = gameObject.AddComponent<OnBecameVisibleProxy>();
                    break;
            }
            if (proxy != null)
            {
                proxy.callback = callback;
                AddProxy(name, proxy);
            }
            return proxy;
        }
        public ProxyAction<PointerEventData> CreateProxyForEventData(string name, Action<PointerEventData> callback)
        {
            ProxyAction<PointerEventData> proxy = this.GetProxy(name) as ProxyAction<PointerEventData>;
            if (proxy != null)
            {
                proxy.callback += callback;
                return proxy;
            }
            switch (name)
            {
                case "OnPointerClick":
                    proxy = gameObject.AddComponent<OnPointerClickProxy>();
                    break;
                case "OnPointerDown":
                    proxy = gameObject.AddComponent<OnPointerDownProxy>();
                    break;
                case "OnPointerEnter":
                    proxy = gameObject.AddComponent<OnPointerEnterProxy>();
                    break;
                case "OnPointerExit":
                    proxy = gameObject.AddComponent<OnPointerExitProxy>();
                    break;
                case "OnPointerUp":
                    proxy = gameObject.AddComponent<OnPointerUpProxy>();
                    break;
            }
            if (proxy != null)
            {
                proxy.callback = callback;
                AddProxy(name, proxy);
            }
            return proxy;
        }
        public OnDragProxy CreateProxyForDrag(Action<PointerEventData> enter, Action<PointerEventData> stay, Action<PointerEventData> exit, bool stayFrame = false)
        {
            const string name = "OnDrag";
            OnDragProxy proxy = this.GetProxy(name) as OnDragProxy;
            if (proxy == null)
            {
                proxy = gameObject.AddComponent<OnDragProxy>();
                AddProxy(name, proxy);
            }
            proxy.enter += enter;
            proxy.stay += stay;
            proxy.exit += exit;
            proxy.stayFrame = stayFrame;

            return proxy;
        }
        public OnCollisionProxy CreateProxyForCollision(Action<Collision> enter, Action<Collision> stay, Action<Collision> exit, bool stayFrame = false)
        {
            const string name = "OnCollision";
            OnCollisionProxy proxy = this.GetProxy(name) as OnCollisionProxy;
            if (proxy == null)
            {
                proxy = gameObject.AddComponent<OnCollisionProxy>();
                AddProxy(name, proxy);
            }
            proxy.enter += enter;
            proxy.stay += stay;
            proxy.exit += exit;
            proxy.stayFrame = stayFrame;

            return proxy;
        }
        public OnCollision2DProxy CreateProxyForCollision2D(Action<Collision2D> enter, Action<Collision2D> stay, Action<Collision2D> exit, bool stayFrame = false)
        {
            const string name = "OnCollision2D";
            OnCollision2DProxy proxy = this.GetProxy(name) as OnCollision2DProxy;
            if (proxy == null)
            {
                proxy = gameObject.AddComponent<OnCollision2DProxy>();
                AddProxy(name, proxy);
            }
            proxy.enter += enter;
            proxy.stay += stay;
            proxy.exit += exit;
            proxy.stayFrame = stayFrame;

            return proxy;
        }
        public OnTriggerProxy CreateProxyForTrigger(Action<Collider> enter, Action<Collider> stay, Action<Collider> exit, bool stayFrame = false)
        {
            const string name = "OnTrigger";
            OnTriggerProxy proxy = this.GetProxy(name) as OnTriggerProxy;
            if (proxy == null)
            {
                proxy = gameObject.AddComponent<OnTriggerProxy>();
                AddProxy(name, proxy);
            }
            proxy.enter += enter;
            proxy.stay += stay;
            proxy.exit += exit;
            proxy.stayFrame = stayFrame;

            return proxy;
        }
        public OnTrigger2DProxy CreateProxyForTrigger2D(Action<Collider2D> enter, Action<Collider2D> stay, Action<Collider2D> exit, bool stayFrame = false)
        {
            const string name = "OnTrigger2D";
            OnTrigger2DProxy proxy = this.GetProxy(name) as OnTrigger2DProxy;
            if (proxy == null)
            {
                proxy = gameObject.AddComponent<OnTrigger2DProxy>();
                AddProxy(name, proxy);
            }
            proxy.enter += enter;
            proxy.stay += stay;
            proxy.exit += exit;
            proxy.stayFrame = stayFrame;

            return proxy;
        }


        public virtual void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool destroy)
        {
            this.awakeCallback = null;
            this.startCallback = null;
            this.destroyCallback = null;
            this.enableCallback = null;
            this.disableCallback = null;
            if (proxies != null)
            {
                foreach (var proxy in proxies.Values)
                {
                    proxy.Release();
                    if (destroy) Destroy(proxy);
                }
                proxies.Clear();
            }
            proxies = null;
            GC.SuppressFinalize(this);
        }

        public static void DisposeAll()
        {
            if (referenceInstances == null)
                return;

            TsBehaviour instance;
            foreach (var weak in referenceInstances)
            {
                if (weak.TryGetTarget(out instance))
                {
                    instance.Dispose(false);
                    instance.referenceSelf = null;
                }
            }
            referenceInstances = null;
        }
    }
    public class ModuleInfo
    {
        public string className { get; set; }
        public string moduleName { get; set; }
        public string modulePath { get; set; }
        public int line { get; set; }
        public int column { get; set; }
        public string stack { get; set; }
    }
}


