using System.IO;

namespace XOR
{
    public static class Util
    {
        internal static bool IsRunning() => TsServiceProcess.Instance != null;
        internal static bool ISL() => !string.IsNullOrEmpty(Prefs.ProjectPath) && File.Exists(Prefs.ProjectPath);

        internal static void Enable()
        {
            Prefs.Enable.SetValue(true);
            try
            {
                TsServiceProcess process = TsServiceProcess.GetInstance();
                process.Env.Eval("require('./main')");

                UnityEngine.Debug.Log($"<b>XOR.{nameof(TsServiceProcess)}: <color=green>Started</color>.</b>");
            }
            catch (System.Exception e)
            {
                Prefs.Enable.SetValue(false);
                TsServiceProcess.ReleaseInstance();
                throw e;
            }
        }
        internal static void Disable()
        {
            Prefs.Enable.SetValue(false);
            TsServiceProcess.ReleaseInstance();

            UnityEngine.Debug.Log($"<b>XOR.{nameof(TsServiceProcess)}: <color=red>Stoped</color>.</b>");
        }
    }
}