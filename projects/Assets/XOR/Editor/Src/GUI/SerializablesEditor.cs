using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XOR.Serializables
{


    internal class NodeWrap
    {
        public SerializedProperty Node { get; private set; }
        public SerializedProperty ArrayParent { get; private set; }
        public int ArrayIndex { get; private set; }
        private SerializedProperty _keyNode;
        private SerializedProperty _indexNode;
        private SerializedProperty _valueNode;

        public Type PairType { get; private set; }
        public Type ValueType { get; private set; }

        public SerializedProperty KeyNode
        {
            get
            {
                if (_keyNode == null) _keyNode = this.Node.FindPropertyRelative("key");
                return _keyNode;
            }
        }
        public SerializedProperty IndexNode
        {
            get
            {
                if (_indexNode == null) _indexNode = this.Node.FindPropertyRelative("index");
                return _indexNode;
            }
        }
        public SerializedProperty ValueNode
        {
            get
            {
                if (_valueNode == null) _valueNode = this.Node.FindPropertyRelative("value");
                return _valueNode;
            }
        }
        public string Key
        {
            get { return this.KeyNode.stringValue; }
            set { this.KeyNode.stringValue = value; }
        }
        public int Index
        {
            get { return this.IndexNode.intValue; }
            set { this.IndexNode.intValue = value; }
        }

        public NodeWrap(SerializedProperty node, Type pairType) : this(node, pairType, 0, null) { }
        public NodeWrap(SerializedProperty node, Type pairType, int arrayIndex, SerializedProperty arrayParent)
        {
            this.Node = node;
            this.ArrayParent = arrayParent;
            this.ArrayIndex = arrayIndex;
            this.PairType = pairType;
            this.ValueType = pairType.GetField("value").FieldType;
        }

        /// <summary>从(数组)父节点上移除当前节点 </summary>
        public void RemoveFromArrayParent()
        {
            ArrayParent.DeleteArrayElementAtIndex(ArrayIndex);
        }
        /// <summary>清除值存储的数据 </summary>
        public void CleanValue()
        {
            if (ValueNode.isArray)
            {
                for (int i = 0; i < ValueNode.arraySize; i++)
                {
                    CleanNode(ValueNode.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                CleanNode(ValueNode);
            }
        }

        static void CleanNode(SerializedProperty node)
        {
            //期望的字段名称
            var expectedPropName = (GetTypeName(node.propertyType) + "Value").ToLower();
            var property = (from _p in typeof(SerializedProperty).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            where _p.CanRead && _p.CanWrite && _p.Name.ToLower().Equals(expectedPropName)
                            select _p).FirstOrDefault();
            if (property != null)
                property.SetMethod.Invoke(node, new object[] { default });
            else
                throw new KeyNotFoundException(string.Format("expected prop name = {0}, but not found.", expectedPropName));
        }
        static string GetTypeName(SerializedPropertyType type)
        {
            switch (type)
            {
                case SerializedPropertyType.Boolean:
                    return "bool";
            }
            return Enum.GetName(typeof(SerializedPropertyType), type);
        }
    }

    internal abstract class Renderer
    {
        protected const float HEIGHT_SPACING_HALF = 2f;
        protected const float HEIGHT_SPACING = HEIGHT_SPACING_HALF * 2;
        public NodeWrap Node { get; set; }
        public virtual void Render(Rect position)
        {
            //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
            using (new EditorGUI.PropertyScope(position, GUIContent.none, Node.ArrayParent))
            {
                //设置属性名宽度 Name HP
                EditorGUIUtility.labelWidth = 0;
                //输入框高度，默认一行的高度
                position.height = this.GetLineHeight();

                float normalizeWidth = position.width, normalizeHalf = normalizeWidth / 2f;

                //输入框的位置
                Rect keyRect = new Rect(position)
                {
                    width = normalizeHalf,
                    height = position.height - HEIGHT_SPACING,
                    y = position.y + HEIGHT_SPACING_HALF
                };
                Rect valueRect = new Rect(keyRect)
                {
                    x = keyRect.x + keyRect.width + 2,
                    width = normalizeHalf,
                };
                Rect optionsRect = new Rect(valueRect)
                {
                    x = valueRect.x + valueRect.width + 2,
                };

                Node.Key = EditorGUI.TextField(keyRect, string.Empty, Node.Key);
                RenderValue(position, valueRect);
            }
        }
        protected virtual void RenderValue(Rect position, Rect valueRect)
        {
        }
        public virtual float GetLineHeight()
        {
            return EditorGUIUtility.singleLineHeight + HEIGHT_SPACING;
        }
        public virtual float GetHeight()
        {
            return this.GetLineHeight();
        }
    }

    internal class NumberRenderer : Renderer
    {
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            Node.ValueNode.doubleValue = EditorGUI.DoubleField(valueRect, string.Empty, Node.ValueNode.doubleValue);
        }
    }
    internal class StringRenderer : Renderer
    {
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            Node.ValueNode.stringValue = EditorGUI.TextField(valueRect, string.Empty, Node.ValueNode.stringValue);
        }
    }

}
