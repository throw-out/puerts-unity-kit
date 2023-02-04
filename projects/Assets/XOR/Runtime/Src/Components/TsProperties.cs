using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{
    [DisallowMultipleComponent]
    public partial class TsProperties : MonoBehaviour, XOR.Serializables.IAccessor
    {
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

        private Action<string, object> onPropertyChange;
        public XOR.Serializables.ResultPair[] GetProperties()
        {
            return XOR.Serializables.Accessor<TsProperties>.GetProperties(this)
                ?.Where(o => o != null)
                .Select(o => new XOR.Serializables.ResultPair(o))
                .ToArray();
        }
        public void SetProperty(string key, object value)
        {
#if UNITY_EDITOR
            XOR.Serializables.Accessor<TsProperties>.SetProperty(this, key, value);
#endif
        }
        public void SetPropertyListener(Action<string, object> handler)
        {
#if UNITY_EDITOR
            this.onPropertyChange = handler;
#endif
        }
    }
}