using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace XOR.UI
{
    [CustomEditor(typeof(XOR.UI.EventTriggerWrapper))]
    internal class EventTriggerEditor : UnityEditor.EventSystems.EventTriggerEditor
    {
        SerializedProperty m_DelegatesProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            m_DelegatesProperty = serializedObject.FindProperty("m_Delegates");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}