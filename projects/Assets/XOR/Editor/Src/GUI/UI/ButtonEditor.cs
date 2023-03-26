using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace XOR.UI
{
    [CustomEditor(typeof(XOR.UI.ButtonWrapper))]
    internal class ButtonEditor : UnityEditor.UI.ButtonEditor
    {
        private SerializedProperty m_OnClick;
        private SerializedProperty m_Events;

        private WrapperRenderer renderer;
        protected override void OnEnable()
        {
            base.OnEnable();

            m_OnClick = serializedObject.FindProperty("m_OnClickWrapper");
            m_Events = m_OnClick.FindPropertyRelative("m_Events");

            renderer = new WrapperRenderer(m_Events);
            renderer.Header = "On Click () - [★]Wrapper";
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            renderer.Renderer();
        }
    }
}