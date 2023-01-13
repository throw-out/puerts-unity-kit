using System;
using System.IO;
using XOR.Services;

namespace XOR
{
    internal static class EditorApplicationUtil
    {
        public static bool IsRunning() => EditorApplication.Instance != null;
        public static bool IsInitializing() => IsRunning() && EditorApplication.Instance.IsInitializing();
        public static bool IsWorkerRunning() => IsRunning() && EditorApplication.Instance.IsWorkerRunning();

        public static bool IsAvailable()
        {
            return IsRunning() && EditorApplication.Instance.IsWorkerRunning() && !EditorApplication.Instance.IsInitializing();
        }

        public static void Start()
        {
            Prefs.Enable.SetValue(true);
            try
            {
                string editorProjectConfig = Settings.Load().EditorProject,
                    projectConfig = Settings.Load().Project;

                editorProjectConfig = PathUtil.GetFullPath(editorProjectConfig);
                projectConfig = PathUtil.GetFullPath(projectConfig);
                if (!File.Exists(editorProjectConfig))
                {
                    string newPath = GUIUtil.RenderSelectEditorProject(editorProjectConfig);
                    if (string.IsNullOrEmpty(newPath))
                    {
                        return;
                    }
                    editorProjectConfig = PathUtil.GetFullPath(newPath);
                }
                if (!File.Exists(projectConfig))
                {
                    string newPath = GUIUtil.RenderSelectProject(projectConfig);
                    if (string.IsNullOrEmpty(newPath))
                    {
                        return;
                    }
                    projectConfig = PathUtil.GetFullPath(newPath);
                }

                string editorRoot = Path.GetDirectoryName(editorProjectConfig);
                string editorOutput = Path.Combine(editorRoot, "output");

                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=green>Executing</color></b> \neditorRoot: {editorRoot}\neditorOutput: {editorOutput}");

                EditorApplication app = EditorApplication.GetInstance();
                app.Loader.AddLoader(new FileLoader(editorOutput, editorRoot));

                //create interfaces
                CSharpInterfaces ci = new CSharpInterfaces();
                ci.SetWorker = app.SetWorker;
                ci.SetProgram = app.SetProgram;
                //init application
                Func<CSharpInterfaces, TSInterfaces> Init = app.Env.Eval<Func<CSharpInterfaces, TSInterfaces>>(@"
var m = require('./main/main');
m.init;
");
                TSInterfaces ti = Init(ci);
                app.SetInterfaces(ti);

                ti.Start(editorOutput, projectConfig);

                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=green>Started</color>.</b>");
            }
            catch (System.Exception e)
            {
                Prefs.Enable.SetValue(false);
                EditorApplication.ReleaseInstance();
                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=red>Exception</color>:</b>\n{e}");
            }
        }
        public static void Stop(bool print = true)
        {
            Prefs.Enable.SetValue(false);
            EditorApplication.ReleaseInstance();
            if (!UnityEngine.Application.isPlaying)
            {
                ThreadWorker.ReleaseAllInstance();
            }

            if (print) Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=red>Stoped</color>.</b>");
        }
    }
}