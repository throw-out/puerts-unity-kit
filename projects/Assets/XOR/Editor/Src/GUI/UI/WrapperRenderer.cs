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
        public SerializedProperty ArrayParent { get; private set; }
        public int ArrayIndex { get; private set; }


        private SerializedProperty _targetNode;
        private SerializedProperty _methodNode;
        private SerializedProperty _parameterNode;

        private SerializedProperty _stringValue;
        private SerializedProperty _floatValue;
        private SerializedProperty _boolValue;
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
        public SerializedProperty DoubleValueNode
        {
            get
            {
                if (_floatValue == null) _floatValue = this.Node.FindPropertyRelative("doubleValue");
                return _floatValue;
            }
        }
        public SerializedProperty BoolValueNode
        {
            get
            {
                if (_boolValue == null) _boolValue = this.Node.FindPropertyRelative("boolValue");
                return _boolValue;
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
        public XOR.Events.EventBaseParameter Parameter
        {
            get { return (XOR.Events.EventBaseParameter)this.ParameterNode.intValue; }
            set { this.ParameterNode.intValue = (int)value; }
        }

        public string StringValue
        {
            get { return this.StringValueNode.stringValue; }
            set { this.StringValueNode.stringValue = value; }
        }
        public double DoubleValue
        {
            get { return this.DoubleValueNode.doubleValue; }
            set { this.DoubleValueNode.doubleValue = value; }
        }
        public bool BoolValue
        {
            get { return this.BoolValueNode.boolValue; }
            set { this.BoolValueNode.boolValue = value; }
        }
        public UnityEngine.Object ObjectValue
        {
            get { return this.ObjectValueNode.objectReferenceValue; }
            set { this.ObjectValueNode.objectReferenceValue = value; }
        }

        public NodeWrap(SerializedProperty node, int arrayIndex, SerializedProperty arrayParent)
        {
            this.Node = node;
            this.ArrayParent = arrayParent;
            this.ArrayIndex = arrayIndex;
        }

        /// <summary>从(数组)父节点上移除当前节点 </summary>
        public void RemoveFromArrayParent()
        {
            ArrayParent.DeleteArrayElementAtIndex(ArrayIndex);
        }
        /// <summary>清除值存储的数据 </summary>
        public void Clean()
        {
            this.Target = null;
            this.Method = string.Empty;
            this.Parameter = XOR.Events.EventBaseParameter.None;
            this.StringValue = default;
            this.DoubleValue = default;
            this.BoolValue = default;
            this.ObjectValue = default;
        }
        public void DecreaseIndex()
        {
            this.ArrayIndex--;
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
        List<NodeWrap> CreateNodes()
        {
            if (elementParent == null || !elementParent.isArray)
                return null;

            var nodes = new List<NodeWrap>();
            for (int i = 0; i < elementParent.arraySize; i++)
            {
                nodes.Add(new NodeWrap(elementParent.GetArrayElementAtIndex(i), i, elementParent));
            }
            return nodes;
        }
        void CreateReorderableList()
        {
            if (elementParent == null || !elementParent.isArray)
                return;
            List<NodeWrap> nodes = CreateNodes();

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
                for (int i = index + 1; i < nodes.Count; i++)
                {
                    nodes[i].DecreaseIndex();
                }
                nodes[index].RemoveFromArrayParent();
                root.ApplyModifiedProperties();
            };
            list.onAddCallback = (list) =>
            {
                elementParent.arraySize++;
                int lastIndex = elementParent.arraySize - 1;
                nodes.Add(new NodeWrap(elementParent.GetArrayElementAtIndex(lastIndex), lastIndex, elementParent));
                root.ApplyModifiedProperties();
            };
        }
        void RenderNode(Rect rect, NodeWrap node)
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
                    if (GUI.Button(
                        new Rect(new Vector2(x + w * HeaderFixed + Spacing, y + 2f), new Vector2(w * (1 - HeaderFixed) - Spacing, h * 0.5f)),
                        string.IsNullOrEmpty(methodName) ? "No Function" : methodName,
                        Skin.dropDown
                    ))
                    {
                        Helper.DropdownMethodsMenu(node, (compoent, md) =>
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
                if (newGo != null && !string.IsNullOrEmpty(methodName) && node.Parameter != XOR.Events.EventBaseParameter.None)
                {
                    var parameterRect = new Rect(new Vector2(x + w * HeaderFixed + Spacing, y + h * 0.5f), new Vector2(w * (1 - HeaderFixed) - Spacing, h * 0.5f));
                    switch (node.Parameter)
                    {
                        case XOR.Events.EventBaseParameter.String:
                            node.StringValue = EditorGUI.TextField(parameterRect, node.StringValue);
                            break;
                        case XOR.Events.EventBaseParameter.Double:
                            node.DoubleValue = EditorGUI.DoubleField(parameterRect, node.DoubleValue);
                            break;
                        case XOR.Events.EventBaseParameter.Bool:
                            node.BoolValue = EditorGUI.Toggle(parameterRect, node.BoolValue);
                            break;
                        case XOR.Events.EventBaseParameter.Object:
                            var md = Helper.GetMethodDeclaration(node);
                            var targetType = md != null && md.parameterTypes != null && md.parameterTypes.Length > 0 ? md.parameterTypes[0] : typeof(UnityEngine.Object);
                            var targetValue = node.ObjectValue != null && targetType.IsAssignableFrom(node.ObjectValue.GetType()) ? node.ObjectValue : null;
                            using (new EditorGUI.DisabledScope(md == null))
                            {
                                node.ObjectValue = EditorGUI.ObjectField(parameterRect, targetValue, targetType, true);
                            }
                            break;
                        case XOR.Events.EventBaseParameter.Post:
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
                var program = EditorApplicationUtil.GetProgram();
                if (program == null || program.state != XOR.Services.ProgramState.Completed)
                {
                    GUIUtil.RenderMustLaunchServices();
                    return;
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
                    var header = components.Length > 1 ? $"{nameof(TsComponent)}-{i + 1}/" : $"{nameof(TsComponent)}/";
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
                menu.ShowAsContext();
            }

            public static XOR.Events.EventBaseParameter GetMethodParameter(XOR.Services.MethodDeclaration method)
            {
                Type firstParameterType = method.parameterTypes != null && method.parameterTypes.Length > 0 ? method.parameterTypes[0] : null;
                if (firstParameterType == null)
                {
                    return XOR.Events.EventBaseParameter.None;
                }
                XOR.Events.EventBaseParameter type;
                if (!methodParameters.TryGetValue(firstParameterType, out type))
                {
                    type = XOR.Events.EventBaseParameter.Object;
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

            static Dictionary<Type, XOR.Events.EventBaseParameter> methodParameters = new Dictionary<Type, XOR.Events.EventBaseParameter>(){
                {typeof(bool), XOR.Events.EventBaseParameter.Bool},
                {typeof(string), XOR.Events.EventBaseParameter.String},
                {typeof(double), XOR.Events.EventBaseParameter.Double},
                {typeof(UnityEngine.Object), XOR.Events.EventBaseParameter.Object},
            };
            static Dictionary<XOR.Events.EventBaseParameter, Type> methodParametersReverse = new Dictionary<XOR.Events.EventBaseParameter, Type>(
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