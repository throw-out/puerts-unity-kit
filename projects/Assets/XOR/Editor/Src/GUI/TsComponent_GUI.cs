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
            RenderComponent();
            RenderComponentMembers();

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

        void RenderComponent()
        {
            if (GUIUtil.RenderHeader("脚本"))
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
                            _RenderComponentSelector
                        );
                    }
                    else
                    {
                        GUIUtil.RenderGroup(
                            _RenderComponentInfo,
                            _RenderComponentSelector
                        );
                    }
                }
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }
        void RenderComponentMembers()
        {
            if (GUIUtil.RenderHeader("成员属性"))
            {
                memberFoldout = !memberFoldout;
            }
            if (memberFoldout)
            {
                using (new EditorGUI.DisabledScope(!EditorApplicationUtil.IsAvailable()))
                {
                    GUIUtil.RenderGroup(_RenderComponentMembers);
                }
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }

        void _RenderComponentSelector()
        {
            GUILayout.Button("选择模块");
        }
        void _RenderComponentInfo()
        {
            GUILayout.Label("SELECT");
        }
        void _RenderComponentMembers()
        {
            GUILayout.Label("Empty");
        }
    }
}
