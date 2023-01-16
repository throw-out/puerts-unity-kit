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
            EditorApplicationHandler.DelayCall += InitializeStart;
        }

        static void InitializeStart()
        {
            EditorApplicationHandler.DelayCall -= InitializeStart;
            if (Prefs.Enable && !EditorApplicationUtil.IsRunning())
            {
                EditorApplicationUtil.Start();
            }
        }


        [MenuItem("Tools/XOR/Settings", false, 0)]
        static void OpenSettings()
        {
            Selection.activeObject = XOR.Settings.Load(true, true);
        }


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


        [MenuItem("Tools/XOR/Component/SyncAll")]
        static void SyncAllComponents()
        {

        }

        [MenuItem("Tools/XOR/Therad/Current")]
        static void ThreadCurrentStatus()
        {
            ThreadWorker[] workers = ThreadWorker.GetAllInstances().ToArray();
            HashSet<ThreadWorker> activeWorkers = new HashSet<ThreadWorker>(workers.Where(w => w.IsAlive));

            StringBuilder builder = new StringBuilder();
            builder.Append($"{nameof(ThreadWorker)} total {workers.Length}, active {activeWorkers.Count}, undecided {ThreadWorker.RealThread - workers.Length}");
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
