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
                GUIUtil.RenderGroup(RenderWatchType, Settings.Load(true, true));
                GUILayout.Space(HeightSpace);
                GUIUtil.RenderGroup(RenderOther, Settings.Load(true, true));
                GUILayout.Space(HeightSpace);
            }
            using (new EditorGUI.DisabledScope(disable))
            {
                if (GUILayout.Button(Language.Default.Get("reset")))
                {
                    RenderReset();
                }
                GUILayout.Space(HeightSpace);
                if (GUILayout.Button(Language.Component.Get("start_services")))
                {
                    EditorApplicationUtil.Start();
                }
            }
            using (new EditorGUI.DisabledScope(!disable))
            {
                if (GUILayout.Button(Language.Component.Get("stop_services")))
                {
                    EditorApplicationUtil.Stop();
                }
            }
        }
        void RenderProject(Settings settings)
        {
            GUILayout.Label(Language.Default.Get("project_config_title"));

            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.TextField(settings.project);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Language.Default.Get("edit")))
            {
                GUIUtil.RenderSelectProject(null);
            }
            if (GUILayout.Button(Language.Default.Get("look")))
            {
                string path = PathUtil.GetFullPath(settings.project);
                if (File.Exists(path))
                {
                    EditorUtility.RevealInFinder(path);
                }
                else
                {
                    Debug.LogErrorFormat(Language.Default.Get("file_missing_details"), path);
                }
            }
            GUILayout.EndHorizontal();
        }
        void RenderEditorProject(Settings settings)
        {
            GUILayout.Label(Language.Default.Get("editor_project_config_title"));

            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.TextField(settings.editorProject);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Language.Default.Get("edit")))
            {
                GUIUtil.RenderSelectEditorProject(null);
            }
            if (GUILayout.Button(Language.Default.Get("look")))
            {
                string path = PathUtil.GetFullPath(settings.editorProject);
                if (File.Exists(path))
                {
                    EditorUtility.RevealInFinder(path);
                }
                else
                {
                    Debug.LogErrorFormat(Language.Default.Get("file_missing_details"), path);
                }
            }
            GUILayout.EndHorizontal();
        }
        void RenderWatchType(Settings settings)
        {
            GUILayout.Label("File Wacther");

            using (new EditorGUI.DisabledScope(!PuertsUtil.IsSupportNodejs()))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Type", GUILayout.Width(HeaderWidth));
                Settings.WacthType wacthType = (Settings.WacthType)EditorGUILayout.EnumPopup(settings.watchType);
                GUILayout.EndHorizontal();
                if (wacthType != settings.watchType)
                {
                    settings.watchType = wacthType;
                }
            }
            GUILayout.Space(HeightSpace);
            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.Label(string.Empty, Skin.infoIcon);
            GUILayout.Label(Language.Default.Get("file_watcher_tip"), Skin.labelArea, GUILayout.ExpandHeight(true));
            GUILayout.EndHorizontal();

            if (!PuertsUtil.IsSupportNodejs())
            {
                GUILayout.Space(HeightSpace);
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.Label(string.Empty, Skin.warnIcon);
                GUILayout.Label(Language.Default.Get("nodejs_unsupport"), Skin.labelArea, GUILayout.ExpandHeight(true));
                GUILayout.EndHorizontal();
            }
        }
        void RenderOther(Settings settings)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("IsESM", GUILayout.Width(HeaderWidth));
            bool on = GUILayout.Toggle(settings.isESM, string.Empty);
            GUILayout.EndHorizontal();
            if (on != settings.isESM)
            {
                settings.isESM = on;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Logger", GUILayout.Width(HeaderWidth));
            Settings.LOGGER logger = (Settings.LOGGER)EditorGUILayout.EnumFlagsField(settings.logger);
            GUILayout.EndHorizontal();
            if (logger != settings.logger)
            {
                settings.logger = logger;
            }
        }

        void RenderReset()
        {
            bool ok = EditorUtility.DisplayDialog(
                Language.Default.Get("tip"),
                Language.Default.Get("want_reset_settings"),
                Language.Default.Get("reset"),
                Language.Default.Get("cancel")
            );
            if (!ok)
            {
                return;
            }
            var _default = ScriptableObject.CreateInstance<Settings>();
            var _current = Settings.Load(true, true);

            _current.project = _default.project;
            _current.editorProject = _default.editorProject;
            _current.isESM = _default.isESM;
            _current.logger = _default.logger;
            _current.watchType = _default.watchType;

            EditorUtility.SetDirty(_current);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
