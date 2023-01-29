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

    internal class SerializedObjectWrap
    {
        public SerializedObject Root { get; private set; }
        public Dictionary<string, Type> TypeMapping { get; private set; }

        public SerializedProperty GetProperty(string propertyName)
        {
            return Root != null ? Root.FindProperty(propertyName) : null;
        }

        public static SerializedObjectWrap Create(SerializedObject componentRoot, Type componentType)
        {
            Dictionary<string, Type> typeMapping = new Dictionary<string, Type>();

            FieldInfo[] fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                Type valueElementType = field.FieldType.IsArray ? field.FieldType.GetElementType() : null;
                if (
                    valueElementType == null ||
                    componentRoot.FindProperty(field.Name) == null ||
                    !IsVaildField(valueElementType)
                )
                {
                    continue;
                }
                typeMapping.Add(field.Name, valueElementType);
            }

            return new SerializedObjectWrap()
            {
                Root = componentRoot,
                TypeMapping = typeMapping,
            };
        }
        private static bool IsVaildField(Type type)
        {
            FieldInfo index, key;
            return typeof(XOR.Serializables.IPair).IsAssignableFrom(type)
                && (index = type.GetField("index")) != null && index.FieldType == typeof(int)
                && (key = type.GetField("key")) != null && key.FieldType == typeof(string)
                && type.GetField("value") != null;
        }
    }
}
namespace XOR.Serializables.TsComponent
{
    internal class Display
    {
        private Dictionary<Type, Renderer> renderers;
        public Display()
        {
            this.renderers = new Dictionary<Type, Renderer>();
        }
        public void AddRenderer<TRenderer>(Type valueType)
            where TRenderer : Renderer, new()
        {
            var renderer = new TRenderer();
            this.renderers.Add(valueType, renderer);
        }
        public void Render(NodeWrap node)
        {
            Renderer renderer = null;
            this.renderers.TryGetValue(node.PairType, out renderer);
            if (renderer != null)
            {
                renderer.Node = node;
                renderer.Render();
            }
        }

        public static Display Create()
        {
            var display = new Display();
            display.AddRenderer<StringRenderer>(typeof(XOR.Serializables.String));
            display.AddRenderer<NumberRenderer>(typeof(XOR.Serializables.Number));
            display.AddRenderer<BooleanRenderer>(typeof(XOR.Serializables.Boolean));
            display.AddRenderer<Vector2Renderer>(typeof(XOR.Serializables.Vector2));
            display.AddRenderer<Vector3Renderer>(typeof(XOR.Serializables.Vector3));
            display.AddRenderer<ObjectRenderer>(typeof(XOR.Serializables.Object));
            return display;
        }
    }

    internal abstract class Renderer
    {
        public NodeWrap Node { get; set; }
        public virtual void Render()
        {
            EditorGUILayout.BeginHorizontal();
            Node.Key = EditorGUILayout.TextField(Node.Key);
            RenderValue();
            EditorGUILayout.EndHorizontal();
        }
        protected abstract void RenderValue();
    }
    internal abstract class ArrayRenderer : Renderer
    {
        public override void Render()
        {
            var arrayParent = Node.ValueNode;
            var arrayTypeName = Node.ValueType.Name;
            if (arrayTypeName.EndsWith("[]"))
            {
                arrayTypeName = arrayTypeName.Replace("[]", $"[{arrayParent.arraySize}]");
            }

            EditorGUILayout.BeginHorizontal();
            Node.Key = EditorGUILayout.TextField(Node.Key);
            arrayParent.isExpanded = EditorGUILayout.Foldout(arrayParent.isExpanded, arrayTypeName);
            EditorGUILayout.EndHorizontal();

            if (arrayParent.isExpanded)
            {
                RenderValue();
                RenderMenu();
            }
        }
        protected override void RenderValue()
        {
            var arrayParent = Node.ValueNode;
            for (int i = 0; i < arrayParent.arraySize; i++)
            {

            }
        }
        protected abstract void RenderElemnetValue(SerializedProperty node, Type type);

        private void RenderMenu()
        {

        }
    }

    internal class NumberRenderer : Renderer
    {
        protected override void RenderValue()
        {
            Node.ValueNode.doubleValue = EditorGUILayout.DoubleField(Node.ValueNode.doubleValue);
        }
    }
    internal class StringRenderer : Renderer
    {
        protected override void RenderValue()
        {
            Node.ValueNode.stringValue = EditorGUILayout.TextField(Node.ValueNode.stringValue);
        }
    }
    internal class BooleanRenderer : Renderer
    {
        protected override void RenderValue()
        {
            Node.ValueNode.boolValue = EditorGUILayout.Toggle(Node.ValueNode.boolValue);
        }
    }
    internal class Vector2Renderer : Renderer
    {
        protected override void RenderValue()
        {
            Node.ValueNode.vector2Value = EditorGUILayout.Vector2Field(string.Empty, Node.ValueNode.vector2Value);
        }
    }
    internal class Vector3Renderer : Renderer
    {
        protected override void RenderValue()
        {
            Node.ValueNode.vector3Value = EditorGUILayout.Vector3Field(string.Empty, Node.ValueNode.vector3Value);
        }
    }
    internal class ObjectRenderer : Renderer
    {
        protected override void RenderValue()
        {
            Node.ValueNode.objectReferenceValue = EditorGUILayout.ObjectField(string.Empty, Node.ValueNode.objectReferenceValue, typeof(Object), true);
        }
    }
}
