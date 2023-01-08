using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    [CustomEditor(typeof(TsBehaviour))]
    //[CanEditMultipleObjects]
    public class TsBehaviour_GUI : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open Module"))
            {
                var instance = target as TsBehaviour;
                if (!File.Exists(instance.m_ModulePath))
                    throw new Exception("Unknow file: " + instance.m_ModulePath);

#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
                Unity.CodeEditor.CodeEditor.CurrentEditor.OpenProject(instance.m_ModulePath, instance.m_Line, 0);
#else
                UnityEngine.Debug.LogWarning($"Unsupported unity version: {UnityEngine.Application.version}");
#endif
            }
            if (GUILayout.Button("Module Stack"))
            {
                var instance = target as TsBehaviour;
                Debug.Log(instance.m_ModuleStack);
            }
        }
    }
}
