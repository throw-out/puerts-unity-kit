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
        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            EditorApplicationHandler.dispose += Dispose;
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    if (Prefs.ASTEnable && !EditorApplicationUtil.IsRunning())
                    {
                        EditorApplicationUtil.Start();
                    }
                    break;
                default:
                    if (EditorApplicationUtil.IsRunning())
                    {
                        EditorApplicationUtil.Stop(true, false);
                    }
                    break;
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


        [MenuItem("Tools/XOR/Configure/Settings", false, 0)]
        static void OpenSettings()
        {
            Selection.activeObject = XOR.Settings.Load(true, true);
        }
        [MenuItem("Tools/XOR/Configure/BehaviourSettings", false, 0)]
        static void OpenBehaviourSettings()
        {
            Selection.activeObject = XOR.BehaviourSettings.Load(true, true);
        }


        #region Language菜单项
        const string kMenuZHCN = "Tools/XOR/Language/简体中文";
        const string kMenuEN = "Tools/XOR/Language/English";

        [MenuItem(kMenuZHCN, false, 0)]
        static void LanguageZHCN() => Prefs.Language.SetValue((int)Language.Env.ZH_CN);
        [MenuItem(kMenuZHCN, true, 0)]
        static bool LanguageZHCNValidate()
        {
            bool isChecked = Prefs.Language.GetValue() == (int)Language.Env.ZH_CN;
            UnityEditor.Menu.SetChecked(kMenuZHCN, isChecked);
            return !isChecked;
        }
        [MenuItem(kMenuEN, false, 0)]
        static void LanguageENUS() => Prefs.Language.SetValue((int)Language.Env.EN_US);
        [MenuItem(kMenuEN, true, 0)]
        static bool LanguageENUSValidate()
        {
            bool isChecked = Prefs.Language.GetValue() == (int)Language.Env.EN_US;
            UnityEditor.Menu.SetChecked(kMenuEN, isChecked);
            return !isChecked;
        }
        #endregion



        #region 开发者模式
        const string kMenuDeveloper = "Tools/XOR/DeveloperMode";
        [MenuItem(kMenuDeveloper, false, 0)]
        static void Developer() => Prefs.DeveloperMode.SetValue(!Prefs.DeveloperMode);
        [MenuItem(kMenuDeveloper, true, 0)]
        static bool DeveloperValidate()
        {
            UnityEditor.Menu.SetChecked(kMenuDeveloper, Prefs.DeveloperMode);
            return true;
        }
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
