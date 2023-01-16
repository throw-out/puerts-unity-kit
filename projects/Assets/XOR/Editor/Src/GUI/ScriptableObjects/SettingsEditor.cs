using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    [CustomEditor(typeof(Settings))]
    //[CanEditMultipleObjects]
    internal class SettingsEditor : Editor
    {
        const float HeaderWidth = 100f;
        const float HeightSpace = 10f;

        public override void OnInspectorGUI()
        {
            bool disable = EditorApplicationUtil.IsRunning();
            using (new EditorGUI.DisabledScope(disable))
            {
                GUIUtil.RenderGroup(RenderProject, Settings.Load(true, true));
                GUILayout.Space(HeightSpace);
                GUIUtil.RenderGroup(RenderEditorProject, Settings.Load(true, true));
                GUILayout.Space(HeightSpace);
                GUIUtil.RenderGroup(RenderOther, Settings.Load(true, true));
                GUILayout.Space(HeightSpace);
            }
            using (new EditorGUI.DisabledScope(disable))
            {
                if (GUILayout.Button("重置"))
                {
                    RenderReset();
                }
                GUILayout.Space(HeightSpace);
                if (GUILayout.Button("启动服务"))
                {
                    EditorApplicationUtil.Start();
                }
            }
            using (new EditorGUI.DisabledScope(!disable))
            {
                if (GUILayout.Button("停止服务"))
                {
                    EditorApplicationUtil.Stop();
                }
            }
        }
        void RenderProject(Settings settings)
        {
            GUILayout.Label("项目");

            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.TextField(settings.Project);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("编辑"))
            {
                GUIUtil.RenderSelectProject(null);
            }
            if (GUILayout.Button("查看"))
            {
                string path = PathUtil.GetFullPath(settings.Project);
                if (File.Exists(path))
                {
                    EditorUtility.RevealInFinder(path);
                }
                else
                {
                    Debug.LogError("文件不存在: " + path);
                }
            }
            GUILayout.EndHorizontal();
        }
        void RenderEditorProject(Settings settings)
        {
            GUILayout.Label("XOR编辑器项目");

            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.TextField(settings.EditorProject);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("编辑"))
            {
                GUIUtil.RenderSelectEditorProject(null);
            }
            if (GUILayout.Button("查看"))
            {
                string path = PathUtil.GetFullPath(settings.EditorProject);
                if (File.Exists(path))
                {
                    EditorUtility.RevealInFinder(path);
                }
                else
                {
                    Debug.LogError("文件不存在: " + path);
                }
            }
            GUILayout.EndHorizontal();
        }

        void RenderOther(Settings settings)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("IsESM", GUILayout.Width(HeaderWidth));
            bool on = GUILayout.Toggle(settings.IsESM, string.Empty);
            GUILayout.EndHorizontal();
            if (on != settings.IsESM)
            {
                settings.IsESM = on;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Logger", GUILayout.Width(HeaderWidth));
            Settings.LOGGER logger = (Settings.LOGGER)EditorGUILayout.EnumFlagsField(settings.Logger);
            GUILayout.EndHorizontal();
            if (logger != settings.Logger)
            {
                settings.Logger = logger;
            }
        }

        void RenderReset()
        {
            bool ok = EditorUtility.DisplayDialog("提示", "您确定要重置XOR.Settings配置文件吗?", "重置", "取消");
            if (!ok)
            {
                return;
            }
            var _default = ScriptableObject.CreateInstance<Settings>();
            var _current = Settings.Load(true, true);

            _current.Project = _default.Project;
            _current.EditorProject = _default.EditorProject;
            _current.IsESM = _default.IsESM;
            _current.Logger = _default.Logger;

            EditorUtility.SetDirty(_current);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
