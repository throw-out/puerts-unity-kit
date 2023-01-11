using UnityEditor;

namespace XOR
{
    [CustomEditor(typeof(TsComponent))]
    internal class TsComponent_GUI : Editor
    {
        public override void OnInspectorGUI()
        {
            if (!EditorApplicationUtil.IsRunning())
            {

            }
            base.OnInspectorGUI();
        }
    }
}
