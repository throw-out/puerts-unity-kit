using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{
    /// <summary>
    /// 将定义合并到XOR.TsComponent
    /// 使用Assembly Definition Reference定义合并至puerts.unity.kit.xor程序集
    /// </summary>
    public partial class TsComponent
    {
        /// <summary>
        /// 定义序列化字段
        /// </summary>
        [SerializeField]
        private Color[] ColorPairs;
        [SerializeField]
        private ColorArray[] ColorArrayPairs;
        [SerializeField]
        private MyData[] MyDataPairs;
        [SerializeField]
        private MyDataArray[] MyDataArrayPairs;

        /// <summary>
        /// 定义序列化类型
        /// </summary>
        [System.Serializable]
        public class Color : XOR.Serializables.Pair<UnityEngine.Color> { }
        [System.Serializable]
        public class ColorArray : XOR.Serializables.Pair<UnityEngine.Color[]> { }
        [System.Serializable]
        public class MyData : XOR.Serializables.Pair<global::MyData> { }
        [System.Serializable]
        public class MyDataArray : XOR.Serializables.Pair<global::MyData[]> { }
    }
}
