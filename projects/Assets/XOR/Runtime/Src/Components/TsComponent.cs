using System;
using System.Linq;
using UnityEngine;

namespace XOR
{
    public partial class TsComponent : XOR.TsBehaviour, XOR.Serializables.IAccessor
    {
        [SerializeField]
        protected string guid;
        [SerializeField]
        protected string route;
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

        internal string GetGuid() => guid;
        internal string GetRoute() => route;

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

        private bool registered;
        private Puerts.JSObject jsObject;
        private WeakReference<GameObject> reference;

        protected override void Awake()
        {
            base.Awake();
            this.reference = TsComponentLifecycle.GetReference(gameObject);
            TsComponentLifecycle.AddComponent(this.reference, this);
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
                jsObject = value;
                registered = true;
            }
        }
        internal bool Registered { get => registered; }
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
        /// 回收无效的TsComponent实例
        /// </summary>
        public static void GC()
        {
            TsComponentLifecycle.GC();
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
    }
}
