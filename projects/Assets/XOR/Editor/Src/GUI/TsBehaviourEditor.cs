using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    //[CanEditMultipleObjects]
    [CustomEditor(typeof(TsBehaviour))]
    internal class TsBehaviourEditor : Editor
    {
        private TsBehaviour component;
        void OnEnable()
        {
            component = target as TsBehaviour;
        }
        public override void OnInspectorGUI()
        {
            if (component == null)
            {
                return;
            }
            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("ClassName");
                GUILayout.Label(component.Module?.className ?? string.Empty);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("ModuleName");
                GUILayout.Label(component.Module?.moduleName ?? string.Empty);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("ModulePath");
                GUILayout.Label(component.Module?.modulePath ?? string.Empty);
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Open Module") && component.Module != null)
            {
                if (!File.Exists(component.Module.modulePath))
                    throw new Exception("Unknow File: " + component.Module.modulePath);

                FileUtil.OpenFileInIDE(component.Module.modulePath, component.Module.line, 0);
            }
            if (GUILayout.Button("Module Stack") && component.Module != null)
            {
                Debug.Log(component.Module.stack);
            }
        }
    }
}
