using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    [CustomEditor(typeof(Settings))]
    //[CanEditMultipleObjects]
    internal class Settings_GUI : Editor
    {
        const float HeaderWidth = 100f;

        public override void OnInspectorGUI()
        {
            bool disable = EditorApplicationUtil.IsRunning();
            using (new EditorGUI.DisabledScope(disable))
            {
                //base.OnInspectorGUI();
                RenderSettings(Settings.Load(true, true));
                GUILayout.Space(20f);
                if (GUILayout.Button("重置"))
                {
                    RenderReset();
                }
            }

            GUILayout.Space(20f);
            using (new EditorGUI.DisabledScope(disable))
            {
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
        void RenderSettings(Settings settings)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Project", GUILayout.Width(HeaderWidth));
            string path = PathUtil.GetFullPath(settings.Project);
            if (GUILayout.Button("选择"))
            {
                GUIUtil.RenderSelectProject(null);
            }
            if (GUILayout.Button("查看") && File.Exists(path))
            {
                EditorUtility.RevealInFinder(path);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.TextField(settings.Project);
            }
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("EditorProject", GUILayout.Width(HeaderWidth));
            path = PathUtil.GetFullPath(settings.EditorProject);
            if (GUILayout.Button("选择"))
            {
                GUIUtil.RenderSelectEditorProject(null);
            }
            if (GUILayout.Button("查看") && File.Exists(path))
            {
                EditorUtility.RevealInFinder(path);
            }
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.TextField(settings.EditorProject);
            }
            GUILayout.EndHorizontal();


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

            Save(_current);
        }
        void Save(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
