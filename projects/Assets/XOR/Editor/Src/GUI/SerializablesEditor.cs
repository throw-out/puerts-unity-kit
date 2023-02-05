using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace XOR.Serializables
{
    internal class RootWrap
    {
        public Type Type { get; private set; }
        public SerializedObject Root { get; private set; }
        public Dictionary<string, FieldWrap> FieldMapping { get; private set; }

        public string GetMenu(Type elementType)
        {
            var targetField = FieldMapping.FirstOrDefault(o => o.Value.Element.Type == elementType);
            if (!string.IsNullOrEmpty(targetField.Key))
            {
                return targetField.Value.Menu;
            }
            return string.Empty;
        }
        public SerializedProperty GetProperty(string propertyName)
        {
            return Root != null ? Root.FindProperty(propertyName) : null;
        }
        public void Update()
        {
            if (Root != null)
            {
                Root.Update();
            }
        }
        public void ApplyModifiedProperties()
        {
            if (Root != null)
            {
                Root.ApplyModifiedProperties();
            }
        }

        public static RootWrap Create(SerializedObject root, Type type)
        {
            return new RootWrap()
            {
                Root = root,
                Type = type,
                FieldMapping = Utility.GetFieldMapping(type),
            };
        }
    }
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
            this.ValueType = elementType.GetField("value", Utility.Flags).FieldType;
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
                case SerializedPropertyType.Integer:
                    return "long";
            }
            return Enum.GetName(typeof(SerializedPropertyType), type);
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
        public string Menu { get; private set; }

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
            var menuPath = elementType.GetCustomAttribute<XOR.Serializables.MenuPathAttribute>(false);
            return new FieldWrap()
            {
                Field = field,
                FieldType = field.FieldType,
                Element = element,
                Menu = menuPath?.Path ?? elementType.Name
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
                if (Utility.IsAssignable(fw.Element, valueType))
                {
                    AddProperty(fw, component, key, index);
                    return true;
                }
            }
            foreach (var fw in FieldMapping.Values)
            {
                if (Utility.IsImplicitAssignable(fw.Element, valueType))
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
            if (fw == null || values == null || !Utility.IsAssignable(fw.Element, valueType) && !Utility.IsImplicitAssignable(fw.Element, valueType))
                return false;
            var _value = value.Value;
            if (_value != null && Utility.IsAssignable(fw.Element, valueType))
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
            else if (Utility.IsAssignable(fw.Element, newValue.GetType()))
            {
                fw.Element.SetValue(value, newValue);
            }
            else if (Utility.IsImplicitAssignable(fw.Element, newValue.GetType()))
            {
                fw.Element.SetValue(value, Utility.GetAssignableValue(fw.Element, newValue));
            }
            else
            {
                Logger.LogWarning($"Invail Type Assignment: The target type require {fw.Element.ValueType.FullName}, but actual type is {newValue.GetType().FullName}");
                fw.Element.SetValue(value, default);
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

        private static ComponentWrap<TComponent> _cacheInsatcne;
        public static ComponentWrap<TComponent> Create(bool isForce = false)
        {
            if (_cacheInsatcne == null || isForce)
            {
                _cacheInsatcne = new ComponentWrap<TComponent>()
                {
                    Type = typeof(TComponent),
                    FieldMapping = Utility.GetFieldMapping(typeof(TComponent)),
                }; ;
            }
            return _cacheInsatcne;
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

    internal static class Utility
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
            return XOR.Serializables.ImplicitOperation.IsImplicitAssignable(element.Type, element.ValueType, valueType);
        }
        /// <summary>
        /// 进行类型强转(允许隐式转换分配)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetAssignableValue(ElementWrap element, object value)
        {
            return XOR.Serializables.ImplicitOperation.GetAssignableValue(element.ValueType, value);
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
        static readonly HashSet<Type> SingleTypes = new HashSet<Type>()
        {
            typeof(float),
            typeof(double)
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
        public static bool IsSingleType(Type type)
        {
            return SingleTypes.Contains(type);
        }
        public static bool GetIntegerRange(Type type, out Tuple<long, long> range)
        {
            return IntegerRanges.TryGetValue(type, out range);
        }
        public static bool SetIntegerRange(Type type, ref int value)
        {
            if (GetIntegerRange(type, out Tuple<long, long> range))
            {
                if (value < range.Item1) value = (int)range.Item1;
                else if (value > range.Item2) value = (int)range.Item2;
                return true;
            }
            return false;
        }
        public static bool SetIntegerArrayRange(Type type, Array array)
        {
            if (array == null || array.Length == 0)
                return false;
            if (GetIntegerRange(type, out Tuple<long, long> range))
            {
                for (int i = 0; i < array.Length; i++)
                {
                    int value = (int)(double)array.GetValue(i);
                    if (value < range.Item1)
                    {
                        array.SetValue(range.Item1, i);
                    }
                    else if (value > range.Item2)
                    {
                        array.SetValue(range.Item2, i);
                    }
                }
                return true;
            }
            return false;
        }

        public static void SetRange(Tuple<float, float> range, ref double value)
        {
            if (value < range.Item1) value = range.Item1;
            else if (value > range.Item2) value = range.Item2;
        }
        public static void SetArrayRange(Tuple<float, float> range, double[] array)
        {
            if (array == null || array.Length == 0)
                return;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < range.Item1)
                {
                    array[i] = range.Item1;
                }
                else if (array[i] > range.Item2)
                {
                    array[i] = range.Item2;
                }
            }
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
        protected const float ArrayMemberIndent = 30f;
        protected const float ArrayMemberTitleWidth = PropertyNameWidth - ArrayMemberIndent;
        protected const float ArrayMenuButtonWidth = 20f;
        protected const float VerticalSpacing = 2f;
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

        protected virtual void RenderMenu()
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
                GUILayout.Label(new GUIContent(typeName, $"未实现编辑的类型: {typeName}"), GUI.skin.textField);
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
                Dirty |= RenderEnumValue(Node, Node.ValueNode);
            }
            else if (Node.ExplicitValueType != null && Utility.IsIntegerType(Node.ExplicitValueType))
            {
                Dirty |= RenderIntegerValue(Node, Node.ValueNode, Node.ExplicitValueType);
            }
            else
            {
                Dirty |= RenderSingleValue(Node, Node.ValueNode);
            }
        }

        public static bool RenderEnumValue(NodeWrap nw, SerializedProperty node)
        {
            string[] keyOptions = nw.ExplicitValueEnum.Keys.ToArray();
            int[] valueOptions = nw.ExplicitValueEnum.Values.Cast<int>().ToArray();
            var value = (int)node.doubleValue;
            var newValue = EditorGUILayout.IntPopup(value, keyOptions, valueOptions);
            if (newValue != value || Math.Abs(newValue - node.doubleValue) > float.Epsilon)
            {
                node.doubleValue = newValue;
                return true;
            }
            return false;
        }
        public static bool RenderIntegerValue(NodeWrap nw, SerializedProperty node, Type type)
        {
            var value = (int)node.doubleValue;
            var newValue = nw.ExplicitValueRange != null ?
                EditorGUILayout.IntSlider(value, (int)nw.ExplicitValueRange.Item1, (int)nw.ExplicitValueRange.Item2) :
                EditorGUILayout.IntField(value);
            Utility.SetIntegerRange(type, ref value);
            if (newValue != value || Math.Abs(newValue - node.doubleValue) > float.Epsilon)
            {
                node.doubleValue = newValue;
                return true;
            }
            return false;
        }
        public static bool RenderSingleValue(NodeWrap nw, SerializedProperty node)
        {
            var value = node.doubleValue;
            var newValue = nw.ExplicitValueRange != null ?
                EditorGUILayout.Slider((float)value, nw.ExplicitValueRange.Item1, nw.ExplicitValueRange.Item2) :
                EditorGUILayout.DoubleField(value);
            if (newValue != value || Math.Abs(newValue - node.doubleValue) > float.Epsilon)
            {
                node.doubleValue = newValue;
                return true;
            }
            return false;
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
                Dirty |= RenderEnumValue(Node, Node.ValueNode);
            }
            else
            {
                Dirty |= RenderStringValue(Node.ValueNode);
            }
        }
        public static bool RenderEnumValue(NodeWrap nw, SerializedProperty node)
        {
            string[] keyOptions = nw.ExplicitValueEnum.Keys.ToArray(),
               valueOptions = nw.ExplicitValueEnum.Values.Cast<string>().ToArray();
            var valueIndex = Array.IndexOf(valueOptions, node.stringValue);
            var newIndex = EditorGUILayout.Popup(valueIndex, keyOptions);
            if (newIndex != valueIndex)
            {
                node.stringValue = valueOptions[newIndex];
                return true;
            }
            return false;
        }
        public static bool RenderStringValue(SerializedProperty node)
        {
            var value = node.stringValue;
            var newValue = EditorGUILayout.TextField(value);
            if (newValue != value)
            {
                node.stringValue = newValue;
                return true;
            }
            return false;
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
                Dirty |= NumberRenderer.RenderEnumValue(Node, node);
            }
            else if (Utility.IsIntegerType(type))
            {
                Dirty |= NumberRenderer.RenderIntegerValue(Node, node, type);
            }
            else
            {
                Dirty |= NumberRenderer.RenderSingleValue(Node, node);
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
                Dirty |= StringRenderer.RenderEnumValue(Node, node);
            }
            else
            {
                Dirty |= StringRenderer.RenderStringValue(node);
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
}
namespace XOR.Serializables.TsProperties
{
    internal class Display
    {
        private RootWrap root;
        private readonly Dictionary<Type, Renderer> renderers;
        private Renderer unknown;
        public Display(RootWrap root)
        {
            this.renderers = new Dictionary<Type, Renderer>();
            this.root = root;
        }
        public void AddRenderer<TRenderer>(Type valueType)
            where TRenderer : Renderer, new()
        {
            var renderer = new TRenderer();
            renderer.Root = root;
            this.renderers.Add(valueType, renderer);
        }
        public bool Render(Rect position, NodeWrap node)
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
                renderer.Render(position, GUIContent.none);
                return renderer.Dirty;
            }
            return false;
        }
        public float GetHeight(NodeWrap node)
        {
            Renderer renderer = null;
            this.renderers.TryGetValue(node.ElementType, out renderer);
            if (renderer != null)
            {
                renderer.Node = node;
                return renderer.GetHeight();
            }
            return EditorGUIUtility.singleLineHeight;
        }
        public static Display Create(RootWrap root)
        {
            Assembly assembly = typeof(Display).Assembly;
            Type[] rendererTypes = (from type in assembly.GetTypes()
                                    where !type.IsAbstract && typeof(Renderer).IsAssignableFrom(type)
                                    select type).ToArray();
            Display display = new Display(root);
            display.unknown = new UnknownRenderer();
            foreach (var type in rendererTypes)
            {
                var target = type.GetCustomAttribute<RenderTargetAttribute>(false);
                if (target == null)
                    continue;
                var renderer = System.Activator.CreateInstance(type) as Renderer;
                renderer.Root = root;
                display.renderers.Add(target.Type, renderer);
            }
            return display;
        }
    }

    internal class Utility
    {
        public static void PopupCreate(RootWrap root, int index, Action callback = null)
        {
            string[] menuItems = root.FieldMapping.Select(o => o.Value.Menu).ToArray();
            CustomMenu(menuItems, null, null, null, (selectIndex) =>
            {
                //Create Element
                var arrayParent = root.GetProperty(root.FieldMapping.Keys.ElementAt(selectIndex));
                arrayParent.arraySize++;
                var newNode = new NodeWrap(
                    arrayParent.GetArrayElementAtIndex(arrayParent.arraySize - 1),
                    root.FieldMapping.Values.ElementAt(selectIndex).Element.Type
                );
                newNode.Index = index;
                newNode.Key = "key" + index;
                if (newNode.ValueNode.isArray)
                {
                    newNode.ValueNode.arraySize = 0;
                    newNode.ValueNode.isExpanded = true;
                }
                newNode.CleanValue();
                //应用更改
                root.ApplyModifiedProperties();

                if (callback != null) callback.Invoke();
            });
        }
        public static void PopupTypes(RootWrap root, NodeWrap node)
        {
            var menuItems = root.FieldMapping.Select(o => o.Value.Menu).ToArray();
            var selected = new[] { menuItems.ToList().IndexOf(root.GetMenu(node.ElementType)) };
            CustomMenu(menuItems, null, selected, selected, (selectIndex) =>
            {
                //Create Element
                var arrayParent = root.GetProperty(root.FieldMapping.Keys.ElementAt(selectIndex));
                arrayParent.arraySize++;
                var newNode = new NodeWrap(
                    arrayParent.GetArrayElementAtIndex(arrayParent.arraySize - 1),
                    root.FieldMapping.Values.ElementAt(selectIndex).Element.Type
                );
                newNode.Index = node.Index;
                newNode.Key = node.Key;
                if (newNode.ValueNode.isArray)
                {
                    newNode.ValueNode.arraySize = node.ValueNode.isArray ? node.ValueNode.arraySize : 0;
                    newNode.ValueNode.isExpanded = node.ValueNode.isArray ? node.ValueNode.isExpanded : true;
                }
                newNode.CleanValue();

                Copy(node, newNode);
                //Delete Element
                node.RemoveFromArrayParent();
                //应用更改
                root.ApplyModifiedProperties();
            });
        }
        public static void PopupComponentsAndTypes(RootWrap root, NodeWrap node, UnityEngine.Object obj, Type targetType)
        {
            var menuItems = root.FieldMapping.Select(o => o.Value.Menu).ToArray();
            var selected = new[] { menuItems.ToList().IndexOf(root.GetMenu(node.ElementType)) };
            string[] separator = null;
            var objects = GetCompoents(obj, targetType);
            if (objects != null)
            {
                //Options
                var _menuItems = new List<string>() { "<NULL>" };
                _menuItems.AddRange(CheckOptions((from o in objects select ("-" + o.GetType().Name)).ToArray()));
                _menuItems.AddRange(menuItems);
                menuItems = _menuItems.ToArray();
                //Objects
                var _objects = objects.ToList();
                _objects.Insert(0, null);
                objects = _objects.ToArray();
                //Separator
                string nullable = null;
                separator = new List<string>(from _ in _objects select nullable) { }.ToArray();
                separator[0] = separator[separator.Length - 1] = "";
                //Select
                selected = new List<int>() { _objects.IndexOf(obj), selected[0] + objects.Length }.ToArray();
            }
            CustomMenu(menuItems, separator, selected, selected, (selectIndex) =>
            {
                if (objects == null || selectIndex >= objects.Length)
                {
                    selectIndex = selectIndex - (objects != null ? objects.Length : 0);
                    //Create Element
                    var arrayParent = root.GetProperty(root.FieldMapping.Keys.ElementAt(selectIndex));
                    arrayParent.arraySize++;
                    var newNode = new NodeWrap(
                            arrayParent.GetArrayElementAtIndex(arrayParent.arraySize - 1),
                            root.FieldMapping.Values.ElementAt(selectIndex).Element.Type
                    );
                    newNode.Index = node.Index;
                    newNode.Key = node.Key;
                    if (newNode.ValueNode.isArray)
                    {
                        newNode.ValueNode.arraySize = node.ValueNode.isArray ? node.ValueNode.arraySize : 0;
                        newNode.ValueNode.isExpanded = node.ValueNode.isArray ? node.ValueNode.isExpanded : true;
                    }
                    newNode.CleanValue();
                    Copy(node, newNode);
                    //Delete Element
                    node.RemoveFromArrayParent();
                }
                else
                {
                    node.ValueNode.objectReferenceValue = objects[selectIndex];
                }
                //应用更改
                root.ApplyModifiedProperties();
            });
        }
        public static void PopupComponents(RootWrap info, SerializedProperty node, UnityEngine.Object obj, Type targetType)
        {
            var menuItems = new[] { "<NULL>" };
            var selected = new[] { 0 };
            string[] separator = null;
            var objects = GetCompoents(obj, targetType);
            if (objects != null)
            {
                //Options
                var _menuItems = new List<string>() { "<NULL>" };
                _menuItems.AddRange(CheckOptions((from o in objects select ("-" + o.GetType().Name)).ToArray()));
                menuItems = _menuItems.ToArray();
                //Objects
                var _objects = objects.ToList();
                _objects.Insert(0, null);
                objects = _objects.ToArray();
                //Separator
                separator = new[] { "" };
                //Select
                selected = new List<int>() { _objects.IndexOf(obj) }.ToArray();
            }
            CustomMenu(menuItems, separator, selected, selected, (selectIndex) =>
            {
                if (objects != null && selectIndex < objects.Length)
                {
                    node.objectReferenceValue = objects[selectIndex];
                    //应用更改
                    info.ApplyModifiedProperties();
                }
            });
        }
        public static void PopupArrayComponents(RootWrap info, SerializedProperty arrayParent, Type targetType)
        {
            var objDict = new Dictionary<SerializedProperty, Dictionary<string, UnityEngine.Object>>();
            var objNames = new List<string>();
            for (int i = 0; i < arrayParent.arraySize; i++)
            {
                var node = arrayParent.GetArrayElementAtIndex(i);
                var compoents = new Dictionary<string, UnityEngine.Object>();
                var _objects = GetCompoents(node.objectReferenceValue, targetType);
                if (_objects != null)
                {
                    Array.ForEach(_objects, o =>
                    {
                        var name = o.GetType().Name;
                        objNames.Add(name);
                        compoents[name] = o;
                    });
                }
                objDict.Add(node, compoents);
            }
            objNames = objNames.Distinct().ToList();
            objNames.Sort();
            Array.ForEach(new[] { "Transform", "GameObject" }, name =>
            {
                if (!objNames.Contains(name)) return;
                objNames.Remove(name);
                objNames.Insert(0, name);
            });
            objNames.Insert(0, "<NULL>");

            var selected = new int[0];
            var separator = new[] { "" };
            CustomMenu(objNames.ToArray(), separator, selected, selected, (selectIndex) =>
           {
               if (selectIndex >= 0 && selectIndex < objNames.Count)
               {
                   var name = objNames[selectIndex];
                   foreach (var item in objDict)
                   {
                       UnityEngine.Object obj = null;
                       if (item.Value.TryGetValue(name, out obj) || selectIndex == 0)
                       {
                           item.Key.objectReferenceValue = obj;
                       }
                   }
                   //应用更改
                   info.ApplyModifiedProperties();
               }
           });
        }

        /// <summary>
        /// 在鼠标位置弹出菜单(在GUI调用完成后)
        /// </summary>
        public static void CustomMenu(string[] options, string[] separator, int[] selected, int[] disabled, Action<int> callback)
        {
            //EditorUtility.DisplayCustomMenu();  // 在指定区域弹出菜单 (需GUI方法内部调用)
            var _separator = new List<string>(separator != null ? separator : new string[0]);
            var _selected = new List<int>(selected != null ? selected : new int[0]);
            var _disabled = new List<int>(disabled != null ? disabled : new int[0]);
            var menu = new UnityEditor.GenericMenu();
            for (int i = 0; i < options.Length; i++)
            {
                var index = i;
                if (_disabled.Contains(index))
                    menu.AddDisabledItem(new GUIContent(options[i]), _selected.Contains(index));
                else
                    menu.AddItem(new GUIContent(options[i]), _selected.Contains(index), () => callback(index));
                if (i < _separator.Count && _separator[i] != null)
                    menu.AddSeparator(_separator[i]);
            }
            menu.ShowAsContext();
        }

        public static void Copy(NodeWrap from, NodeWrap to)
        {
            if (from.ValueType.IsArray && to.ValueType.IsArray)
            {
                var targetType = to.ValueType.GetElementType();
                for (int i = 0; i < from.ValueNode.arraySize && i < to.ValueNode.arraySize; i++)
                {
                    Copy(
                        from.ValueNode.GetArrayElementAtIndex(i),
                        to.ValueNode.GetArrayElementAtIndex(i),
                        targetType
                    );
                }
            }
            else
            {
                Copy(from.ValueNode, to.ValueNode, to.ValueType);
            }
        }
        public static void Copy(SerializedProperty from, SerializedProperty to, Type targetType)
        {
            if (from.propertyType == to.propertyType)
            {
                switch (from.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        to.intValue = from.intValue;
                        break;
                    case SerializedPropertyType.Boolean:
                        to.boolValue = from.boolValue;
                        break;
                    case SerializedPropertyType.Float:
                        to.doubleValue = from.doubleValue;
                        break;
                    case SerializedPropertyType.String:
                        to.stringValue = from.stringValue;
                        break;
                    case SerializedPropertyType.Color:
                        to.colorValue = from.colorValue;
                        break;
                    case SerializedPropertyType.ObjectReference:
                        if (from.objectReferenceValue != null && targetType.IsAssignableFrom(from.objectReferenceValue.GetType()))
                        {
                            to.objectReferenceValue = from.objectReferenceValue;
                        }
                        break;
                    case SerializedPropertyType.Vector2:
                        to.vector2Value = from.vector2Value;
                        break;
                    case SerializedPropertyType.Vector3:
                        to.vector3Value = from.vector3Value;
                        break;
                    case SerializedPropertyType.Vector4:
                        to.vector4Value = from.vector4Value;
                        break;
                    case SerializedPropertyType.Rect:
                        to.rectValue = from.rectValue;
                        break;
                    case SerializedPropertyType.Bounds:
                        to.boundsValue = from.boundsValue;
                        break;
                    case SerializedPropertyType.Vector2Int:
                        to.vector2IntValue = from.vector2IntValue;
                        break;
                    case SerializedPropertyType.Vector3Int:
                        to.vector3IntValue = from.vector3IntValue;
                        break;
                    case SerializedPropertyType.RectInt:
                        to.rectIntValue = from.rectIntValue;
                        break;
                    case SerializedPropertyType.BoundsInt:
                        to.boundsIntValue = from.boundsIntValue;
                        break;
                }
            }
        }

        private static List<string> reservedKeywords = new List<string>()
        {
            "abstract", "arguments", "boolean", "break", "byte",
            "case", "catch", "char", "class", "const",
            "continue", "debugger", "default", "delete", "do",
            "double", "else", "enum", "eval", "export",
            "extends", "false", "final", "finally", "float",
            "for", "function", "goto", "if", "implements",
            "import", "in", "instanceof", "int", "interface",
            "let", "long", "native", "new", "null",
            "package", "private", "protected", "public", "return",
            "short", "static", "super", "switch", "synchronized",
            "this", "throw", "throws", "transient", "true",
            "try", "typeof", "var", "void", "volatile",
            "while", "with", "yield"
        };
        /// <summary>检测命名是否符合规则 </summary>
        public static bool IsValidKey(string name)
        {
            //#:    35
            //$:    36
            //0-9:  48-57
            //A-Z:  65-90
            //_:    95
            //a-z:  97-122
            if (string.IsNullOrEmpty(name))
                return false;
            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (!(
                    c == 35 && i == 0 ||
                    c == 36 ||
                    c >= 48 && c <= 57 && i != 0 ||
                    c >= 65 && c <= 90 ||
                    c == 95 ||
                    c >= 97 && c <= 122))
                    return false;
            }
            return !reservedKeywords.Contains(name);
        }
        /// <summary>生成typescript声明代码 </summary>
        public static string GenerateDeclareCode(XOR.Serializables.ResultPair[] pairs, string declarePrefix, bool useFullname)
        {
            if (pairs == null || pairs.Length == 0)
                return string.Empty;
            declarePrefix = declarePrefix.Trim();

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < pairs.Length; i++)
            {
                if (i > 0) builder.AppendLine();
                var pair = pairs[i];

                builder.Append(declarePrefix);
                builder.Append(" ");
                if (IsValidKey(pair.key))
                {
                    builder.Append(pair.key);
                }
                else
                {
                    builder.Append("[\"");
                    builder.Append(pair.key);
                    builder.Append("\"]");
                }
                builder.Append(": ");

                if (pair.value != null)
                {
                    string typeStr = null;
                    if (pair.value.GetType().IsArray && ((Array)pair.value).Length > 0)
                    {
                        var arr = (Array)pair.value;
                        for (int j = 0; j < arr.Length; j++)
                        {
                            var o = arr.GetValue(j);
                            if (o != null && !o.Equals(null))
                            {
                                typeStr = "System.Array$1<" + GetTypeName(o.GetType(), useFullname) + ">";
                                break;
                            }
                        }
                    }
                    builder.Append(typeStr ?? GetTypeName(pair.value.GetType(), useFullname));
                }
                else
                {
                    builder.Append(GetTypeName(null, useFullname));
                }

                builder.Append(";");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取UnityEngine.Object上的组件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        static UnityEngine.Object[] GetCompoents(UnityEngine.Object obj, Type targetType)
        {
            if (obj != null)
            {
                var result = new List<UnityEngine.Object>();
                var type = obj.GetType();
                //获取GameObject和Transform组件
                var getObj = type.GetProperty("gameObject");
                var getTrf = type.GetProperty("transform");
                var gameObject = (getObj != null ? getObj.GetValue(obj) : null) as GameObject;
                var transform = (getTrf != null ? getTrf.GetValue(obj) : null) as Transform;
                //调用GetComponents方法获取所有组件, 如果有gameObject则从Gameobject对象中获取所有组件(排除obj自身对排序的干扰)
                if (gameObject != null)
                    type = gameObject.GetType();
                var getComponents = (from method in type.GetMethods()
                                     let parames = method.GetParameters()
                                     where method.Name == "GetComponents"
                                        && method.ReturnType == typeof(Component[])
                                        && parames.Length == 1
                                        && parames[0].ParameterType == typeof(System.Type)
                                     select method).FirstOrDefault();
                if (getComponents != null)
                {
                    var components = getComponents.Invoke(gameObject != null ? gameObject : obj, new object[] { typeof(Component) }) as Component[];
                    foreach (var o in components)
                    {
                        if (!result.Contains(o))
                            result.Add(o);
                    }
                }
                //obj自身
                if (!result.Contains(obj))
                    result.Add(obj);
                //通过Type名进行排序
                result = (from o in result orderby o.GetType().Name select o).ToList();
                //GameObject / Transform
                if (transform != null)
                {
                    result.Remove(transform);
                    result.Insert(0, transform);
                }
                if (gameObject != null)
                {
                    result.Remove(gameObject);
                    result.Insert(0, gameObject);
                }
                //移除无效项
                if (targetType != null)
                {
                    for (int i = result.Count - 1; i >= 0; i--)
                    {
                        if (result[i] == null || !targetType.IsAssignableFrom(result[i].GetType()))
                        {
                            result.RemoveAt(i);
                        }
                    }
                }
                return result.ToArray();
            }
            return null;
        }
        /// <summary>
        /// 检测Menu.Options是否有重复名称, 并返回更正后的Options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        static string[] CheckOptions(string[] options)
        {
            //重命名(重复名称-相同类型)
            var countDict = new Dictionary<string, int>();
            foreach (var option in options)
            {
                if (countDict.ContainsKey(option))
                {
                    var count = countDict[option];
                    countDict[option] = count == 0 ? 2 : ++count;
                }
                else countDict.Add(option, 0);
            }
            for (int i = options.Length - 1; i >= 0; i--)
            {
                var count = countDict[options[i]];
                if (count > 0)
                {
                    countDict[options[i]] = count - 1;
                    options[i] += "(" + count + ")";
                }
            }
            return options;
        }
        /// <summary> 删除选中的节点 </summary>
        static void DeleteElements(NodeWrap[] nodes)
        {
            //依据父节点进行分组
            var grouping = new Dictionary<SerializedProperty, List<NodeWrap>>();
            foreach (var node in nodes)
            {
                List<NodeWrap> list;
                if (!grouping.TryGetValue(node.ArrayParent, out list))
                {
                    list = new List<NodeWrap>();
                    grouping.Add(node.ArrayParent, list);
                }
                list.Add(node);
            }
            //进行排序然后删除(先删除arrayIndex大的节点)
            foreach (var group in grouping)
            {
                var values = group.Value.ToList();
                values.Sort((v1, v2) => v1.ArrayIndex > v2.ArrayIndex ? -1 : v1.ArrayIndex < v2.ArrayIndex ? 1 : 0);
                foreach (var element in values)
                {
                    element.RemoveFromArrayParent();
                }
            }
        }

        static string GetTypeName(Type type, bool useFullname)
        {
            if (type == null)
                return "undefined";
            //Array Type
            if (type.IsArray)
                return $"{GetTypeName(type.GetElementType(), useFullname)}[]";
            //Value Mapping
            if (type.Equals(typeof(double)) || type.Equals(typeof(float)) || type.Equals(typeof(int)) || type.Equals(typeof(long)))
                return "number";
            if (type.Equals(typeof(long)))
                return "bigint";
            if (type.Equals(typeof(string)) || type.Equals(typeof(char)))
                return "string";
            if (type.Equals(typeof(bool)))
                return "boolean";
            if (type.Equals(typeof(DateTime)))
                return "Date";
            return useFullname ? type.FullName.Replace("+", ".") : type.Name;
        }
    }

    internal abstract class Renderer
    {
        protected const float VerticalSpacingDouble = VerticalSpacing * 2;
        protected const float VerticalSpacing = 2f;
        protected const float OperationWidth = 16f;
        protected const float QuadWidth = OperationWidth + 4f;
#if UNITY_2019_1_OR_NEWER
        protected const string OperationStyle = "PaneOptions";
#else
        protected const string OperationStyle = "Icon.Options";
#endif

        /// <summary>
        /// 节点值是否被修改
        /// </summary>
        public bool Dirty { get; set; }
        public NodeWrap Node { get; set; }
        public RootWrap Root { get; set; }
        public virtual void Render(Rect position, GUIContent label)
        {
            //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
            using (new EditorGUI.PropertyScope(position, label, Node.ArrayParent))
            {
                //设置属性名宽度 Name HP
                EditorGUIUtility.labelWidth = 0;
                //输入框高度，默认一行的高度
                position.height = this.GetLineHeight();

                float normalizeWidth = position.width - QuadWidth,
                    normalizeHalf = normalizeWidth / 2f;

                //输入框的位置
                Rect keyRect = new Rect(position)
                {
                    width = normalizeHalf,
                    height = position.height - VerticalSpacingDouble,
                    y = position.y + VerticalSpacing
                };
                Rect valueRect = new Rect(keyRect)
                {
                    x = keyRect.x + keyRect.width + 2,
                    width = normalizeHalf,
                };
                Rect operationRect = new Rect(valueRect)
                {
                    x = valueRect.x + valueRect.width + 2,
                };
                var newKey = EditorGUI.TextField(keyRect, string.Empty, Node.Key);
                if (newKey != Node.Key)
                {
                    Node.Key = newKey;
                    Dirty |= true;
                }
                RenderValue(position, valueRect);
                RenderOperation(operationRect);
            }
        }
        protected abstract void RenderValue(Rect position, Rect valueRect);
        protected virtual void RenderOperation(Rect operationRect)
        {
            if (EditorGUI.Toggle(operationRect, false, OperationStyle))
            {
                Utility.PopupTypes(Root, Node);
            }
        }

        public virtual float GetLineHeight()
        {
            return EditorGUIUtility.singleLineHeight + VerticalSpacingDouble;
        }
        public virtual float GetHeight()
        {
            return this.GetLineHeight();
        }
    }
    internal abstract class ArrayRenderer : Renderer
    {
        protected const float ArrayMemberIndent = 30f;
        protected const float ArrayMemberTitleWidth = 70f;
        protected const float HorizontalSpacing = VerticalSpacing;
        protected const float Padding = OperationWidth;
        private Color boxColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);

        public override void Render(Rect position, GUIContent label)
        {
            base.Render(position, label);

            if (Node.ValueNode.isExpanded)
            {
                var arrayParent = Node.ValueNode;
                var lineHight = base.GetLineHeight();

                //render background box
                Rect bgRect = new Rect(position)
                {
                    y = lineHight + position.y,
                    x = position.x + ArrayMemberIndent,
                    width = position.width - Padding - ArrayMemberIndent,
                    height = lineHight * (arrayParent.arraySize + 1) + VerticalSpacing * arrayParent.arraySize
                };
                if (arrayParent.arraySize > 0)
                {
                    bgRect.height += VerticalSpacingDouble;
                }
                EditorGUI.DrawRect(bgRect, boxColor);

                //render menu
                Rect menuRect = new Rect(position)
                {
                    y = position.y + lineHight,
                    x = position.x + ArrayMemberIndent,
                    width = position.width - Padding - ArrayMemberIndent,
                    height = lineHight
                };

                //render members
                bool hasObjectReference = false;
                Rect memberRect = new Rect(menuRect)
                {
                    y = menuRect.y + menuRect.height + VerticalSpacingDouble,
                };
                for (int i = 0; i < arrayParent.arraySize; i++)
                {
                    hasObjectReference |= RenderMember(
                        new Rect(memberRect) { y = memberRect.y + (lineHight + VerticalSpacing) * i },
                        i,
                        arrayParent.GetArrayElementAtIndex(i)
                    );
                }

                RenderMenu(menuRect, arrayParent, hasObjectReference);
            }
        }
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            var arrayParent = Node.ValueNode;
            var arrayTypeName = (Node.ExplicitValueType != null ? Node.ExplicitValueType : Node.ValueType).FullName;
            if (arrayTypeName.EndsWith("[]"))
            {
                arrayTypeName = arrayTypeName.Replace("[]", $"[{arrayParent.arraySize}]");
            }
            arrayParent.isExpanded = EditorGUI.Foldout(new Rect(valueRect) { x = valueRect.x + 10 }, arrayParent.isExpanded, arrayTypeName);
        }
        protected virtual void RenderMenu(Rect position, SerializedProperty parentNode, bool hasObjectReference)
        {
            float height = EditorGUIUtility.singleLineHeight;

            Rect titleRect = new Rect(position)
            {
                x = position.x + HorizontalSpacing,
                y = position.y + VerticalSpacing,
                width = ArrayMemberTitleWidth - HorizontalSpacing
            },
            lengthRect = new Rect(titleRect)
            {
                x = titleRect.x + titleRect.width + HorizontalSpacing,
                width = position.width - ArrayMemberTitleWidth - height * 2 - HorizontalSpacing * 4 - Padding
            },
            mbRect = new Rect(lengthRect)
            {
                x = lengthRect.x + lengthRect.width + HorizontalSpacing,
                height = height,
                width = height
            },
            pbRect = new Rect(mbRect)
            {
                x = mbRect.x + mbRect.width + HorizontalSpacing,
                height = height,
                width = height
            };

            int size = Node.ValueNode.arraySize;
            EditorGUI.LabelField(titleRect, "Length");
            int newSize = Mathf.Clamp(EditorGUI.IntField(lengthRect, size), 0, ushort.MaxValue);
            if (EditorGUI.Toggle(mbRect, false, "OL Minus") && size > 0)
            {
                newSize = size - 1;
            }
            if (EditorGUI.Toggle(pbRect, false, "OL Plus") && size < ushort.MaxValue)
            {
                newSize = size + 1;
            }
            if (newSize != size)
            {
                Node.ValueNode.arraySize = newSize;
                Dirty |= true;
            }

            if (hasObjectReference)
            {
                Rect optionsRect = new Rect(pbRect) { x = pbRect.x + height + VerticalSpacing };
                if (EditorGUI.Toggle(optionsRect, false, OperationStyle))
                {
                    Utility.PopupArrayComponents(
                        Root,
                        parentNode,
                        Node.ValueType.GetElementType()
                    );
                }
            }
        }
        protected virtual bool RenderMember(Rect position, int index, SerializedProperty memberNode)
        {
            Rect indexRect = new Rect(position)
            {
                x = position.x + HorizontalSpacing,
                width = ArrayMemberTitleWidth - HorizontalSpacing
            },
            valueRect = new Rect(indexRect)
            {
                x = indexRect.x + indexRect.width + HorizontalSpacing,
                width = position.width - indexRect.width - HorizontalSpacing - QuadWidth
            },
            optionsRect = new Rect(valueRect)
            {
                x = valueRect.x + valueRect.width + HorizontalSpacing,
                width = QuadWidth,
            };

            EditorGUI.LabelField(indexRect, $"Element {index}");
            RenderMemberValue(valueRect, memberNode, Node.ValueType.GetElementType());
            if (memberNode.propertyType == SerializedPropertyType.ObjectReference
                       && EditorGUI.Toggle(optionsRect, false, OperationStyle))
            {
                Utility.PopupComponents(
                    Root,
                    memberNode,
                    memberNode.objectReferenceValue,
                    Node.ValueType.GetElementType()
                );
            }
            return memberNode.propertyType == SerializedPropertyType.ObjectReference;
        }
        protected abstract void RenderMemberValue(Rect positon, SerializedProperty node, Type type);

        public override float GetHeight()
        {
            var height = base.GetLineHeight();
            if (Node != null && Node.ValueNode.isExpanded)
            {
                height += (base.GetLineHeight() + VerticalSpacing) * (Node.ValueNode.arraySize + 1);
                height += VerticalSpacingDouble;
            }
            return height;
        }
    }

    internal class UnknownRenderer : Renderer
    {
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                string typeName = (Node.ExplicitValueType != null ? Node.ExplicitValueType : Node.ValueType).FullName;
                EditorGUI.LabelField(valueRect, new GUIContent(typeName, $"未实现编辑的类型: {typeName}"), GUI.skin.textField);
            }
        }
    }


    [RenderTarget(typeof(XOR.Serializables.Number))]
    internal class NumberRenderer : Renderer
    {
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            var value = Node.ValueNode.doubleValue;
            var newValue = EditorGUI.DoubleField(valueRect, string.Empty, value);
            if (Math.Abs(newValue - value) > float.Epsilon)
            {
                Node.ValueNode.doubleValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.Bigint))]
    internal class BigintRenderer : Renderer
    {
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            var value = Node.ValueNode.longValue;
            var newValue = EditorGUI.LongField(valueRect, string.Empty, value);
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
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            var value = Node.ValueNode.stringValue;
            var newValue = EditorGUI.TextField(valueRect, string.Empty, value);
            if (newValue != value)
            {
                Node.ValueNode.stringValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.Boolean))]
    internal class BooleanRenderer : Renderer
    {
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            var value = Node.ValueNode.boolValue;
            var newValue = EditorGUI.Toggle(valueRect, string.Empty, value);
            if (newValue != value)
            {
                Node.ValueNode.boolValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.Vector2))]
    internal class Vector2Renderer : Renderer
    {
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            var value = Node.ValueNode.vector2Value;
            var newValue = EditorGUI.Vector2Field(valueRect, string.Empty, value);
            if (newValue != value)
            {
                Node.ValueNode.vector2Value = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.Vector3))]
    internal class Vector3Renderer : Renderer
    {
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            var value = Node.ValueNode.vector3Value;
            var newValue = EditorGUI.Vector3Field(valueRect, string.Empty, value);
            if (newValue != value)
            {
                Node.ValueNode.vector3Value = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.Object))]
    internal class ObjectRenderer : Renderer
    {
        protected override void RenderValue(Rect position, Rect valueRect)
        {
            var value = Node.ValueNode.objectReferenceValue;
            var newValue = EditorGUI.ObjectField(valueRect, string.Empty, value, typeof(UnityEngine.Object), true);
            if (newValue != value)
            {
                Node.ValueNode.objectReferenceValue = newValue;
                Dirty |= true;
            }
        }
        protected override void RenderOperation(Rect optionsRect)
        {
            if (EditorGUI.Toggle(optionsRect, false, OperationStyle))
            {
                Utility.PopupComponentsAndTypes(
                    Root,
                    Node,
                    Node.ValueNode.objectReferenceValue,
                    Node.ValueType
                );
            }
        }
    }


    [RenderTarget(typeof(XOR.Serializables.NumberArray))]
    internal class NumberArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(Rect valueRect, SerializedProperty node, Type type)
        {
            var value = node.doubleValue;
            var newValue = EditorGUI.DoubleField(valueRect, string.Empty, value);
            if (Math.Abs(newValue - value) > float.Epsilon)
            {
                node.doubleValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.BigintArray))]
    internal class BigintArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(Rect valueRect, SerializedProperty node, Type type)
        {
            var value = node.longValue;
            var newValue = EditorGUI.LongField(valueRect, string.Empty, value);
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
        protected override void RenderMemberValue(Rect valueRect, SerializedProperty node, Type type)
        {
            var value = node.stringValue;
            var newValue = EditorGUI.TextField(valueRect, string.Empty, value);
            if (newValue != value)
            {
                node.stringValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.BooleanArray))]
    internal class BooleanArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(Rect valueRect, SerializedProperty node, Type type)
        {
            var value = node.boolValue;
            var newValue = EditorGUI.Toggle(valueRect, string.Empty, value);
            if (newValue != value)
            {
                node.boolValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.Vector2Array))]
    internal class Vector2ArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(Rect valueRect, SerializedProperty node, Type type)
        {
            var value = node.vector2Value;
            var newValue = EditorGUI.Vector2Field(valueRect, string.Empty, value);
            if (newValue != value)
            {
                node.vector2Value = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.Vector3Array))]
    internal class Vector3ArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(Rect valueRect, SerializedProperty node, Type type)
        {
            var value = node.vector3Value;
            var newValue = EditorGUI.Vector3Field(valueRect, string.Empty, value);
            if (newValue != value)
            {
                node.vector3Value = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.Serializables.ObjectArray))]
    internal class ObjectArrayRenderer : ArrayRenderer
    {
        protected override void RenderMemberValue(Rect valueRect, SerializedProperty node, Type type)
        {
            var value = node.objectReferenceValue;
            var newValue = EditorGUI.ObjectField(valueRect, string.Empty, value, typeof(UnityEngine.Object), true);
            if (newValue != value)
            {
                node.objectReferenceValue = newValue;
                Dirty |= true;
            }
        }
    }
}