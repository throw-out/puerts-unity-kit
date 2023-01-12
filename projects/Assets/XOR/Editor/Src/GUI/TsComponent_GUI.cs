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

        private bool servicesStatusFoldout = true;
        private bool componentFoldout = true;
        private bool memberFoldout = true;

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
