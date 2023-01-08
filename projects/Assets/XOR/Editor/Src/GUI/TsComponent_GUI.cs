using UnityEditor;

namespace XOR
{
    [CustomEditor(typeof(TsComponent))]
    public class TsComponent_GUI : Editor
    {
        public override void OnInspectorGUI()
        {
            if (!Util.IsRunning())
            {

            }
            base.OnInspectorGUI();
        }
    }
}
