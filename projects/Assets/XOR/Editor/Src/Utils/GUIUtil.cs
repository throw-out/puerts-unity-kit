using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    internal static class GUIUtil
    {
        public static bool RenderStatusHeader()
        {
            bool click = false;

            GUILayout.BeginVertical(Skin.headerBox);
            GUILayout.BeginHorizontal();
            click |= GUILayout.Button("服务状态:", Skin.labelBold, GUILayout.ExpandWidth(false));
            click |= GUILayout.Button("●"
                , EditorApplicationUtil.IsRunning() ? Skin.labelBoldGreen : Skin.labelBoldGray
                , GUILayout.ExpandWidth(false)
            );
            click |= GUILayout.Button("主线程", Skin.label);
            click |= GUILayout.Button("●"
                , !EditorApplicationUtil.IsWorkerRunning() ? Skin.labelBoldGray : EditorApplicationUtil.IsInitializing() ? Skin.labelBoldYellow : Skin.labelBoldGreen
                , GUILayout.ExpandWidth(false)
            );
            click |= GUILayout.Button("工作线程", Skin.label);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.EndVertical();

            return click;
        }
        public static void RenderStatusContent()
        {
            GUILayout.BeginVertical(Skin.groupBox);
            GUILayout.Space(5f);
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
            GUILayout.Space(5f);
            GUILayout.EndVertical();
        }

        public static bool RenderHeader(string title)
        {
            bool click = false;

            GUILayout.BeginVertical(Skin.headerBox);
            GUILayout.BeginHorizontal();
            click |= GUILayout.Button(title ?? string.Empty, Skin.labelBold);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.EndVertical();

            return click;
        }
        public static void RenderGroup(Action firstAction, params Action[] actions)
        {
            GUILayout.BeginVertical(Skin.groupBox);
            GUILayout.Space(5f);
            firstAction();
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i]();
            }
            GUILayout.Space(5f);
            GUILayout.EndVertical();
        }
    }
}
