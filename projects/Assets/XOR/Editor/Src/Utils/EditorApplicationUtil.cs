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
                string editorProject = Settings.Load().EditorProject;
                string project = Settings.Load().Project;

                string editorRoot = Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, Path.GetDirectoryName(editorProject)));

                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=green>Executing</color></b>");

                EditorApplication app = EditorApplication.GetInstance();

                //editor loader
                string projectRoot = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "TsEditorProject");
                string outputRoot = Path.Combine(projectRoot, "output");
                app.Loader.AddLoader(new FileLoader(outputRoot, projectRoot));

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

                ti.Start(
                    Path.Combine(editorRoot, "output"),
                    Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, project))
                );

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