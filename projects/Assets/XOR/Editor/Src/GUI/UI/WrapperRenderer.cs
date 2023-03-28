using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XOR.UI
{
    internal class NodeWrap
    {
        public SerializedProperty Node { get; private set; }

        private SerializedProperty _targetNode;
        private SerializedProperty _methodNode;
        private SerializedProperty _parameterNode;

        private SerializedProperty _stringValue;
        private SerializedProperty _objectValue;

        public SerializedProperty TragetNode
        {
            get
            {
                if (_targetNode == null) _targetNode = this.Node.FindPropertyRelative("target");
                return _targetNode;
            }
        }
        public SerializedProperty MethodNode
        {
            get
            {
                if (_methodNode == null) _methodNode = this.Node.FindPropertyRelative("method");
                return _methodNode;
            }
        }
        public SerializedProperty ParameterNode
        {
            get
            {
                if (_parameterNode == null) _parameterNode = this.Node.FindPropertyRelative("parameter");
                return _parameterNode;
            }
        }

        public SerializedProperty StringValueNode
        {
            get
            {
                if (_stringValue == null) _stringValue = this.Node.FindPropertyRelative("stringValue");
                return _stringValue;
            }
        }
        public SerializedProperty ObjectValueNode
        {
            get
            {
                if (_objectValue == null) _objectValue = this.Node.FindPropertyRelative("objectValue");
                return _objectValue;
            }
        }

        public UnityEngine.Object Target
        {
            get { return this.TragetNode.objectReferenceValue; }
            set { this.TragetNode.objectReferenceValue = value; }
        }
        public string Method
        {
            get { return this.MethodNode.stringValue; }
            set { this.MethodNode.stringValue = value; }
        }
        public XOR.Events.ParameterType Parameter
        {
            get { return (XOR.Events.ParameterType)this.ParameterNode.intValue; }
            set { this.ParameterNode.intValue = (int)value; }
        }

        public string StringValue
        {
            get { return this.StringValueNode.stringValue; }
            set { this.StringValueNode.stringValue = value; }
        }
        public double DoubleValue
        {
            get { return XOR.Events.Serializer.ToData<double>(this.StringValueNode.stringValue); }
            set { this.StringValueNode.stringValue = XOR.Events.Serializer.ToString(value); }
        }
        public bool BoolValue
        {
            get { return XOR.Events.Serializer.ToData<bool>(this.StringValueNode.stringValue); }
            set { this.StringValueNode.stringValue = XOR.Events.Serializer.ToString(value); }
        }
        public long LongValue
        {
            get { return XOR.Events.Serializer.ToData<long>(this.StringValueNode.stringValue); }
            set { this.StringValueNode.stringValue = XOR.Events.Serializer.ToString(value); }
        }
        public Vector2 Vector2Value
        {
            get { return XOR.Events.Serializer.ToData<Vector2>(this.StringValueNode.stringValue); }
            set { this.StringValueNode.stringValue = XOR.Events.Serializer.ToString(value); }
        }
        public Vector3 Vector3Value
        {
            get { return XOR.Events.Serializer.ToData<Vector3>(this.StringValueNode.stringValue); }
            set { this.StringValueNode.stringValue = XOR.Events.Serializer.ToString(value); }
        }
        public Color ColorValue
        {
            get { return XOR.Events.Serializer.ToData<Color>(this.StringValueNode.stringValue); }
            set { this.StringValueNode.stringValue = XOR.Events.Serializer.ToString(value); }
        }
        public Color32 Color32Value
        {
            get { return XOR.Events.Serializer.ToData<Color32>(this.StringValueNode.stringValue); }
            set { this.StringValueNode.stringValue = XOR.Events.Serializer.ToString(value); }
        }
        public UnityEngine.Object ObjectValue
        {
            get { return this.ObjectValueNode.objectReferenceValue; }
            set { this.ObjectValueNode.objectReferenceValue = value; }
        }

        public NodeWrap(SerializedProperty node)
        {
            this.Node = node;
        }

        /// <summary>清除值存储的数据 </summary>
        public void Clean()
        {
            this.Target = null;
            this.Method = string.Empty;
            this.Parameter = XOR.Events.ParameterType.None;
            this.StringValue = default;
        }
        public void ApplyModifiedProperties()
        {
            Node.serializedObject.ApplyModifiedProperties();
        }
    }
    internal class WrapperRenderer
    {
        const float HeaderFixed = 0.3f;
        const float Spacing = 5f;

        private SerializedObject root;
        private SerializedProperty elementParent;
        private ReorderableList list;
        public string Header { get; set; }

        public WrapperRenderer(SerializedProperty elementParent)
        {
            this.root = elementParent != null ? elementParent.serializedObject : null;
            this.elementParent = elementParent;
            this.CreateReorderableList();
        }
        public void Renderer()
        {
            if (list == null)
                return;
            list.DoLayoutList();
        }

        void CreateReorderableList()
        {
            if (elementParent == null || !elementParent.isArray)
                return;
            List<NodeWrap> nodes = RebuildNodes(elementParent);

            list = new ReorderableList(nodes, typeof(NodeWrap),
                false, true, true, true
            );
            list.elementHeightCallback = (index) =>
            {
                return EditorGUIUtility.singleLineHeight * 2 + Spacing + 4f;
            };
            list.drawHeaderCallback = (rect) =>
            {   //绘制表头
                EditorGUI.LabelField(rect, Header);
            };
            list.drawElementCallback = (rect, index, selected, focused) =>
            {   //绘制元素
                //EditorGUI.PropertyField(rect, elements.GetArrayElementAtIndex(index), true);
                rect.height -= Spacing - 4f;
                RenderNode(rect, nodes[index]);
            };
            list.onRemoveCallback = (list) =>
            {
                int index = list.index;
                elementParent.DeleteArrayElementAtIndex(index);
                root.ApplyModifiedProperties();
                RebuildNodes(elementParent, in nodes);
            };
            list.onAddCallback = (list) =>
            {
                elementParent.arraySize++;
                root.ApplyModifiedProperties();
                int lastIndex = elementParent.arraySize - 1;
                nodes.Add(new NodeWrap(elementParent.GetArrayElementAtIndex(lastIndex)));
            };
        }
        static List<NodeWrap> RebuildNodes(SerializedProperty parent)
        {
            if (parent == null || !parent.isArray)
                return null;
            var nodes = new List<NodeWrap>();
            RebuildNodes(parent, in nodes);
            return nodes;
        }
        static void RebuildNodes(SerializedProperty parent, in List<NodeWrap> nodes)
        {
            nodes.Clear();
            for (int i = 0; i < parent.arraySize; i++)
            {
                nodes.Add(new NodeWrap(parent.GetArrayElementAtIndex(i)));
            }
        }
        static void RenderNode(Rect rect, NodeWrap node)
        {
            float x = rect.x, y = rect.y, w = rect.width, h = rect.height;

            using (new EditorGUI.PropertyScope(rect, GUIContent.none, node.Node))
            {
                //Render Type
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.EnumPopup(new Rect(new Vector2(x, y + 2f), new Vector2(w * HeaderFixed, h * 0.5f)), UnityEngine.Events.UnityEventCallState.RuntimeOnly);
                }

                //Render GameObject
                var go = Helper.GetGamObject(node.Target);
                var newGo = EditorGUI.ObjectField(new Rect(new Vector2(x, y + h * 0.5f), new Vector2(w * HeaderFixed, h * 0.5f)), go, typeof(GameObject), true) as GameObject;
                if (newGo != go)
                {
                    node.Clean();                           //clear
                    node.Target = newGo;
                    node.ApplyModifiedProperties();
                }

                //Render Method
                string methodName = newGo != null ? node.Method : string.Empty;
                using (new EditorGUI.DisabledScope(newGo == null))
                {
                    var position = new Rect(new Vector2(x + w * HeaderFixed + Spacing, y + 2f), new Vector2(w * (1 - HeaderFixed) - Spacing, h * 0.5f));
                    if (GUI.Button(
                        new Rect(new Vector2(x + w * HeaderFixed + Spacing, y + 2f), new Vector2(w * (1 - HeaderFixed) - Spacing, h * 0.5f)),
                        string.IsNullOrEmpty(methodName) ? "No Function" : methodName,
                        Skin.dropDown
                    ))
                    {
                        Helper.DropdownMethodsMenu(node, position, (compoent, md) =>
                        {
                            node.Clean();
                            node.Target = compoent;
                            if (md != null)
                            {
                                node.Method = md.name;
                                node.Parameter = Helper.GetMethodParameter(md);
                            }
                            node.ApplyModifiedProperties();
                        });
                    }
                }
                //Render Parameter
                if (newGo != null && !string.IsNullOrEmpty(methodName) && node.Parameter != XOR.Events.ParameterType.None)
                {
                    var parameterRect = new Rect(new Vector2(x + w * HeaderFixed + Spacing, y + h * 0.5f), new Vector2(w * (1 - HeaderFixed) - Spacing, h * 0.5f));
                    switch (node.Parameter)
                    {
                        case XOR.Events.ParameterType.String:
                            node.StringValue = EditorGUI.TextField(parameterRect, node.StringValue);
                            break;
                        case XOR.Events.ParameterType.Double:
                            node.DoubleValue = EditorGUI.DoubleField(parameterRect, node.DoubleValue);
                            break;
                        case XOR.Events.ParameterType.Bool:
                            node.BoolValue = EditorGUI.Toggle(parameterRect, node.BoolValue);
                            break;
                        case XOR.Events.ParameterType.Long:
                            node.LongValue = EditorGUI.LongField(parameterRect, node.LongValue);
                            break;
                        case XOR.Events.ParameterType.Vector2:
                            node.Vector2Value = EditorGUI.Vector2Field(parameterRect, string.Empty, node.Vector2Value);
                            break;
                        case XOR.Events.ParameterType.Vector3:
                            node.Vector3Value = EditorGUI.Vector3Field(parameterRect, string.Empty, node.Vector3Value);
                            break;
                        case XOR.Events.ParameterType.Color:
                        case XOR.Events.ParameterType.Color32:
                            node.ColorValue = EditorGUI.ColorField(parameterRect, node.ColorValue);
                            break;
                        case XOR.Events.ParameterType.Object:
                            var md = Helper.GetMethodDeclaration(node);
                            var targetType = md != null && md.parameterTypes != null && md.parameterTypes.Length > 0 ? md.parameterTypes[0] : typeof(UnityEngine.Object);
                            var targetValue = node.ObjectValue != null && targetType.IsAssignableFrom(node.ObjectValue.GetType()) ? node.ObjectValue : null;
                            using (new EditorGUI.DisabledScope(md == null))
                            {
                                node.ObjectValue = EditorGUI.ObjectField(parameterRect, targetValue, targetType, true);
                            }
                            break;
                        case XOR.Events.ParameterType.Post:
                            break;
                    }
                    node.ApplyModifiedProperties();
                }
            }
        }

        private static class Helper
        {
            public static GameObject GetGamObject(UnityEngine.Object target)
            {
                if (target == null)
                    return null;
                if (target is GameObject)
                {
                    return (GameObject)target;
                }
                if (target is Component)
                {
                    return ((Component)target).gameObject;
                }
                return null;
            }
            public static void DropdownMethodsMenu(NodeWrap node, Action<UnityEngine.Object, XOR.Services.MethodDeclaration> callback)
            {
                var menu = GenerateMethodsMenu(node, callback);
                if (menu != null) menu.ShowAsContext();
            }
            public static void DropdownMethodsMenu(NodeWrap node, Rect position, Action<UnityEngine.Object, XOR.Services.MethodDeclaration> callback)
            {
                var menu = GenerateMethodsMenu(node, callback);
                if (menu != null) menu.DropDown(position);
            }

            public static XOR.Events.ParameterType GetMethodParameter(XOR.Services.MethodDeclaration method)
            {
                Type firstParameterType = method.parameterTypes != null && method.parameterTypes.Length > 0 ? method.parameterTypes[0] : null;
                if (firstParameterType == null)
                {
                    return XOR.Events.ParameterType.None;
                }
                XOR.Events.ParameterType type;
                if (!methodParameters.TryGetValue(firstParameterType, out type))
                {
                    type = XOR.Events.ParameterType.Object;
                }
                return type;
            }
            public static XOR.Services.MethodDeclaration GetMethodDeclaration(NodeWrap node)
            {
                if (string.IsNullOrEmpty(node.Method) || !(node.Target is XOR.TsComponent component))
                    return null;
                var program = EditorApplicationUtil.GetProgram();
                if (program == null)
                {
                    return null;
                }
                var statement = program.GetStatement(component.GetGuid());
                if (statement == null || !(statement is XOR.Services.TypeDeclaration type))
                {
                    return null;
                }
                var methods = type.GetMethods(node.Method);
                if (methods != null)
                {
                    return methods.FirstOrDefault(m => IsMatchMethod(node, m, methods.Length));
                }
                return null;
            }


            static UnityEditor.GenericMenu GenerateMethodsMenu(NodeWrap node, Action<UnityEngine.Object, XOR.Services.MethodDeclaration> callback)
            {
                var program = EditorApplicationUtil.GetProgram();
                if (program == null || program.state != XOR.Services.ProgramState.Completed)
                {
                    GUIUtil.RenderMustLaunchServices();
                    return null;
                }
                GameObject target = GetGamObject(node.Target);
                string methodName = node.Method;

                var menu = new UnityEditor.GenericMenu();
                menu.allowDuplicateNames = true;
                menu.AddItem(new GUIContent($"No Function"), false, () => callback(target, null));
                menu.AddSeparator(string.Empty);

                var components = target.GetComponents<XOR.TsComponent>();
                for (int i = 0; i < components.Length; i++)
                {
                    var header = components.Length > 1 ? $"{nameof(TsComponent)}({i + 1})/" : $"{nameof(TsComponent)}/";
                    var component = components[i];
                    var statement = program.GetStatement(component.GetGuid());
                    if (statement == null || !(statement is XOR.Services.TypeDeclaration type))
                    {
                        menu.AddDisabledItem(new GUIContent($"{header}Missing"), false);
                        continue;
                    }
                    if (!type.HasMethods())
                    {
                        menu.AddDisabledItem(new GUIContent($"{header}Empty"), false);
                        continue;
                    }
                    foreach (var method in type.GetMethods())
                    {
                        if (!IsAvailableMethod(method))
                            continue;
                        menu.AddItem(
                            new GUIContent($"{header}{method.BuildTooltip()}"),
                            methodName == method.name && IsMatchMethod(node, method, type.GetMethodsCount(methodName)),
                            () => callback(component, method)
                        );
                    }
                }
                return menu;
            }

            static Dictionary<Type, XOR.Events.ParameterType> methodParameters = new Dictionary<Type, XOR.Events.ParameterType>(){
                {typeof(string), XOR.Events.ParameterType.String},
                {typeof(double), XOR.Events.ParameterType.Double},
                {typeof(bool), XOR.Events.ParameterType.Bool},
                {typeof(long), XOR.Events.ParameterType.Long},
                {typeof(UnityEngine.Vector2), XOR.Events.ParameterType.Vector2},
                {typeof(UnityEngine.Vector3), XOR.Events.ParameterType.Vector3},
                {typeof(UnityEngine.Color), XOR.Events.ParameterType.Color},
                {typeof(UnityEngine.Color32), XOR.Events.ParameterType.Color32},
                {typeof(UnityEngine.Object), XOR.Events.ParameterType.Object},
            };
            static Dictionary<XOR.Events.ParameterType, Type> methodParametersReverse = new Dictionary<XOR.Events.ParameterType, Type>(
                methodParameters.ToDictionary(o => o.Value, o => o.Key)
            );
            static bool IsAvailableMethod(XOR.Services.MethodDeclaration method)
            {
                if (method.parameterTypes == null || method.parameterTypes.Length == 0)
                    return true;
                if (method.parameterTypes.Length > 1)
                    return false;
                return methodParameters.ContainsKey(method.parameterTypes[0]) ||
                    typeof(UnityEngine.Object).IsAssignableFrom(method.parameterTypes[0]);
            }
            static bool IsMatchMethod(NodeWrap node, XOR.Services.MethodDeclaration method, int methodCount)
            {
                if (methodCount <= 1)
                    return true;
                Type firstParameterType = method.parameterTypes != null && method.parameterTypes.Length > 0 ? method.parameterTypes[0] : null;
                Type targetType;
                methodParametersReverse.TryGetValue(node.Parameter, out targetType);

                return firstParameterType == targetType ||
                    firstParameterType != null && targetType == typeof(UnityEngine.Object) && targetType.IsAssignableFrom(firstParameterType);
            }
        }
    }
}