using System;
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
            click |= GUILayout.Button(Language.Component.Get("services_status"),
                Skin.labelBold,
                GUILayout.ExpandWidth(false)
            );
            click |= GUILayout.Button("●"
                , EditorApplicationUtil.IsRunning() ? Skin.labelGreen : Skin.labelGray
                , GUILayout.ExpandWidth(false)
            );
            click |= GUILayout.Button(Language.Component.Get("main_thread"), Skin.label);
            click |= GUILayout.Button("●"
                , !EditorApplicationUtil.IsWorkerRunning() ? Skin.labelGray : EditorApplicationUtil.IsInitializing() ? Skin.labelYellow : Skin.labelGreen
                , GUILayout.ExpandWidth(false));
            click |= GUILayout.Button(Language.Component.Get("worker_thread"), Skin.label);
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
                if (GUILayout.Button(Language.Component.Get("restart_services")))
                {
                    EditorApplicationUtil.Stop(false);
                    EditorApplicationUtil.Start();
                }
                if (GUILayout.Button(Language.Component.Get("stop_services")))
                {
                    EditorApplicationUtil.Stop();
                }
            }
            else
            {
                if (GUILayout.Button(Language.Component.Get("start_services")))
                {
                    EditorApplicationUtil.Start();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Language.Component.Get("status"), GUILayout.ExpandWidth(false));
            GUILayout.Label(EditorApplicationUtil.GetStatus());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Language.Component.Get("compile_status"), GUILayout.ExpandWidth(false));
            GUILayout.Label(EditorApplicationUtil.GetCompileStatus());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Language.Component.Get("compile_errors"), GUILayout.ExpandWidth(false));
            GUILayout.Label(EditorApplicationUtil.GetErrors());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Language.Component.Get("script_number"), GUILayout.ExpandWidth(false));
            GUILayout.Label(EditorApplicationUtil.GetScripts());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Language.Component.Get("class_number"), GUILayout.ExpandWidth(false));
            GUILayout.Label(EditorApplicationUtil.GetTypeCount());
            GUILayout.EndHorizontal();

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
        public static void RenderGroup<T>(Action<T> action, T args)
        {
            GUILayout.BeginVertical(Skin.groupBox);
            action(args);
            GUILayout.EndVertical();
        }
        public static void RenderGroup<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            GUILayout.BeginVertical(Skin.groupBox);
            action(arg1, arg2);
            GUILayout.EndVertical();
        }
        public static void RenderGroup<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            GUILayout.BeginVertical(Skin.groupBox);
            action(arg1, arg2, arg3);
            GUILayout.EndVertical();
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
        /// 选择项目路径, 并将结果存储到Settings资源
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public static string RenderSelectProject(string currentPath)
        {
            if (!string.IsNullOrEmpty(currentPath))
            {
                bool ok = EditorUtility.DisplayDialog(
                    Language.Default.Get("file_missing"),
                    string.Format(Language.Default.Get("file_missing_details"), currentPath),
                    Language.Default.Get("config"),
                    Language.Default.Get("cancel")
                );
                if (!ok)
                {
                    return string.Empty;
                }
            }
            string newPath = EditorUtility.OpenFilePanelWithFilters(
                Language.Default.Get("project_config_title"),
                Language.Default.Get("select_tscofnig"),
                new string[] { "NPM", "json" }
            );
            if (!string.IsNullOrEmpty(newPath) && File.Exists(newPath))
            {
                Settings asset = Settings.Load(true, true);
                asset.project = PathUtil.GetLocalPath(newPath);

                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return asset.project;
            }
            return string.Empty;
        }
        /// <summary>
        /// 选择项目路径, 并将结果存储到Settings资源
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public static string RenderSelectEditorProject(string currentPath)
        {
            if (!string.IsNullOrEmpty(currentPath))
            {
                bool ok = EditorUtility.DisplayDialog(
                    Language.Default.Get("file_missing"),
                    string.Format(Language.Default.Get("file_missing_details"), currentPath),
                    Language.Default.Get("config"),
                    Language.Default.Get("cancel")
                );
                if (!ok)
                {
                    return string.Empty;
                }
            }
            string newPath = EditorUtility.OpenFilePanelWithFilters(
                Language.Default.Get("project_config_title"),
                Language.Default.Get("select_tscofnig"),
                new string[] { "NPM", "json" }
            );
            if (!string.IsNullOrEmpty(newPath) && File.Exists(newPath))
            {
                Settings asset = Settings.Load(true, true);
                asset.editorProject = PathUtil.GetLocalPath(newPath);

                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return asset.editorProject;
            }
            return string.Empty;
        }

        /// <summary>
        /// 一个无效的tsconfig.json文件, 没有配套的package.json文件
        /// </summary>
        public static void RenderInvailTsconfig(string path)
        {
            UnityEditor.EditorUtility.DisplayDialog(
                Language.Default.Get("warning"),
                string.Format(Language.Default.Get("invail_tsconfig"), path),
                Language.Default.Get("ok")
            );
        }
        /// <summary>
        /// 模块依赖未安装完整
        /// </summary>
        /// <param name="path"></param>
        /// <param name="modules"></param>
        public static bool RenderDependentsUninstall(string path, string[] modules)
        {
            return UnityEditor.EditorUtility.DisplayDialog(
                Language.Default.Get("tip"),
                string.Format(Language.Default.Get("dependents_uninstall"), string.Join("\n", modules)),
                Language.Default.Get("confirm"),
                Language.Default.Get("cancel")
            );
        }

        /// <summary>
        /// 弹出必须启动TsComponent服务窗口
        /// </summary>
        public static void RenderMustLaunchServices()
        {
            bool startup = EditorUtility.DisplayDialog(
                Language.Default.Get("tip"),
                Language.Default.Get("must_start_services"),
                Language.Default.Get("launch"),
                Language.Default.Get("cancel")
            );
            if (startup && !EditorApplicationUtil.IsRunning())
                EditorApplicationUtil.Start();
        }

        /// <summary>
        /// 弹窗询问生成类型
        /// </summary>
        public static void RenderGenerateClass(Action confirm, int total = -1, int count = -1)
        {
            string message = Language.Default.Get("generate_class_confirm");
            if (total > 0)
            {
                message = string.Format(message, total, count);
            }
            bool ok = EditorUtility.DisplayDialog(
                Language.Default.Get("tip"),
                message,
                Language.Default.Get("confirm"),
                Language.Default.Get("cancel")
            );
            if (ok && confirm != null)
                confirm();
        }
        /// <summary>
        /// 弹窗提示生成类型为空
        /// </summary>
        public static void RenderGenerateClassEmpty()
        {
            string message = Language.Default.Get("generate_class_empty");
            EditorUtility.DisplayDialog(
                Language.Default.Get("tip"),
                message,
                Language.Default.Get("confirm")
            );
        }

        /// <summary>
        /// 弹窗确认窗口
        /// </summary>
        public static void RenderConfirm(string messageKey, Action confirm)
        {
            bool ok = EditorUtility.DisplayDialog(
                Language.Default.Get("tip"),
                Language.Default.Get(messageKey),
                Language.Default.Get("confirm"),
                Language.Default.Get("cancel")
            );
            if (ok && confirm != null)
                confirm();
        }
        /// <summary>
        /// 弹窗确认窗口
        /// </summary>
        public static void RenderConfirm<T1>(string messageKey, Action<T1> confirm, T1 arg1)
        {
            bool ok = EditorUtility.DisplayDialog(
                Language.Default.Get("tip"),
                Language.Default.Get(messageKey),
                Language.Default.Get("confirm"),
                Language.Default.Get("cancel")
            );
            if (ok && confirm != null)
                confirm(arg1);
        }
        /// <summary>
        /// 弹窗确认窗口
        /// </summary>
        public static void RenderConfirm<T1, T2>(string messageKey, Action<T1, T2> confirm, T1 arg1, T2 arg2)
        {
            bool ok = EditorUtility.DisplayDialog(
                Language.Default.Get("tip"),
                Language.Default.Get(messageKey),
                Language.Default.Get("confirm"),
                Language.Default.Get("cancel")
            );
            if (ok && confirm != null)
                confirm(arg1, arg2);
        }
    }
}
