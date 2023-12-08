using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    [InitializeOnLoad]
    internal class Menu
    {
        static Menu()
        {
            EditorApplicationHandler.delayCall += InitializeStart;
            EditorApplicationHandler.dispose += Dispose;
        }

        static void InitializeStart()
        {
            EditorApplicationHandler.delayCall -= InitializeStart;
            if (Prefs.ASTEnable && !EditorApplicationUtil.IsRunning())
            {
                EditorApplicationUtil.Start();
            }
        }
        static void Dispose()
        {
            EditorApplicationHandler.dispose -= Dispose;
            int count = ThreadWorker.RealThread - ThreadWorker.GetAllInstances().Count;
            while (count > 0)
            {
                bool wait = EditorUtility.DisplayDialog(
                    Language.Default.Get("warning"),
                    string.Format(Language.Default.Get("appdomain_unload_exception"), nameof(XOR.ThreadWorker), count, nameof(Puerts.JsEnv)),
                    Language.Default.Get("wait"),
                    "Unload"
                );
                if (!wait) break;
                count = ThreadWorker.RealThread - ThreadWorker.GetAllInstances().Count;
            }
        }


        [MenuItem("Tools/XOR/Settings", false, 0)]
        static void OpenSettings()
        {
            Selection.activeObject = XOR.Settings.Load(true, true);
        }


        #region Language菜单项
        [MenuItem("Tools/XOR/Language/简体中文", false, 0)]
        static void LanguageZHCN() => Prefs.Language.SetValue((int)Language.Env.ZH_CN);
        [MenuItem("Tools/XOR/Language/简体中文", true, 0)]
        static bool LanguageZHCNValidate() => Prefs.Language.GetValue() != (int)Language.Env.ZH_CN;
        [MenuItem("Tools/XOR/Language/English", false, 0)]
        static void LanguageENUS() => Prefs.Language.SetValue((int)Language.Env.EN_US);
        [MenuItem("Tools/XOR/Language/English", true, 0)]
        static bool LanguageENUSValidate() => Prefs.Language.GetValue() != (int)Language.Env.EN_US;
        #endregion



        #region 开发者模式
        [MenuItem("Tools/XOR/DeveloperMode/Enable", false, 0)]
        static void DeveloperEnable() => Prefs.DeveloperMode.SetValue(true);
        [MenuItem("Tools/XOR/DeveloperMode/Enable", true, 0)]
        static bool DeveloperEnableValidate() => !Prefs.DeveloperMode.GetValue();
        [MenuItem("Tools/XOR/DeveloperMode/Disable", false, 0)]
        static void DeveloperDisable() => Prefs.DeveloperMode.SetValue(false);
        [MenuItem("Tools/XOR/DeveloperMode/Disable", true, 0)]
        static bool DeveloperDisableValidate() => Prefs.DeveloperMode.GetValue();
        #endregion


        #region Servies菜单项
        [MenuItem("Tools/XOR/Servies/Restart")]
        static void Reload()
        {
            EditorApplicationUtil.Stop(false);
            EditorApplicationUtil.Start();
        }
        [MenuItem("Tools/XOR/Servies/Start")]
        static void Enable() => EditorApplicationUtil.Start();
        [MenuItem("Tools/XOR/Servies/Start", true)]
        static bool EnableValidate() => !EditorApplicationUtil.IsRunning();
        [MenuItem("Tools/XOR/Servies/Stop")]
        static void Disable() => EditorApplicationUtil.Stop();
        [MenuItem("Tools/XOR/Servies/Stop", true)]
        static bool DisableValidate() => EditorApplicationUtil.IsRunning();
        #endregion


        [MenuItem("Tools/XOR/Component/Status")]
        static void TsComponentsStatus() => TsComponent.PrintStatus();
        [MenuItem("Tools/XOR/Component/SyncAll")]
        static void SyncTsComponents() => TsComponentHelper.SyncAssetsComponents(false);
        [MenuItem("Tools/XOR/Component/SyncAll[Force]")]
        static void SyncTsComponentsForce() => TsComponentHelper.SyncAssetsComponents(true);


        [MenuItem("Tools/XOR/Therad/Current")]
        static void ThreadCurrentStatus()
        {
            ThreadWorker[] workers = ThreadWorker.GetAllInstances().ToArray();
            HashSet<ThreadWorker> activeWorkers = new HashSet<ThreadWorker>(workers.Where(w => w.IsAlive));

            StringBuilder builder = new StringBuilder();
            builder.Append($"{nameof(ThreadWorker)} Status: total {workers.Length}, active {activeWorkers.Count}, undecided {ThreadWorker.RealThread - workers.Length}");
            for (int i = 0; i < workers.Length; i++)
            {
                builder.AppendLine();
                builder.Append(workers[i].ThreadId);
                builder.Append(": ");
                if (activeWorkers.Contains(workers[i]))
                {
                    builder.Append("<color=green>active</color>");
                }
                else
                {
                    builder.Append("<color=gray>dispose</color>");
                }
            }
            Debug.Log(builder.ToString());
        }
        [MenuItem("Tools/XOR/Therad/StopAll")]
        static void ThreadStopAll()
        {
            ThreadWorker.ReleaseAllInstances();
        }
    }
}
