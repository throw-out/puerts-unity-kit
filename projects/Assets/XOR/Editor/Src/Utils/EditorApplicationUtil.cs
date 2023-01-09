using System.IO;

namespace XOR
{
    public static class EditorApplicationUtil
    {
        internal static bool IsRunning() => EditorApplication.Instance != null;

        internal static void Start()
        {
            Prefs.Enable.SetValue(true);
            try
            {
                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=green>Executing</color></b>");

                EditorApplication process = EditorApplication.GetInstance();
                process.Env.Eval("require('./main/main')");

                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=green>Started</color>.</b>");
            }
            catch (System.Exception e)
            {
                Prefs.Enable.SetValue(false);
                EditorApplication.ReleaseInstance();
                throw e;
            }
        }
        internal static void Stop(bool print = true)
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