using UnityEditor;
using UnityEngine;

namespace XOR
{
    [CustomEditor(typeof(TsComponent))]
    internal class TsComponent_GUI : Editor
    {
        private TsComponent component;

        private bool statusFoldout = true;

        void OnEnable()
        {
            component = target as TsComponent;
        }
        void OnDisable()
        {
            component = null;
        }
        public override void OnInspectorGUI()
        {
            if (component == null)
            {
                base.OnInspectorGUI();
                return;
            }
            EditorGUILayout.BeginVertical();

            statusFoldout = EditorGUILayout.Foldout(statusFoldout, $"状态: 主线程({GUIUtil.Status(EditorApplicationUtil.IsRunning())}) | 工作线程(${GUIUtil.Status(EditorApplicationUtil.IsWorkerRunning())})");
            if (statusFoldout)
            {
                RendererStatus();
            }

            EditorGUILayout.EndVertical();
        }

        void RendererStatus()
        {
            GUIUtil.RendererApplicationStatus();
        }
    }
}
