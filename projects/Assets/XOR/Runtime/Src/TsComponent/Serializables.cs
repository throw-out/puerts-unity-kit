using System;
using UnityEngine;

namespace XOR
{
    public class ResultPair
    {
        public string key;
        public object value;
        public ResultPair(IPair pair)
        {
            this.key = pair.Key;
            this.value = pair.Value;
        }
    }
    public interface IPair
    {
        int Index { get; }
        string Key { get; }
        object Value { get; }
    }
    public class Pair<T> : IPair
    {
        public int index;
        public string key;
        public T value;

        public int Index { get { return this.index; } }
        public string Key { get { return this.key; } }
        public object Value { get { return this.value; } }
    }
    [System.Serializable]
    public class String : Pair<System.String> { }
    [System.Serializable]
    public class Number : Pair<System.Double> { }
    [System.Serializable]
    public class Boolean : Pair<System.Boolean> { }
    [System.Serializable]
    public class Vector2 : Pair<UnityEngine.Vector2> { }
    [System.Serializable]
    public class Vector3 : Pair<UnityEngine.Vector3> { }
    [System.Serializable]
    public class Object : Pair<UnityEngine.Object> { }

    [MenuPath("Array/String")]
    [System.Serializable]
    public class StringArray : Pair<System.String[]> { }
    [MenuPath("Array/Double")]
    [System.Serializable]
    public class DoubleArray : Pair<System.Double[]> { }
    [MenuPath("Array/Boolean")]
    [System.Serializable]
    public class BooleanArray : Pair<System.Boolean[]> { }
    [MenuPath("Array/Vector2")]
    [System.Serializable]
    public class Vector2Array : Pair<UnityEngine.Vector2[]> { }
    [MenuPath("Array/Vector3")]
    [System.Serializable]
    public class Vector3Array : Pair<UnityEngine.Vector3[]> { }
    [MenuPath("Array/Object")]
    [System.Serializable]
    public class ObjectArray : Pair<UnityEngine.Object[]> { }

    //MenuItem路径设置
    [AttributeUsageAttribute(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class MenuPathAttribute : Attribute
    {
        public string path { get; private set; }
        public MenuPathAttribute(string path)
        {
            this.path = path;
        }
    }
}