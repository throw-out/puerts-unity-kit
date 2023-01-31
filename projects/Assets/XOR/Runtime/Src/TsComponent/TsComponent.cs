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
    }
}
