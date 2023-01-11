using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    internal static class GUIUtil
    {
        public static void RendererApplicationStatus()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (EditorApplicationUtil.IsRunning())
            {
                if (GUILayout.Button("停止服务"))
                {
                    EditorApplicationUtil.Stop();
                }
                if (GUILayout.Button("重启服务"))
                {
                    EditorApplicationUtil.Stop(false);
                    EditorApplicationUtil.Start();
                }
            }
            else
            {
                if (GUILayout.Button("启动服务"))
                {
                    EditorApplicationUtil.Start();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        public static void RendererApplicationNotRunning()
        {

        }
        public static void RendererApplicationInitializing()
        {

        }

        public static string Status(bool active, bool richText = true)
        {
            //return active ? "<color=green>■</color>" : "<color=red>□</color>";
            return active ? "●" : "○";
        }
    }
}
