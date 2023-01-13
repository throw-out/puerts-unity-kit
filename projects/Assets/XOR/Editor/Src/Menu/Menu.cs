using System;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    [InitializeOnLoad]
    internal class Menu
    {
        static Menu()
        {
            EditorApplicationHandler.DelayCall += InitializeStart;
        }

        static void InitializeStart()
        {
            EditorApplicationHandler.DelayCall -= InitializeStart;
            if (Prefs.Enable && !EditorApplicationUtil.IsRunning())
            {
                EditorApplicationUtil.Start();
            }
        }


        [MenuItem("Tools/XOR/Settings", false, 0)]
        static void OpenSettings()
        {
            Selection.activeObject = XOR.Settings.Load(true, true);
        }


        [MenuItem("Tools/XOR/Servies/Restart")]
        static void Reload()
        {
            EditorApplicationUtil.Stop(false);
            EditorApplicationUtil.Start();
        }
        [MenuItem("Tools/XOR/Servies/Start")]
        static void Enable() => EditorApplicationUtil.Start();
        [MenuItem("Tools/XOR/Servies/Start", true)]
        static bool EnableValidate() => !EditorApplicationUtil.IsRunning();
        [MenuItem("Tools/XOR/Servies/Stop")]
        static void Disable() => EditorApplicationUtil.Stop();
        [MenuItem("Tools/XOR/Servies/Stop", true)]
        static bool DisableValidate() => EditorApplicationUtil.IsRunning();


        [MenuItem("Tools/XOR/Component/SyncAll")]
        static void SyncAllComponents()
        {

        }

    }
}
