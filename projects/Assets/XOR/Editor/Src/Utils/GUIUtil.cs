using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    internal static class GUIUtil
    {
        public static bool RenderStatusHeader()
        {
            bool click = false;

            GUILayout.BeginVertical(Skin.headerBox, GUILayout.ExpandHeight(true));
            GUILayout.BeginHorizontal();
            click |= GUILayout.Button("服务状态:", Skin.labelBold, GUILayout.ExpandWidth(false));
            click |= GUILayout.Button("●"
                , EditorApplicationUtil.IsRunning() ? Skin.labelGreen : Skin.labelGray
                , GUILayout.ExpandWidth(false)
            );
            click |= GUILayout.Button("主线程", Skin.label);
            click |= GUILayout.Button("●"
                , !EditorApplicationUtil.IsWorkerRunning() ? Skin.labelGray : EditorApplicationUtil.IsInitializing() ? Skin.labelYellow : Skin.labelGreen
                , GUILayout.ExpandWidth(false)
            );
            click |= GUILayout.Button("工作线程", Skin.label);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            return click;
        }
        public static void RenderStatusContent()
        {
            GUILayout.BeginVertical(Skin.groupBox);
            GUILayout.BeginHorizontal();
            if (EditorApplicationUtil.IsRunning())
            {
                if (GUILayout.Button("重启服务"))
                {
                    EditorApplicationUtil.Stop(false);
                    EditorApplicationUtil.Start();
                }
                if (GUILayout.Button("停止服务"))
                {
                    EditorApplicationUtil.Stop();
                }
            }
            else
            {
                if (GUILayout.Button("启动服务"))
                {
                    EditorApplicationUtil.Start();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("状态: UNKNOWN");
            GUILayout.Label("脚本数量: UNKNOWN");
            GUILayout.Label("编译错误: UNKNOWN");
            GUILayout.Label("脚本数量: UNKNOWN");
            GUILayout.Label("已解析类: UNKNOWN");
            GUILayout.EndVertical();
        }

        public static bool RenderHeader(string title)
        {
            bool click = false;

            GUILayout.BeginVertical(Skin.headerBox, GUILayout.ExpandHeight(true));
            GUILayout.BeginHorizontal();
            click |= GUILayout.Button(title ?? string.Empty, Skin.labelBold);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            return click;
        }
        public static void RenderGroup(Action firstAction, params Action[] actions)
        {
            GUILayout.BeginVertical(Skin.groupBox);
            firstAction();
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i]();
            }
            GUILayout.EndVertical();
        }


        /// <summary>
        /// 选择编辑器项目路径, 并将结果存储到Settings资源
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public static string RenderSelectEditorProject(string currentPath)
        {
            string newPath = EditorUtility.OpenFilePanelWithFilters("编辑器项目", "请选择XOR typescript编辑器项目路径", new string[] { "NPM", ".json" });
            if (!string.IsNullOrEmpty(newPath) && File.Exists(newPath))
            {
                Settings asset = Settings.Load(true, true);
                asset.EditorProject = PathUtil.GetLocalPath(newPath);

                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return asset.EditorProject;
            }
            return string.Empty;
        }
        /// <summary>
        /// 选择项目路径, 并将结果存储到Settings资源
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public static string RenderSelectProject(string currentPath)
        {
            string newPath = EditorUtility.OpenFilePanelWithFilters("编辑器项目", "请选择typescript项目路径", new string[] { "NPM", ".json" });
            if (!string.IsNullOrEmpty(newPath) && File.Exists(newPath))
            {
                Settings asset = Settings.Load(true, true);
                asset.Project = PathUtil.GetLocalPath(newPath);

                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return asset.Project;
            }
            return string.Empty;
        }
    }
}
