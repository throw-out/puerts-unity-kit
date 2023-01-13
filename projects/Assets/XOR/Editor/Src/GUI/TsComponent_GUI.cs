using UnityEditor;
using UnityEngine;
using XOR.Services;

namespace XOR
{
    [CustomEditor(typeof(TsComponent))]
    internal class TsComponent_GUI : Editor
    {
        private TsComponent component;
        private Statement statement;

        private static bool servicesStatusFoldout = true;
        private static bool componentFoldout = true;
        private static bool memberFoldout = true;

        void OnEnable()
        {
            component = target as TsComponent;
            statement = null;
            if (!EditorApplicationUtil.IsRunning())
            {
                servicesStatusFoldout = true;
            }
        }
        void OnDisable()
        {
            component = null;
            statement = null;
        }
        public override void OnInspectorGUI()
        {
            if (component == null)
            {
                return;
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
                componentFoldout = !componentFoldout;
            }
            if (componentFoldout)
            {
                using (new EditorGUI.DisabledScope(!EditorApplicationUtil.IsAvailable()))
                {
                    if (string.IsNullOrEmpty(component.m_Guid))
                    {
                        GUIUtil.RenderGroup(
                            _RenderModuleSelector
                        );
                    }
                    else
                    {
                        GUIUtil.RenderGroup(
                            _RenderModuleInfo,
                            _RenderModuleSelector
                        );
                    }
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

        void _RenderModuleSelector()
        {
            GUILayout.Button("选择模块");
        }
        void _RenderModuleInfo()
        {
            GUILayout.Label("SELECT");
        }
        void _RenderMembers()
        {
            GUILayout.Label("Empty");
        }
    }
}
