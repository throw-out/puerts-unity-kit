using System;
using UnityEngine;

namespace XOR
{
    using XOR.Serializables;

    public partial class TsComponent : MonoBehaviour
    {
        [SerializeField]
        protected string guid;
        [SerializeField]
        protected string route;
        [SerializeField]
        protected string version;

        #region 序列化字段
        [SerializeField]
        private String[] StringPairs;
        [SerializeField]
        private Number[] NumberPairs;
        [SerializeField]
        private Boolean[] BooleanPairs;
        [SerializeField]
        private Vector2[] Vector2Pairs;
        [SerializeField]
        private Vector3[] Vector3Pairs;
        [SerializeField]
        private Object[] ObjectPairs;

        [SerializeField]
        private StringArray[] StringArrayPairs;
        [SerializeField]
        private DoubleArray[] DoubleArrayPairs;
        [SerializeField]
        private BooleanArray[] BooleanArrayPairs;
        [SerializeField]
        private Vector2Array[] Vector2ArrayPairs;
        [SerializeField]
        private Vector3Array[] Vector3ArrayPairs;
        [SerializeField]
        private ObjectArray[] ObjectArrayPairs;
        #endregion
    }
}
