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

        public Type ExplicitValueType { get; set; }
        public Tuple<float, float> ExplicitValueRange { get; set; }
        public Dictionary<string, object> ExplicitValueEnum { get; set; }

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
            this.ValueType = pairType.GetField("value", Helper.Flags).FieldType;
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
        public Type Type { get; private set; }
        public Dictionary<string, FieldWrap> FieldMapping { get; private set; }

        public SerializedProperty GetProperty(string propertyName)
        {
            return Root != null ? Root.FindProperty(propertyName) : null;
        }
        public static SerializedObjectWrap Create(SerializedObject root, Type type)
        {
            return new SerializedObjectWrap()
            {
                Root = root,
                Type = type,
                FieldMapping = Helper.GetFieldMapping(type),
            };
        }
    }

    internal class ElementWrap
    {
        public Type Type { get; private set; }
        public Type ValueType { get { return ValueField.FieldType; } }
        public FieldInfo IndexField { get; private set; }
        public FieldInfo KeyField { get; private set; }
        public FieldInfo ValueField { get; private set; }

        public int GetIndex(object obj)
        {
            return (int)IndexField.GetValue(obj);
        }
        public void SetIndex(object obj, int value)
        {
            IndexField.SetValue(obj, value);
        }
        public string GetKey(object obj)
        {
            return (string)KeyField.GetValue(obj);
        }
        public void SetKey(object obj, string value)
        {
            KeyField.SetValue(obj, value);
        }
        public object GetValue(object obj)
        {
            return KeyField.GetValue(obj);
        }
        public void SetValue(object obj, object value)
        {
            KeyField.SetValue(obj, value);
        }
        public object CreateInstance()
        {
            return System.Activator.CreateInstance(Type);
        }
        public static ElementWrap From(Type type)
        {
            if (!typeof(XOR.Serializables.IPair).IsAssignableFrom(type))
                return null;
            FieldInfo indexField = type.GetField("index");
            if (indexField == null || indexField.FieldType != typeof(int))
                return null;
            FieldInfo keyField = type.GetField("key");
            if (keyField == null || keyField.FieldType != typeof(string))
                return null;
            FieldInfo valueField = type.GetField("value");
            if (valueField == null)
                return null;
            return new ElementWrap()
            {
                Type = type,
                IndexField = indexField,
                KeyField = keyField,
                ValueField = valueField,
            };
        }
    }
    internal class FieldWrap
    {
        public FieldInfo Field { get; private set; }
        public Type FieldType { get; private set; }
        public ElementWrap Element { get; private set; }

        public IEnumerable<IPair> GetValue(object component)
        {
            return Field.GetValue(component) as IEnumerable<IPair>;
        }
        public void SetValue(object component, IEnumerable<IPair> values)
        {
            if (FieldType.IsArray)
            {
                Array array = values.ToArray();
                if (!FieldType.IsAssignableFrom(array.GetType()))
                {
                    Array newArray = Array.CreateInstance(Element.Type, array.Length);
                    Array.Copy(array, newArray, array.Length);
                    array = newArray;
                }
                Field.SetValue(component, array);
            }
            else
            {
                Debug.LogWarning($"Unsupport FieldType: ${Field.DeclaringType.FullName}.{Field.Name}");
            }
        }
        public void AppendElement(object component, object elementObj)
        {
            var values = this.GetValue(component);
            var length = values != null ? values.Count() : 0;

            Array newValues = Array.CreateInstance(Element.Type, length + 1);
            newValues.SetValue(elementObj, newValues.Length - 1);
            if (length > 0)
            {
                Array.Copy(values.ToArray(), newValues, length);
            }

            this.SetValue(component, newValues as IEnumerable<IPair>);
        }
        public static FieldWrap From(FieldInfo field)
        {
            Type elementType = field.FieldType.IsArray ? field.FieldType.GetElementType() : null;
            if (elementType == null)
                return null;
            ElementWrap element = ElementWrap.From(elementType);
            if (element == null)
                return null;
            return new FieldWrap()
            {
                Field = field,
                FieldType = field.FieldType,
                Element = element
            };
        }
    }
    internal class ComponentWrap<TComponent>
        where TComponent : UnityEngine.Component
    {
        public Type Type { get; private set; }
        public Dictionary<string, FieldWrap> FieldMapping { get; private set; }

        public IPair[] GetProperties(TComponent component)
        {
            List<IPair> results = new List<IPair>();
            foreach (FieldWrap fw in FieldMapping.Values)
            {
                var fv = fw.GetValue(component);
                if (fv == null)
                    continue;
                results.AddRange(fv);
            }
            return results.ToArray();
        }
        public void ClearProperties(TComponent component)
        {
            foreach (FieldWrap fw in FieldMapping.Values)
            {
                fw.SetValue(component, new IPair[0]);
            }
        }
        public IPair GetProperty(TComponent component, string key)
        {
            FindProperty(component, key, out var value);
            return value;
        }
        public bool AddProperty(TComponent component, Type valueType, string key, int index)
        {
            foreach (var fw in FieldMapping.Values)
            {
                if (fw.Element.ValueType.IsAssignableFrom(valueType))
                {
                    AddProperty(fw, component, key, index);
                    return true;
                }
            }
            foreach (var fw in FieldMapping.Values)
            {
                if (Helper.IsImplicitAssignable(fw.Element, valueType))
                {
                    AddProperty(fw, component, key, index);
                }
            }
            return false;
        }
        public bool RemoveProperty(TComponent component, params string[] keys)
        {
            foreach (string key in keys)
            {
                FindProperty(component, key, out var value, out var field, out var values);
                if (field == null || values == null)
                    continue;
                field.SetValue(component, values.Where(v => v != value).ToArray());
            }
            return false;
        }
        public bool SetPropertyIndex(TComponent component, string key, int newIndex)
        {
            FindProperty(component, key, out var value, out var field, out var values);
            if (value == null || field == null || values == null)
                return false;
            field.Element.SetIndex(value, newIndex);
            return true;
        }
        public bool SetPropertyKey(TComponent component, string key, string newKey)
        {
            FindProperty(component, key, out var value, out var field, out var values);
            if (value == null || field == null || values == null)
                return false;
            field.Element.SetKey(value, newKey);
            return true;
        }

        void FindProperty(TComponent component, string key, out IPair value)
        {
            FindProperty(component, key, out value, out var field, out var values);
        }
        void FindProperty(TComponent component, string key,
            out IPair value,
            out FieldWrap field,
            out IEnumerable<IPair> values
        )
        {
            foreach (var fw in FieldMapping.Values)
            {
                values = fw.GetValue(component);
                if (values == null)
                    continue;
                value = values.FirstOrDefault(v => v.Key == key);
                if (value != null)
                {
                    field = fw;
                    return;
                }
            }
            value = null;
            field = null;
            values = null;
        }
        void AddProperty(FieldWrap fieldWrap, TComponent component, string key, int index)
        {
            object elementObj = fieldWrap.Element.CreateInstance();
            fieldWrap.Element.SetIndex(elementObj, index);
            fieldWrap.Element.SetKey(elementObj, key);
            fieldWrap.AppendElement(component, elementObj);
        }
        public static ComponentWrap<TComponent> Create()
        {
            return new ComponentWrap<TComponent>()
            {
                Type = typeof(TComponent),
                FieldMapping = Helper.GetFieldMapping(typeof(TComponent)),
            };
        }
    }


    static class Helper
    {
        public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public static Dictionary<string, FieldWrap> GetFieldMapping(Type type)
        {
            Dictionary<string, FieldWrap> fieldMapping = new Dictionary<string, FieldWrap>();

            FieldInfo[] fields = type.GetFields(Flags);
            foreach (FieldInfo field in fields)
            {
                FieldWrap fw = FieldWrap.From(field);
                if (fw == null)
                    continue;
                fieldMapping.Add(field.Name, fw);
            }
            return fieldMapping;
        }

        /// <summary>
        /// 是否允许隐式分配
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool IsImplicitAssignable(ElementWrap element, Type valueType)
        {
            ImplicitAttribute implicitTypes = element.Type.GetCustomAttribute<ImplicitAttribute>(false);
            if (implicitTypes != null && implicitTypes.Types.Contains(valueType))
            {
                return true;
            }
            if (element.ValueType.IsArray && valueType.IsArray)
            {
                return element.ValueType.GetElementType().IsAssignableFrom(valueType.GetElementType()) ||
                    implicitTypes != null && implicitTypes.Types.Contains(valueType.GetElementType());
            }
            return false;
        }
    }
}
namespace XOR.Serializables.TsComponent
{
    internal class Display
    {
        private readonly Dictionary<Type, Renderer> renderers;
        private Renderer unknown;
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
        public bool Render(NodeWrap node)
        {
            Renderer renderer = null;
            if (!this.renderers.TryGetValue(node.PairType, out renderer))
            {
                renderer = unknown;
            }
            if (renderer != null)
            {
                renderer.Dirty = false;
                renderer.Node = node;
                renderer.Render();
                return renderer.Dirty;
            }
            return false;
        }

        public static Display Create()
        {
            Assembly assembly = typeof(Display).Assembly;
            Type[] rendererTypes = (from type in assembly.GetTypes()
                                    where !type.IsAbstract && typeof(Renderer).IsAssignableFrom(type)
                                    select type).ToArray();
            Display display = new Display();
            display.unknown = new UnknownRenderer();
            foreach (var type in rendererTypes)
            {
                var target = type.GetCustomAttribute<RenderTargetAttribute>(false);
                if (target == null)
                    continue;
                var renderer = System.Activator.CreateInstance(type) as Renderer;
                display.renderers.Add(target.Type, renderer);
            }
            return display;
        }
    }

    internal abstract class Renderer
    {
        /// <summary>
        /// 节点值是否被修改
        /// </summary>
        public bool Dirty { get; set; }
        public NodeWrap Node { get; set; }
        public virtual void Render()
        {
            EditorGUILayout.BeginHorizontal();
            //Node.Key = EditorGUILayout.TextField(Node.Key);
            if (Node.ExplicitValueType != null)
            {
                GUILayout.Label(new GUIContent(Node.Key, $"{Node.Key}: {Node.ExplicitValueType.FullName}"), GUILayout.Width(100f));
            }
            else
            {
                GUILayout.Label(new GUIContent(Node.Key, Node.Key), GUILayout.Width(100f));
            }
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

    internal class UnknownRenderer : Renderer
    {
        protected override void RenderValue()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                if (Node.ExplicitValueType != null)
                {
                    GUILayout.Label(new GUIContent(Node.ExplicitValueType.FullName, $"不受支持的类型: {Node.ExplicitValueType.FullName}"), GUI.skin.textField);
                }
                else
                {
                    GUILayout.Label(new GUIContent("UNKNOWN", "不受支持的类型"), GUI.skin.textField);
                }
            }
        }
    }

    [RenderTarget(typeof(XOR.Serializables.Number))]
    internal class NumberRenderer : Renderer
    {
        static readonly HashSet<Type> IntegerTypes = new HashSet<Type>()
        {
            typeof(byte),
            typeof(sbyte),
            typeof(System.Char),
            typeof(System.Int16),
            typeof(System.UInt16),
            typeof(System.Int32),
            typeof(System.UInt32),
        };
        static readonly Dictionary<Type, Tuple<long, long>> IntegerRanges = new Dictionary<Type, Tuple<long, long>>()
        {
            { typeof(byte), new Tuple<long, long>(byte.MinValue, byte.MaxValue) },
            { typeof(sbyte), new Tuple<long, long>(sbyte.MinValue, sbyte.MaxValue) },
            { typeof(System.Char), new Tuple<long, long>(System.Char.MinValue, System.Char.MaxValue) },
            { typeof(System.Int16), new Tuple<long, long>(System.Int16.MinValue, System.Int16.MaxValue) },
            { typeof(System.UInt16), new Tuple<long, long>(System.UInt16.MinValue, System.UInt16.MaxValue) },
            { typeof(System.Int32), new Tuple<long, long>(System.Int32.MinValue, System.Int32.MaxValue) },
            { typeof(System.UInt32), new Tuple<long, long>(System.UInt32.MinValue, System.UInt32.MaxValue) },
        };

        protected override void RenderValue()
        {
            if (Node.ExplicitValueEnum != null)
            {
                RendererEnumValue();
            }
            else if (Node.ExplicitValueType != null && IntegerTypes.Contains(Node.ExplicitValueType))
            {
                RenderIntegerValue();
            }
            else
            {
                RenderFloatValue();
            }
        }
        protected virtual void RendererEnumValue()
        {
            var value = (int)Node.ValueNode.doubleValue;
            var newValue = EditorGUILayout.IntPopup(value, Node.ExplicitValueEnum.Keys.ToArray(), Node.ExplicitValueEnum.Values.Cast<int>().ToArray());
            if (newValue != value || Math.Abs(newValue - Node.ValueNode.doubleValue) > float.Epsilon)
            {
                Node.ValueNode.doubleValue = newValue;
                Dirty |= true;
            }
        }
        protected virtual void RenderIntegerValue()
        {
            var value = (int)Node.ValueNode.doubleValue;
            var newValue = Node.ExplicitValueRange != null ?
                EditorGUILayout.IntSlider(value, (int)Node.ExplicitValueRange.Item1, (int)Node.ExplicitValueRange.Item2) :
                EditorGUILayout.IntField(value);
            if (IntegerRanges.TryGetValue(Node.ExplicitValueType, out Tuple<long, long> range))
            {
                if (newValue < range.Item1) newValue = (int)range.Item1;
                else if (newValue > range.Item2) newValue = (int)range.Item2;
            }
            if (newValue != value || Math.Abs(newValue - Node.ValueNode.doubleValue) > float.Epsilon)
            {
                Node.ValueNode.doubleValue = newValue;
                Dirty |= true;
            }
        }
        protected virtual void RenderFloatValue()
        {
            var value = (float)Node.ValueNode.doubleValue;
            var newValue = Node.ExplicitValueRange != null ?
                EditorGUILayout.Slider(value, Node.ExplicitValueRange.Item1, Node.ExplicitValueRange.Item2) :
                EditorGUILayout.DoubleField(value);
            if (newValue != value || Math.Abs(newValue - Node.ValueNode.doubleValue) > float.Epsilon)
            {
                Node.ValueNode.doubleValue = newValue;
                Dirty |= true;
            }
        }
    }

    [RenderTarget(typeof(XOR.Serializables.Bigint))]
    internal class BigintRenderer : Renderer
    {
        protected override void RenderValue()
        {
            var value = Node.ValueNode.longValue;
            var newValue = Node.ExplicitValueRange != null ?
                EditorGUILayout.IntSlider((int)value, (int)Node.ExplicitValueRange.Item1, (int)Node.ExplicitValueRange.Item2) :
                EditorGUILayout.LongField(value);
            if (newValue != value)
            {
                Node.ValueNode.longValue = newValue;
                Dirty |= true;
            }
        }
    }

    [RenderTarget(typeof(XOR.Serializables.String))]
    internal class StringRenderer : Renderer
    {
        protected override void RenderValue()
        {
            if (Node.ExplicitValueEnum != null)
            {
                RendererEnumValue();
            }
            else
            {
                RendererStringValue();
            }
        }
        protected virtual void RendererStringValue()
        {
            var value = Node.ValueNode.stringValue;
            var newValue = EditorGUILayout.TextField(value);
            if (newValue != value)
            {
                Node.ValueNode.stringValue = newValue;
                Dirty |= true;
            }
        }
        protected virtual void RendererEnumValue()
        {
            var value = Node.ValueNode.stringValue;
            var enumOptions = Node.ExplicitValueEnum.Keys.ToArray();
            var valueIndex = Array.IndexOf(enumOptions, value);
            var newIndex = EditorGUILayout.Popup(valueIndex, enumOptions);
            if (newIndex != valueIndex)
            {
                Node.ValueNode.stringValue = Node.ExplicitValueEnum.Values.ToArray()[newIndex] as string;
                Dirty |= true;
            }
        }
    }

    [RenderTarget(typeof(XOR.Serializables.Boolean))]
    internal class BooleanRenderer : Renderer
    {
        protected override void RenderValue()
        {
            var newValue = EditorGUILayout.Toggle(Node.ValueNode.boolValue);
            if (newValue != Node.ValueNode.boolValue)
            {
                Node.ValueNode.boolValue = newValue;
                Dirty |= true;
            }
        }
    }

    [RenderTarget(typeof(XOR.Serializables.Vector2))]
    internal class Vector2Renderer : Renderer
    {
        protected override void RenderValue()
        {
            var newValue = EditorGUILayout.Vector2Field(string.Empty, Node.ValueNode.vector2Value);
            if (newValue != Node.ValueNode.vector2Value)
            {
                Node.ValueNode.vector2Value = newValue;
                Dirty |= true;
            }
        }
    }

    [RenderTarget(typeof(XOR.Serializables.Vector3))]
    internal class Vector3Renderer : Renderer
    {
        protected override void RenderValue()
        {
            var newValue = EditorGUILayout.Vector3Field(string.Empty, Node.ValueNode.vector3Value);
            if (newValue != Node.ValueNode.vector3Value)
            {
                Node.ValueNode.vector3Value = newValue;
                Dirty |= true;
            }
        }
    }

    [RenderTarget(typeof(XOR.Serializables.Object))]
    internal class ObjectRenderer : Renderer
    {
        protected override void RenderValue()
        {
            var newValue = EditorGUILayout.ObjectField(string.Empty, Node.ValueNode.objectReferenceValue, Node.ExplicitValueType ?? typeof(UnityEngine.Object), true);
            if (newValue != Node.ValueNode.objectReferenceValue)
            {
                Node.ValueNode.objectReferenceValue = newValue;
                Dirty |= true;
            }
        }
    }

    /// <summary>
    /// 定义为目标数据的的渲染器
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal class RenderTargetAttribute : Attribute
    {
        public Type Type { get; private set; }
        public RenderTargetAttribute(Type type)
        {
            this.Type = type;
        }
    }
}