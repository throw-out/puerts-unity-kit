using UnityEngine;
using UnityEditor;

namespace HR
{
    public class ManagerGUI : EditorWindow
    {

        [MenuItem("Tools/CDP/HR Manager")]
        private static void ShowWindow()
        {
            var window = GetWindow<ManagerGUI>();
            window.titleContent = new GUIContent("ManagerGUI");
            window.Show();
        }
        private Debugger debugger;

        private void OnGUI()
        {
            if (GUILayout.Button("Start"))
            {
                if (debugger == null)
                {
                    debugger = new Debugger();
                    debugger.ignoreCase = true;
                    debugger.trace = true;
                }

                debugger.Open("127.0.0.1", 9090);
            }
            if (GUILayout.Button("Stop") && debugger != null)
            {
                debugger.Close();
            }
        }
    }

}
