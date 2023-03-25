using System;
using System.Linq;
using UnityEngine;

namespace XOR
{
    public partial class TsComponent : XOR.TsBehaviour, XOR.Serializables.IAccessor
    {
#pragma warning disable
        [SerializeField]
        protected string guid;
        [SerializeField]
        protected string route;
        [SerializeField]
        protected string path;
        [SerializeField]
        protected string version;

        #region 序列化字段
        [SerializeField]
        private XOR.Serializables.String[] StringPairs;
        [SerializeField]
        private XOR.Serializables.Number[] NumberPairs;
        [SerializeField]
        private XOR.Serializables.Bigint[] BigintPairs;
        [SerializeField]
        private XOR.Serializables.Boolean[] BooleanPairs;
        [SerializeField]
        private XOR.Serializables.Vector2[] Vector2Pairs;
        [SerializeField]
        private XOR.Serializables.Vector3[] Vector3Pairs;
        [SerializeField]
        private XOR.Serializables.Object[] ObjectPairs;

        [SerializeField]
        private XOR.Serializables.StringArray[] StringArrayPairs;
        [SerializeField]
        private XOR.Serializables.NumberArray[] NumberArrayPairs;
        [SerializeField]
        private XOR.Serializables.BigintArray[] BigintArrayPairs;
        [SerializeField]
        private XOR.Serializables.BooleanArray[] BooleanArrayPairs;
        [SerializeField]
        private XOR.Serializables.Vector2Array[] Vector2ArrayPairs;
        [SerializeField]
        private XOR.Serializables.Vector3Array[] Vector3ArrayPairs;
        [SerializeField]
        private XOR.Serializables.ObjectArray[] ObjectArrayPairs;
        #endregion
#pragma warning restore

        private Action<string, object> onPropertyChange;
        public XOR.Serializables.ResultPair[] GetProperties()
        {
            return XOR.Serializables.Accessor<TsComponent>.GetProperties(this)
                ?.Where(o => o != null)
                .Select(o => new XOR.Serializables.ResultPair(o))
                .ToArray();
        }
        public void SetProperty(string key, object value)
        {
#if UNITY_EDITOR
            XOR.Serializables.Accessor<TsComponent>.SetProperty(this, key, value);
#endif
        }
        public void SetPropertyListener(Action<string, object> handler)
        {
#if UNITY_EDITOR
            this.onPropertyChange = handler;
#endif
        }

        private bool initialized;
        private bool registered;
        private Puerts.JSObject jsObject;
        private WeakReference<GameObject> reference;

        ~TsComponent()
        {
            this.Release();
        }
        protected override void Awake()
        {
            base.Awake();
            this.Init();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.Release();
        }
        public Puerts.JSObject JSObject
        {
            get => jsObject;
            internal set
            {
                if (!initialized) Init();
                jsObject = value;
                registered = true;
            }
        }
        internal bool Registered { get => registered; }
        internal bool Initialized { get => initialized; }
        /// <summary>
        /// 初始化并创建JSObject对象;
        /// 如果GameObject或其父节点的activeSelf一开始为false, 那么Awake就不会被执行, 直到activeSelf变为true时才会执行;
        /// 如果Awake从未执行过, 那么其对应的OnDestroty也不会执行, 此时只能通过TsComponent.GC()释放对象
        /// </summary>
        /// <returns>是否成功</returns>
        internal void Init()
        {
            if (initialized) return;
            initialized = true;
            if (reference != null || this.IsDestroyed || this == null)
                return;
            reference = TsComponentLifecycle.GetReference(gameObject);
            TsComponentLifecycle.AddComponent(reference, this);
        }
        internal void Release()
        {
            if (reference == null)
                return;
            var _reference = reference;
            reference = null;
            jsObject = null;
            if (this.gameObject == null)
            {
                TsComponentLifecycle.DestroyComponents(_reference);
            }
            else
            {
                TsComponentLifecycle.DestroyComponent(_reference, this);
            }
        }
        /// <summary>
        /// 调用JS方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        internal void InvokeMethod(string methodName, params object[] args)
        {
            if (jsObject == null || string.IsNullOrEmpty(methodName))
                return;
            TsComponentLifecycle.Invoke(jsObject, methodName, args);
        }
        public string GetGuid() => guid;
        public string GetRoute() => route;
        public string GetPath() => path;

        /// <summary>
        /// 回收无效的TsComponent实例
        /// 如果使用Object.DestroyImmediate销毁资源, 其OnDestroy不会被调用.
        /// 如果Awake方法从未调用过, 其OnDestroy也不会被调用.这通常出现在GameObject或其父节点的activeSelf一直为false
        /// </summary>
        public static void GC()
        {
            TsComponentLifecycle.GC();
        }
        /// <summary>
        /// 打印所有实例状态(先执行一次GC)
        /// </summary>
        public static void PrintStatus()
        {
            TsComponentLifecycle.GC();
            TsComponentLifecycle.PrintStatus();
        }
        /// <summary>
        /// 注册Puerts.JsEnv实例
        /// </summary>
        /// <param name="env"></param>
        public static void Register(Puerts.JsEnv env)
        {
            if (env == null)
                throw new ArgumentNullException();
            TsComponentLifecycle.Register(env);
        }
        /// <summary>
        /// 移除已注册的Puerts.JsEnv实例
        /// </summary>
        public static void Unregister()
        {
            TsComponentLifecycle.Unregister();
        }
    }
}
