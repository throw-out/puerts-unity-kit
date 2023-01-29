using System;
using System.Collections.Generic;
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
        private List<NodeWrap> nodes;
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
            RebuildNodes();
        }
        void OnDisable()
        {
            component = null;
        }

        void RebuildNodes()
        {
            //创建当前需要渲染的节点信息
            nodes = new List<NodeWrap>();
            foreach (var map in root.TypeMapping)
            {
                SerializedProperty arrayParent = root.GetProperty(map.Key);
                for (int i = 0; i < arrayParent.arraySize; i++)
                {
                    nodes.Add(new NodeWrap(arrayParent.GetArrayElementAtIndex(i), map.Value, i, arrayParent));
                }
            }
        }
        void RebuildVersion()
        {
            if (statement == null || !(statement is TypeDeclaration))
                return;
            TypeDeclaration type = (TypeDeclaration)statement;
        }

        public override void OnInspectorGUI()
        {
            if (component == null)
            {
                return;
            }
            statement = EditorApplicationUtil.GetStatement(ComponentUtil.GetGuid(component));
            if (statement != null && statement.version != ComponentUtil.GetVersion(component))
            {
                ComponentUtil.SetVersion(component, statement.version);
                RebuildVersion();
                RebuildNodes();
            }

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
                using (new EditorGUI.DisabledScope(!EditorApplicationUtil.IsAvailable()))
                {
                    GUIUtil.RenderGroup(_RenderMembers);
                }
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }

        void _RenderModule()
        {
            string guid = component != null ? ComponentUtil.GetGuid(component) : string.Empty;
            string route = component != null ? ComponentUtil.GetRoute(component) : string.Empty;
            string version = component != null ? ComponentUtil.GetVersion(component) : string.Empty;

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
            using (new EditorGUI.DisabledScope(statement == null))
            {
                if (GUILayout.Button("打开脚本"))
                {

                }
            }
            if (GUILayout.Button("选择模块"))
            {
                ModuleSelector.Show(EditorApplicationUtil.GetProgram(), OnSelectorCallback, moduleSelectorRect);
            }
            if (Event.current.type == EventType.Repaint)
            {
                moduleSelectorRect = GUILayoutUtility.GetLastRect();
            }
            GUILayout.EndHorizontal();
        }

        void _RenderMembers()
        {
            GUILayout.Label("Empty");
        }

        void OnSelectorCallback(string guid)
        {
            if (component == null)
                return;
            ComponentUtil.SetGuid(component, guid);
            SetDirty();
        }
        new void SetDirty()
        {
            if (component == null)
                return;
            EditorUtility.SetDirty(component);
        }


    }
}
