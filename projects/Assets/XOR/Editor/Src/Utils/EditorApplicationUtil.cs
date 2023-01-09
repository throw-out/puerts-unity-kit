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
                EditorApplication process = EditorApplication.GetInstance();
                process.Env.Eval("require('./main')");

                UnityEngine.Debug.Log($"<b>XOR.{nameof(EditorApplication)}: <color=green>Started</color>.</b>");
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

            if (print) UnityEngine.Debug.Log($"<b>XOR.{nameof(EditorApplication)}: <color=red>Stoped</color>.</b>");
        }
    }
}