using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XOR.Serializables;
using XOR.Services;

namespace XOR
{
    [CustomEditor(typeof(TsComponent))]
    internal class TsComponentEditor : Editor
    {
        private const int ModuleHeaderWidth = 60;

        private static bool servicesStatusFoldout = true;
        private static bool moduleFoldout = true;
        private static bool memberFoldout = true;

        private TsComponent component;
        private Statement statement;

        private Rect moduleSelectorRect;

        private SerializedObjectWrap root;
        private List<NodeWrap> properties;
        private XOR.Serializables.TsComponent.Display display;

        void OnEnable()
        {
            component = target as TsComponent;
            if (!EditorApplicationUtil.IsRunning())
            {
                servicesStatusFoldout = true;
            }
            //初始化节点信息
            root = SerializedObjectWrap.Create(serializedObject, typeof(TsComponent));
            properties = new List<NodeWrap>();
            display = XOR.Serializables.TsComponent.Display.Create();
        }
        void OnDisable()
        {
            component = null;
        }

        public override void OnInspectorGUI()
        {
            if (component == null)
            {
                return;
            }
            statement = EditorApplicationUtil.GetStatement(Helper.GetGuid(component));
            if (statement != null && statement.version != Helper.GetVersion(component))
            {
                Helper.RebuildProperties(component, statement);
            }
            Helper.RebuildNodes(root, properties, statement);

            EditorGUILayout.BeginVertical();

            RenderServicesStatus();
            RenderModule();
            RenderMembers();

            EditorGUILayout.EndVertical();
        }

        void RenderServicesStatus()
        {
            if (GUIUtil.RenderStatusHeader())
            {
                servicesStatusFoldout = !servicesStatusFoldout;
            }
            if (servicesStatusFoldout)
            {
                GUIUtil.RenderStatusContent();
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }
        void RenderModule()
        {
            if (GUIUtil.RenderHeader("模块"))
            {
                moduleFoldout = !moduleFoldout;
            }
            if (moduleFoldout)
            {
                using (new EditorGUI.DisabledScope(!EditorApplicationUtil.IsAvailable()))
                {
                    GUIUtil.RenderGroup(
                        _RenderModule,
                        _RenderModuleSelector
                    );
                }
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }
        void RenderMembers()
        {
            if (GUIUtil.RenderHeader("成员属性"))
            {
                memberFoldout = !memberFoldout;
            }
            if (memberFoldout)
            {
                using (new EditorGUI.DisabledScope(statement == null))
                {
                    GUIUtil.RenderGroup(_RenderPropertiesNodes);
                }
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }

        void _RenderModule()
        {
            string guid = component != null ? Helper.GetGuid(component) : string.Empty;
            string route = component != null ? Helper.GetRoute(component) : string.Empty;
            string version = component != null ? Helper.GetVersion(component) : string.Empty;

            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("GUID:", GUILayout.Width(ModuleHeaderWidth));
                GUILayout.Label(guid, Skin.label);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Type:", GUILayout.Width(ModuleHeaderWidth));
                if (statement != null) GUILayout.Label(new GUIContent(statement.name, statement.name), Skin.label);
                else GUILayout.Label("module missing", Skin.label);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Module:", GUILayout.Width(ModuleHeaderWidth));
                if (statement != null) GUILayout.Label(new GUIContent(statement.module, statement.module), Skin.label);
                else GUILayout.Label("module missing", Skin.label);
                GUILayout.EndHorizontal();
            }
        }
        void _RenderModuleSelector()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("模块"))
            {
                ModuleSelector.Show(EditorApplicationUtil.GetProgram(), OnSelectorCallback, moduleSelectorRect);
            }
            if (Event.current.type == EventType.Repaint)
            {
                moduleSelectorRect = GUILayoutUtility.GetLastRect();
            }
            using (new EditorGUI.DisabledScope(statement == null))
            {
                if (GUILayout.Button("重置"))
                {
                    Helper.ClearProperties(component);
                    //Helper.RebuildNodes(root, properties, statement);
                }
                if (GUILayout.Button("编辑"))
                {

                }
            }
            GUILayout.EndHorizontal();
        }
        void _RenderPropertiesNodes()
        {
            if (display == null || properties == null || properties.Count == 0)
            {
                GUILayout.Label("Empty");
                return;
            }
            bool dirty = false;
            foreach (var node in properties)
            {
                dirty |= display.Render(node);
            }
            if (dirty)
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                Helper.SetDirty(component);
            }
        }

        void OnSelectorCallback(string guid)
        {
            if (component == null)
                return;
            Helper.ClearProperties(component);
            Helper.SetGuid(component, guid);
            Helper.SetDirty(component);
        }

        static class Helper
        {
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            static FieldInfo _guidField;
            static FieldInfo _routeField;
            static FieldInfo _versionField;
            static FieldInfo guidField
            {
                get
                {
                    if (_guidField == null) _guidField = typeof(TsComponent).GetField("guid", Flags);
                    return _guidField;
                }
            }
            static FieldInfo routeField
            {
                get
                {
                    if (_routeField == null) _routeField = typeof(TsComponent).GetField("route", Flags);
                    return _routeField;
                }
            }
            static FieldInfo versionField
            {
                get
                {
                    if (_versionField == null) _versionField = typeof(TsComponent).GetField("version", Flags);
                    return _versionField;
                }
            }

            public static string GetGuid(TsComponent component)
            {
                return guidField.GetValue(component) as string;
            }
            public static string GetRoute(TsComponent component)
            {
                return routeField.GetValue(component) as string;
            }
            public static string GetVersion(TsComponent component)
            {
                return versionField.GetValue(component) as string;
            }
            public static void SetGuid(TsComponent component, string value)
            {
                guidField.SetValue(component, value);
            }
            public static void SetRoute(TsComponent component, string value)
            {
                routeField.SetValue(component, value);
            }
            public static void SetVersion(TsComponent component, string value)
            {
                versionField.SetValue(component, value);
            }

            public static void RebuildNodes(SerializedObjectWrap root, List<NodeWrap> outputNodes, Statement statement)
            {
                if (outputNodes == null)
                    return;
                outputNodes.Clear();
                //创建当前需要渲染的节点信息
                foreach (var info in root.FieldMapping)
                {
                    SerializedProperty arrayParent = root.GetProperty(info.Key);
                    for (int i = 0; i < arrayParent.arraySize; i++)
                    {
                        var nw = new NodeWrap(arrayParent.GetArrayElementAtIndex(i), info.Value.Element.Type, i, arrayParent);
                        if (statement != null && statement is TypeDeclaration type && type.Properties.TryGetValue(nw.Key, out var property))
                        {
                            nw.ExplicitValueType = property.valueType;
                            nw.ExplicitValueRange = property.valueRange;
                            nw.ExplicitValueEnum = property.valueEnum;
                            nw.Tooltip = property.BuildTooltip();
                        }
                        outputNodes.Add(nw);
                    }
                }
                outputNodes.Sort((n1, n2) => n1.Index < n2.Index ? -1 : n1.Index < n2.Index ? 1 : 0);
            }
            public static void RebuildProperties(TsComponent component, Statement statement)
            {
                if (statement == null || !(statement is TypeDeclaration))
                    return;
                TypeDeclaration type = (TypeDeclaration)statement;

                ComponentWrap<TsComponent> cw = ComponentWrap<TsComponent>.Create();
                //移除无效的节点
                Dictionary<string, IPair> properties = cw.GetProperties(component)?.ToDictionary(pair => pair.Key);
                if (properties != null)
                {
                    foreach (string key in properties.Keys.ToArray())
                    {
                        if (!type.Properties.ContainsKey(key) ||
                            !cw.IsExplicitPropertyValue(component, key, type.Properties[key].valueType)    //检测当前值兼容性
                        )
                        {
                            properties.Remove(key);
                            cw.RemoveProperty(component, key);
                        }
                    }
                }
                //添加未序列化的节点
                int index = 0;
                foreach (var property in type.Properties.Values)
                {
                    IPair current = null;
                    properties?.TryGetValue(property.name, out current);
                    if (current == null)
                    {
                        if (cw.AddProperty(component, property.valueType, property.name, index))
                        {
                            if (property.defaultValue != null) cw.SetPropertyValue(component, property.name, property.defaultValue);
                        }
                        else
                        {
                            Debug.LogWarning($"{nameof(XOR.TsComponent)} unsupport type: {property.valueType.FullName}");
                        }
                    }
                    else if (current.Index != index)
                    {
                        cw.SetPropertyIndex(component, property.name, index);
                    }
                    index++;
                }

                Helper.SetVersion(component, statement.version);
                Helper.SetDirty(component);
            }
            public static void ClearProperties(TsComponent component)
            {
                ComponentWrap<TsComponent> cw = ComponentWrap<TsComponent>.Create();
                cw.ClearProperties(component);

                Helper.SetVersion(component, default);
                Helper.SetDirty(component);
            }

            public static void SetDirty(UnityEngine.Object obj)
            {
                if (obj == null)
                    return;
                EditorUtility.SetDirty(obj);
            }
        }
    }
}
