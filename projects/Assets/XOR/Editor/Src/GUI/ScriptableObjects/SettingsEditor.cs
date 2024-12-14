using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XOR.Services;

namespace XOR
{
    [CustomEditor(typeof(Settings))]
    //[CanEditMultipleObjects]
    internal class SettingsEditor : Editor
    {
        const float HeaderWidth = 100f;
        const float HeightSpace = 10f;

        void OnDisable()
        {
            if (!ScriptingDefineSymbols.Change)
                return;
            bool ok = EditorUtility.DisplayDialog(
                Language.Default.Get("tip"),
                Language.Default.Get("config_changed"),
                Language.Default.Get("save"),
                Language.Default.Get("cancel")
            );
            if (ok)
            {
                ScriptingDefineSymbols.Save();
            }
            else
            {
                ScriptingDefineSymbols.Read();
            }
        }
        public override void OnInspectorGUI()
        {
            bool disable = EditorApplicationUtil.IsRunning();
            using (new EditorGUI.DisabledScope(disable))
            {
                GUIUtil.RenderGroup(RenderProject, Settings.Load(true, true));
                GUILayout.Space(HeightSpace);
                GUIUtil.RenderGroup(RenderOther, Settings.Load(true, true));
                GUILayout.Space(HeightSpace);
                GUIUtil.RenderGroup(RenderWatchType, Settings.Load(true, true));
                GUILayout.Space(HeightSpace);
                GUIUtil.RenderGroup(RenderMetadataCached, Settings.Load(true, true));
                GUILayout.Space(HeightSpace);
                GUIUtil.RenderGroup(RenderScriptingDefine);
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
            GUILayout.BeginVertical();

            //project
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

            //editor project
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

            GUILayout.EndVertical();
        }
        void RenderOther(Settings settings)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("IsESM", GUILayout.Width(HeaderWidth));
            bool isESM = GUILayout.Toggle(settings.isESM, string.Empty);
            GUILayout.EndHorizontal();
            if (isESM != settings.isESM)
            {
                settings.isESM = isESM;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Logger", GUILayout.Width(HeaderWidth));
            Settings.LOGGER logger = (Settings.LOGGER)EditorGUILayout.EnumFlagsField(settings.logger);
            GUILayout.EndHorizontal();
            if (logger != settings.logger)
            {
                settings.logger = logger;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Verbose", GUILayout.Width(HeaderWidth));
            bool verbose = GUILayout.Toggle(settings.verbose, string.Empty);
            GUILayout.EndHorizontal();
            if (verbose != settings.verbose)
            {
                settings.verbose = verbose;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Auto Script", GUILayout.Width(HeaderWidth));
            bool autoLoadScript = GUILayout.Toggle(settings.autoLoadScript, string.Empty);
            GUILayout.EndHorizontal();
            if (autoLoadScript != settings.autoLoadScript)
            {
                settings.autoLoadScript = autoLoadScript;
            }

            _RenderTooptip(Skin.infoIcon, Language.Default.Get("script_path_tip"));
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
            _RenderTooptip(Skin.infoIcon, Language.Default.Get("file_watcher_tip"));

            if (!PuertsUtil.IsSupportNodejs())
            {
                _RenderTooptip(Skin.warnIcon, Language.Default.Get("nodejs_unsupport"));
            }
        }
        void RenderMetadataCached(Settings settings)
        {
            GUILayout.Label("Metadata Cached");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled", GUILayout.Width(HeaderWidth));
            bool enabled = GUILayout.Toggle(settings.cached, string.Empty);
            GUILayout.EndHorizontal();
            if (enabled != settings.cached)
            {
                settings.cached = enabled;
                if (!enabled)
                {
                    ProgramCached.DeleteRoot();
                    EditorApplicationUtil.DeleteCached();
                }
            }
            GUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(!settings.cached))
            {
                if (GUILayout.Button(Language.Default.Get("delete_metadata_cached")))
                {
                    ProgramCached.DeleteRoot();
                    EditorApplicationUtil.DeleteCached();
                }
                if (GUILayout.Button(Language.Default.Get("reload_metadata_cached")))
                {
                    EditorApplicationUtil.DeleteCached();
                    var cached = EditorApplicationUtil.GetProgramCached();
                    var appProgram = EditorApplication.Instance?.Program;
                    if (appProgram != null)
                    {
                        appProgram.SetCahced(cached);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        void RenderScriptingDefine()
        {
            GUILayout.Label("Scripting Define Symbols");
            using (new EditorGUI.DisabledScope(UnityEngine.Application.isPlaying))
            {
                GUILayout.Space(HeightSpace);
                _RenderScriptingDefine("THREAD_SAFE");

                _RenderTooptip(Skin.infoIcon, Language.Default.Get("thread_safe_tip"));

                using (new EditorGUI.DisabledScope(!ScriptingDefineSymbols.Change))
                {
                    GUILayout.Space(HeightSpace);

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(Language.Default.Get("reset")))
                    {
                        ScriptingDefineSymbols.Read();
                    }
                    if (GUILayout.Button(Language.Default.Get("save")))
                    {
                        ScriptingDefineSymbols.Save();
                        AssetDatabase.Refresh();
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        void _RenderScriptingDefine(string symbol)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(symbol, symbol), GUILayout.Width(HeaderWidth * 2));
            bool exist = ScriptingDefineSymbols.HasSymbol(symbol);
            if (GUILayout.Toggle(exist, string.Empty) != exist)
            {
                if (exist) ScriptingDefineSymbols.RemoveSymbol(symbol);
                else ScriptingDefineSymbols.AddSymbol(symbol);
            }
            GUILayout.EndHorizontal();
        }
        void _RenderTooptip(GUIStyle icon, string tooltip)
        {
            GUILayout.Space(HeightSpace);
            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.Label(string.Empty, icon);
            GUILayout.Label(tooltip, Skin.labelArea, GUILayout.ExpandHeight(true));
            GUILayout.EndHorizontal();
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
            _current.autoLoadScript = _default.autoLoadScript;
            _current.cached = _default.cached;
            _current.logger = _default.logger;
            _current.watchType = _default.watchType;
            _current.verbose = _default.verbose;

            EditorUtility.SetDirty(_current);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
