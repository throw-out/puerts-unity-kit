using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace XOR.UI
{
    [CustomEditor(typeof(XOR.UI.InputFieldWrapper))]
    internal class InputFieldEditor : UnityEditor.UI.InputFieldEditor
    {
        private SerializedProperty m_OnValueChanged;
        private SerializedProperty m_OnValueChangedEvents;
        private SerializedProperty m_OnEndEdit;
        private SerializedProperty m_OnEndEditEvents;

        private WrapperRenderer onValueChangedRenderer;
        private WrapperRenderer onEndEditRenderer;
        protected override void OnEnable()
        {
            base.OnEnable();

            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChangedWrapper");
            m_OnEndEdit = serializedObject.FindProperty("m_OnEndEditWrapper");

            m_OnValueChangedEvents = m_OnValueChanged.FindPropertyRelative("m_Events");
            m_OnEndEditEvents = m_OnEndEdit.FindPropertyRelative("m_Events");

            onValueChangedRenderer = new WrapperRenderer(m_OnValueChangedEvents);
            onValueChangedRenderer.Header = "On Value Changed (String) - [★]Wrapper";
            onEndEditRenderer = new WrapperRenderer(m_OnEndEditEvents);
            onEndEditRenderer.Header = "On End Edit (String) - [★]Wrapper";
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            onValueChangedRenderer.Renderer();
            onEndEditRenderer.Renderer();
        }
    }
}