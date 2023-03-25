using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace XOR.UI
{
    [CustomEditor(typeof(XOR.UI.ToggleWrapper))]
    internal class ToggleEditor : UnityEditor.UI.ToggleEditor
    {
        private SerializedProperty m_OnValueChanged;
        private SerializedProperty m_Events;

        private WrapperRenderer renderer;
        protected override void OnEnable()
        {
            base.OnEnable();

            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChangedWrapper");
            m_Events = m_OnValueChanged.FindPropertyRelative("m_Events");

            renderer = new WrapperRenderer(m_Events);
            renderer.Header = "On Value Changed (Boolean) - [★]Wrapper";
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            renderer.Renderer();
        }
    }
}