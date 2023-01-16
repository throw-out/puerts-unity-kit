using UnityEditor;
using UnityEngine;

namespace XOR
{
    internal class ModuleSelector : PopupWindowContent
    {
        /*
        internal static ModuleSelector GetWindow()
        {
            ModuleSelector window = (ModuleSelector)EditorWindow.GetWindow(typeof(ModuleSelector), true, nameof(ModuleSelector));
            window.Show();
            return window;
        }
        //*/
        internal static void GetWindow()
        {
            PopupWindow.Show(GUILayoutUtility.GetLastRect(), new ModuleSelector());
        }
        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("111111");
        }
    }
}
