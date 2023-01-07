using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    [CustomEditor(typeof(TsProxy))]
    //[CanEditMultipleObjects]
    public class TsProxy_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("open module"))
            {
                var instance = target as TsProxy;
                if (!File.Exists(instance.m_ModulePath))
                    throw new Exception("Unknow file: " + instance.m_ModulePath);

                Unity.CodeEditor.CodeEditor.CurrentEditor.OpenProject(instance.m_ModulePath, instance.m_Line, 0);
            }
            if (GUILayout.Button("module stack"))
            {
                var instance = target as TsProxy;
                Debug.Log(instance.m_ModuleStack);
            }
        }
    }
}
