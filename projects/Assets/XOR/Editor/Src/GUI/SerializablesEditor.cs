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

        public Type ElementType { get; private set; }
        public Type ValueType { get; private set; }

        public Type ExplicitValueType { get; set; }
        public Tuple<float, float> ExplicitValueRange { get; set; }
        public Dictionary<string, object> ExplicitValueEnum { get; set; }
        public string Tooltip { get; set; }

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

        public NodeWrap(SerializedProperty node, Type elementType) : this(node, elementType, 0, null) { }
        public NodeWrap(SerializedProperty node, Type elementType, int arrayIndex, SerializedProperty arrayParent)
        {
            this.Node = node;
            this.ArrayParent = arrayParent;
            this.ArrayIndex = arrayIndex;
            this.ElementType = elementType;
            this.ValueType = elementType.GetField("value", Helper.Flags).FieldType;
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
            return ValueField.GetValue(obj);
        }
        public void SetValue(object obj, object value)
        {
            ValueField.SetValue(obj, value);
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
                Array array;
                if (values != null)
                {
                    array = values.ToArray();
                }
                else
                {
                    array = Array.CreateInstance(Element.Type, 0);
                }
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
                var fvs = fw.GetValue(component);
                if (fvs == null)
                    continue;
                results.AddRange(fvs);
            }
            return results.Distinct().ToArray();
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
                if (Helper.IsAssignable(fw.Element, valueType))
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
                    return true;
                }
            }
            return false;
        }
        public bool RemoveProperty(TComponent component, string key)
        {
            FindProperty(component, key, out var value, out var fw, out var values);
            if (fw == null || values == null)
                return false;
            fw.SetValue(component, values.Where(v => v != value).ToArray());
            return true;
        }
        /// <summary>当前键是否可分配至目标类型 </summary>
        public bool IsExplicitPropertyValue(TComponent component, string key, Type valueType)
        {
            FindProperty(component, key, out IPair value, out var fw, out var values);
            if (fw == null || values == null || !Helper.IsAssignable(fw.Element, valueType) && !Helper.IsImplicitAssignable(fw.Element, valueType))
                return false;
            var _value = value.Value;
            if (_value != null && Helper.IsAssignable(fw.Element, valueType))
            {
                var _vt = _value.GetType();
                if (valueType.IsArray && _vt.IsArray)
                {
                    var memberType = valueType.GetElementType();
                    var array = (Array)_value;
                    var newArray = Array.CreateInstance(fw.Element.ValueType.GetElementType(), array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        var m = array.GetValue(i);
                        if (m == null || m is UnityEngine.Object && ((UnityEngine.Object)m) == null || !memberType.IsAssignableFrom(m.GetType()))
                            continue;
                        newArray.SetValue(m, i);
                    }
                    fw.Element.SetValue(value, newArray);
                }
                else if (!valueType.IsAssignableFrom(_vt))
                {
                    fw.Element.SetValue(value, default);
                }
            }
            return true;
        }

        public bool SetPropertyIndex(TComponent component, string key, int newIndex)
        {
            FindProperty(component, key, out var value, out var fw, out var values);
            if (value == null || fw == null || values == null)
                return false;
            fw.Element.SetIndex(value, newIndex);
            return true;
        }
        public bool SetPropertyKey(TComponent component, string key, string newKey)
        {
            FindProperty(component, key, out var value, out var fw, out var values);
            if (value == null || fw == null || values == null)
                return false;
            fw.Element.SetKey(value, newKey);
            return true;
        }
        public bool SetPropertyValue(TComponent component, string key, object newValue)
        {
            FindProperty(component, key, out var value, out var fw, out var values);
            if (value == null || fw == null || values == null)
                return false;
            if (newValue == null)
            {
                fw.Element.SetValue(value, default);
            }
            else if (Helper.IsAssignable(fw.Element, newValue.GetType()))
            {
                fw.Element.SetValue(value, newValue);
            }
            else if (Helper.IsImplicitAssignable(fw.Element, newValue.GetType()))
            {
                fw.Element.SetValue(value, Helper.GetImplicitAssignableValue(fw.Element, newValue));
            }
            else
            {
                Debug.LogWarning($"Wrong Type Assignment: The target type require {fw.Element.ValueType.FullName}, but actual type is {newValue.GetType().FullName}");
                return false;
            }
            return true;
        }

        void FindProperty(TComponent component, string key, out IPair value)
        {
            FindProperty(component, key, out value, out var fw, out var values);
        }
        void FindProperty(TComponent component, string key,
            out IPair value,
            out FieldWrap fieldWrap,
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
                    fieldWrap = fw;
                    return;
                }
            }
            value = null;
            fieldWrap = null;
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
        /// 是否允许分配至指定类型
        /// </summary>
        /// <param name="element"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool IsAssignable(ElementWrap element, Type valueType)
        {
            return element.ValueType.IsAssignableFrom(valueType);
        }
        /// <summary>
        /// 是否允许隐式分配
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool IsImplicitAssignable(ElementWrap element, Type valueType)
        {
            if (element.ValueType.IsArray != valueType.IsArray)
                return false;

            Type[] implicitTypes = GetImplicitAssignableTypes(element.Type);
            if (implicitTypes != null && implicitTypes.Contains(valueType))
            {
                return true;
            }
            if (element.ValueType.IsArray)      //数组类型隐式分配
            {
                return element.ValueType.GetElementType().IsAssignableFrom(valueType.GetElementType()) ||
                    implicitTypes != null && implicitTypes.Contains(valueType.GetElementType());
            }
            return false;
        }

        static Dictionary<Type, Func<object, object>> typeConvertFuncs = new Dictionary<Type, Func<object, object>>()
        {
            { typeof(double), v => System.Convert.ToDouble(v)},
        };
        /// <summary>
        /// 进行类型强转
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetImplicitAssignableValue(ElementWrap element, object value)
        {
            if (value == null || element.ValueType.IsArray != value.GetType().IsArray)
            {
                return default;
            }
            Type valueType = element.ValueType;
            //获取转化方法
            Func<object, object> convertFunc;
            typeConvertFuncs.TryGetValue(valueType.IsArray ? valueType.GetElementType() : valueType, out convertFunc);
            if (convertFunc == null)
            {
                return default;
            }
            if (valueType.IsArray)
            {
                Array array = (Array)value;
                Array newArray = Array.CreateInstance(valueType.GetElementType(), array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    var am = array.GetValue(i);
                    if (am == null) continue;
                    newArray.SetValue(convertFunc(am), i);
                }
                return newArray;
            }
            else
            {
                return convertFunc(value);
            }
        }

        static Dictionary<Type, Type[]> cacheImplicitAssignableTypes = new Dictionary<Type, Type[]>();
        static Type[] GetImplicitAssignableTypes(Type type)
        {
            Type[] implicitTypes;
            if (!cacheImplicitAssignableTypes.TryGetValue(type, out implicitTypes))
            {
                ImplicitAttribute implicitDefine = type.GetCustomAttribute<ImplicitAttribute>(false);
                if (implicitDefine != null)
                {
                    implicitTypes = implicitDefine.Types;
                }
                cacheImplicitAssignableTypes.Add(type, implicitTypes);
            }
            return implicitTypes;
        }

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
        public static bool IsIntegerType(Type type)
        {
            return IntegerTypes.Contains(type);
        }
        public static bool GetIntegerRange(Type type, out Tuple<long, long> range)
        {
            return IntegerRanges.TryGetValue(type, out range);
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
            if (!this.renderers.TryGetValue(node.ElementType, out renderer))
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
        protected const float PropertyNameWidth = 100f;
        protected const float ArrayMemberIndent = 30f;
        protected const float ArrayMemberTitleWidth = PropertyNameWidth - ArrayMemberIndent;
        protected const float ArrayMenuButtonWidth = 20f;
        protected const float VerticalSpacing = 2f;

        /// <summary>
        /// 节点值是否被修改
        /// </summary>
        public bool Dirty { get; set; }
        public NodeWrap Node { get; set; }
        public virtual void Render()
        {
            EditorGUILayout.BeginHorizontal();
            if (Node.Tooltip != null)
            {
                GUILayout.Label(new GUIContent(Node.Key, Node.Tooltip), GUILayout.Width(PropertyNameWidth));
            }
            else
            {
                GUILayout.Label(Node.Key, GUILayout.Width(PropertyNameWidth));
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
            base.Render();

            if (Node.ValueNode.isExpanded)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(ArrayMemberIndent);

                GUILayout.BeginVertical("HelpBox");
                GUILayout.Space(VerticalSpacing);
                RenderMenu();
                GUILayout.Space(VerticalSpacing);
                RenderMembers();
                GUILayout.Space(VerticalSpacing);
                GUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
        }
        protected override void RenderValue()
        {
            var arrayParent = Node.ValueNode;
            var arrayTypeName = (Node.ExplicitValueType != null ? Node.ExplicitValueType : Node.ValueType).FullName;
            if (arrayTypeName.EndsWith("[]"))
            {
                arrayTypeName = arrayTypeName.Replace("[]", $"[{arrayParent.arraySize}]");
            }
            arrayParent.isExpanded = EditorGUILayout.Foldout(arrayParent.isExpanded, arrayTypeName);
        }

        private void RenderMenu()
        {
            EditorGUILayout.BeginHorizontal();
            int size = Node.ValueNode.arraySize;
            GUILayout.Label("Length", GUILayout.Width(ArrayMemberTitleWidth));
            int newSize = Mathf.Clamp(EditorGUILayout.IntField(size), 0, ushort.MaxValue);
            if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(ArrayMenuButtonWidth)) && size > 0)
            {
                newSize = size - 1;
            }
            if (GUILayout.Button(string.Empty, "OL Plus", GUILayout.Width(ArrayMenuButtonWidth)) && size < ushort.MaxValue)
            {
                newSize = size + 1;
            }
            if (newSize != size)
            {
                Node.ValueNode.arraySize = newSize;
                Dirty |= true;
            }
            EditorGUILayout.EndHorizontal();
        }
        protected virtual void RenderMembers()
        {
            var arrayParent = Node.ValueNode;
            var memberType = (Node.ExplicitValueType != null ? Node.ExplicitValueType : Node.ValueType).GetElementType();
            var deleteMemberIndex = -1;

            for (int i = 0; i < arrayParent.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"Element {i}", GUILayout.Width(ArrayMemberTitleWidth));
                RenderMemberValue(arrayParent.GetArrayElementAtIndex(i), memberType);
                if (GUILayout.Button(string.Empty, "WinBtnClose"))
                {
                    deleteMemberIndex = i;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (deleteMemberIndex >= 0)
            {
                arrayParent.DeleteArrayElementAtIndex(deleteMemberIndex);
                Dirty |= true;
            }
        }
        protected abstract void RenderMemberValue(SerializedProperty node, Type type);
    }

    internal class UnknownRenderer : Renderer
    {
        protected override void RenderValue()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                string typeName = (Node.ExplicitValueType != null ? Node.ExplicitValueType : Node.ValueType).FullName;
                GUILayout.Label(new GUIContent(typeName, $"不受支持的类型: {typeName}"), GUI.skin.textField);
            }
        }
    }


    [RenderTarget(typeof(XOR.Serializables.Number))]
    internal class NumberRenderer : Renderer
    {
        protected override void RenderValue()
        {
            if (Node.ExplicitValueEnum != null)
            {
                RendererEnumValue();
            }
            else if (Node.ExplicitValueType != null && Helper.IsIntegerType(Node.ExplicitValueType))
            {
                RenderIntegerValue();
            }
            else
            {
                RenderSingleValue();
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
            if (Helper.GetIntegerRange(Node.ExplicitValueType, out Tuple<long, long> range))
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
        protected virtual void RenderSingleValue()
        {
            var value = Node.ValueNode.doubleValue;
            var newValue = Node.ExplicitValueRange != null ?
                EditorGUILayout.Slider((float)value, Node.ExplicitValueRange.Item1, Node.ExplicitValueRange.Item2) :
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
            string[] keyOptions = Node.ExplicitValueEnum.Keys.ToArray(),
               valueOptions = Node.ExplicitValueEnum.Values.Cast<string>().ToArray();
            var valueIndex = Array.IndexOf(valueOptions, Node.ValueNode.stringValue);
            var newIndex = EditorGUILayout.Popup(valueIndex, keyOptions);
            if (newIndex != valueIndex)
            {
                Node.ValueNode.stringValue = valueOptions[newIndex];
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


    [RenderTarget(typeof(XOR.Serializables.NumberArray))]
    internal class NumberArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(SerializedProperty node, Type type)
        {
            if (Node.ExplicitValueEnum != null)
            {
                RendererMemberEnumValue(node, type);
            }
            else if (Helper.IsIntegerType(type))
            {
                RenderMemberIntValue(node, type);
            }
            else
            {
                RenderMemberSingleValue(node, type);
            }
        }

        protected virtual void RendererMemberEnumValue(SerializedProperty node, Type type)
        {
            var value = (int)node.doubleValue;
            var newValue = EditorGUILayout.IntPopup(value, Node.ExplicitValueEnum.Keys.ToArray(), Node.ExplicitValueEnum.Values.Cast<int>().ToArray());
            if (newValue != value || Math.Abs(newValue - node.doubleValue) > float.Epsilon)
            {
                node.doubleValue = newValue;
                Dirty |= true;
            }
        }
        protected virtual void RenderMemberIntValue(SerializedProperty node, Type type)
        {
            var value = (int)node.doubleValue;
            var newValue = Node.ExplicitValueRange != null ?
                EditorGUILayout.IntSlider(value, (int)Node.ExplicitValueRange.Item1, (int)Node.ExplicitValueRange.Item2) :
                EditorGUILayout.IntField(value);
            if (Helper.GetIntegerRange(type, out Tuple<long, long> range))
            {
                if (newValue < range.Item1) newValue = (int)range.Item1;
                else if (newValue > range.Item2) newValue = (int)range.Item2;
            }
            if (newValue != value || Math.Abs(newValue - node.doubleValue) > float.Epsilon)
            {
                node.doubleValue = newValue;
                Dirty |= true;
            }
        }
        protected virtual void RenderMemberSingleValue(SerializedProperty node, Type type)
        {
            var value = node.doubleValue;
            var newValue = Node.ExplicitValueRange != null ?
                EditorGUILayout.Slider((float)value, Node.ExplicitValueRange.Item1, Node.ExplicitValueRange.Item2) :
                EditorGUILayout.DoubleField(value);
            if (newValue != value || Math.Abs(newValue - node.doubleValue) > float.Epsilon)
            {
                node.doubleValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.BigintArray))]
    internal class BigintArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(SerializedProperty node, Type type)
        {
            var value = node.longValue;
            var newValue = Node.ExplicitValueRange != null ?
                EditorGUILayout.IntSlider((int)value, (int)Node.ExplicitValueRange.Item1, (int)Node.ExplicitValueRange.Item2) :
                EditorGUILayout.LongField(value);
            if (newValue != value)
            {
                node.longValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.StringArray))]
    internal class StringArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(SerializedProperty node, Type type)
        {
            if (Node.ExplicitValueEnum != null)
            {
                RendererMemberEnumValue(node, type);
            }
            else
            {
                RendererMemberStringValue(node, type);
            }
        }
        protected virtual void RendererMemberStringValue(SerializedProperty node, Type type)
        {
            var value = node.stringValue;
            var newValue = EditorGUILayout.TextField(value);
            if (newValue != value)
            {
                node.stringValue = newValue;
                Dirty |= true;
            }

        }
        protected virtual void RendererMemberEnumValue(SerializedProperty node, Type type)
        {
            string[] keyOptions = Node.ExplicitValueEnum.Keys.ToArray(),
                valueOptions = Node.ExplicitValueEnum.Values.Cast<string>().ToArray();
            var valueIndex = Array.IndexOf(valueOptions, node.stringValue);
            var newIndex = EditorGUILayout.Popup(valueIndex, keyOptions);
            if (newIndex != valueIndex)
            {
                node.stringValue = valueOptions[newIndex];
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.BooleanArray))]
    internal class BooleanArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(SerializedProperty node, Type type)
        {
            var newValue = EditorGUILayout.Toggle(node.boolValue);
            if (newValue != node.boolValue)
            {
                node.boolValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.Vector2Array))]
    internal class Vector2ArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(SerializedProperty node, Type type)
        {
            var newValue = EditorGUILayout.Vector2Field(string.Empty, node.vector2Value);
            if (newValue != node.vector2Value)
            {
                node.vector2Value = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.Vector3Array))]
    internal class Vector3ArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(SerializedProperty node, Type type)
        {
            var newValue = EditorGUILayout.Vector3Field(string.Empty, node.vector3Value);
            if (newValue != node.vector3Value)
            {
                node.vector3Value = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.ObjectArray))]
    internal class ObjectArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(SerializedProperty node, Type type)
        {
            var newValue = EditorGUILayout.ObjectField(string.Empty, node.objectReferenceValue, type, true);
            if (newValue != node.objectReferenceValue)
            {
                node.objectReferenceValue = newValue;
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