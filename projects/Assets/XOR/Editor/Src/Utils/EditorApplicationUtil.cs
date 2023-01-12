using System;
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
            return IsRunning() && IsWorkerRunning() && !IsInitializing();
        }

        public static void Start()
        {
            Prefs.Enable.SetValue(true);
            try
            {
                string editorProject = Settings.Load().EditorProject;
                string project = Settings.Load().Project;

                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=green>Executing</color></b>");

                EditorApplication process = EditorApplication.GetInstance();
                //create interfaces
                CSharpInterfaces ci = new CSharpInterfaces();
                ci.SetWorker = process.SetWorker;
                ci.SetProgram = process.SetProgram;

                //init application
                Func<CSharpInterfaces, TSInterfaces> Init = process.Env.Eval<Func<CSharpInterfaces, TSInterfaces>>(@"
var m = require('./main/main');
m.init;
");
                TSInterfaces ti = Init(ci);
                process.SetInterfaces(ti);

                ti.Start(editorProject, project);

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