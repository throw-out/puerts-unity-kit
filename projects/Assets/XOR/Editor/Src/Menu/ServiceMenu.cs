using System;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    [InitializeOnLoad]
    public class ServiceMenu
    {
        static ServiceMenu()
        {
            if (Prefs.Enable && !EditorApplicationUtil.IsRunning())
            {
                EditorApplicationUtil.Start();
            }
        }

        [MenuItem("Tools/XOR/Reload")]
        static void Reload()
        {
            EditorApplicationUtil.Stop(false);
            EditorApplicationUtil.Start();
        }

        [MenuItem("Tools/XOR/Enable")]
        static void Enable() => EditorApplicationUtil.Start();
        [MenuItem("Tools/XOR/Enable", true)]
        static bool EnableValidate() => !EditorApplicationUtil.IsRunning();
        [MenuItem("Tools/XOR/Disable")]
        static void Disable() => EditorApplicationUtil.Stop();
        [MenuItem("Tools/XOR/Disable", true)]
        static bool DisableValidate() => EditorApplicationUtil.IsRunning();


        [MenuItem("Tools/XOR/Settings", false, 0)]
        static void OpenSettings()
        {
            Selection.activeObject = XOR.Settings.Load(true, true);
        }
    }
}
