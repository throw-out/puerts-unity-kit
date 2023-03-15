using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

namespace HR
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class ManagerGUI : EditorWindow
    {
        private static List<ProfileView> profiles;
        private static ProfileView current;
        private static bool saveExecute;

        static ManagerGUI()
        {
            EditorApplicationHandler.update += Tick;
            EditorApplicationHandler.delayCall += Init;
        }

        [MenuItem("Tools/CDP/HR Manager")]
        private static void ShowGUI()
        {
            var window = GetWindow<ManagerGUI>();
            window.titleContent = new GUIContent("ManagerGUI");
            window.Show();
        }
        [MenuItem("Tools/CDP/Restart All")]
        private static void RestartAll()
        {
            if (profiles == null)
                return;
            foreach (var profile in profiles)
            {
                profile.Stop();
                profile.Start();
            }
        }
        [MenuItem("Tools/CDP/Stop All")]
        private static void StopAll()
        {
            if (profiles == null)
                return;
            foreach (var profile in profiles)
            {
                profile.Stop();
            }
        }
        static void Init()
        {
            ReadSettings();
            current = profiles.Count > 0 ? profiles[0] : null;

            foreach (var profile in profiles)
            {
                if (!profile.Execute)
                    continue;
                if (saveExecute) profile.Start();
                else profile.Stop();
            }
        }
        static void Tick()
        {
            if (profiles == null)
                return;
            foreach (var profile in profiles)
            {
                profile.Tick();
            }
        }

        void OnGUI()
        {
            if (profiles == null)
                return;
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(200f));
            RenderProfiles();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            RenderCurrentProfile();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            RenderMenu();
        }

        void RenderMenu()
        {
            GUILayout.BeginHorizontal();
            var newState = EditorGUILayout.Toggle("Save States", saveExecute);
            if (newState != saveExecute)
            {
                saveExecute = newState;
                SaveSettings();
            }
            if (GUILayout.Button("Local Network"))
            {
                ScanLocalListeners();
            }
            GUILayout.EndHorizontal();
        }
        void RenderProfiles()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Profile"))
            {
                profiles.Add(new ProfileView(new Profile()));
                SaveSettings();
                if (current == null)
                {
                    current = profiles[0];
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);
            GUILayout.BeginVertical("HelpBox");
            GUILayout.BeginScrollView(Vector2.zero);
            int removeIndex = -1;
            for (int i = 0; i < profiles.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(profiles[i].Name))
                {
                    current = profiles[i];
                }
                if (GUILayout.Button("x", GUILayout.Width(20f)))
                {
                    removeIndex = i;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            if (removeIndex >= 0)
            {
                if (current == profiles[removeIndex])
                {
                    current = removeIndex < profiles.Count - 1 ? profiles[removeIndex + 1] : removeIndex > 0 ? profiles[removeIndex - 1] : null;
                }
                profiles[removeIndex].Stop();
                profiles.RemoveAt(removeIndex);
                SaveSettings();
            }
        }
        void RenderCurrentProfile()
        {
            GUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(current == null || current.IsAlive || current.Execute))
            {
                if (GUILayout.Button("Start") && current != null)
                {
                    current.Start();
                }
            }
            using (new EditorGUI.DisabledScope(current == null || !current.IsAlive && !current.Execute))
            {
                if (GUILayout.Button("Stop"))
                {
                    current?.Stop();
                }
            }
            GUILayout.EndHorizontal();

            if (current == null)
                return;

            GUILayout.Space(5f);
            current.OnGUI();
            if (current.Dirty)
            {
                current.Apply();
                SaveSettings();
            }
        }


        async void ScanLocalListeners()
        {
            try
            {
                var ports = await InspectorScanner.GetActiveInspector((token, current, total) =>
                {
                    if (token.IsCancelled)
                        return;
                    bool cancel = EditorUtility.DisplayCancelableProgressBar("Scanning", $"Scan Local Listeners: {current}/{total}", current / (float)total);
                    if (cancel)
                    {
                        token.Cancel();
                    }
                });
                EditorUtility.ClearProgressBar();
                if (ports == null || profiles == null)
                    return;
                if (ports.Length == 0)
                {
                    EditorUtility.DisplayDialog("Scan Complete", $"Inspector: Empty", "OK");
                    return;
                }
                EditorUtility.DisplayDialog("Scan Complete", $"Inspector:\n{string.Join("\n", ports.Select(port => $"ws://localhost:{port}"))}", "OK");
                foreach (var port in ports)
                {
                    if (profiles.FirstOrDefault(p => p.profile.port == port && p.profile.host == "localhost") != null)
                        continue;
                    var profile = new Profile()
                    {
                        host = "localhost",
                        port = (ushort)port
                    };
                    profiles.Add(new ProfileView(profile));
                }
                SaveSettings();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }
        }

        static void ReadSettings()
        {
            profiles = Prefs.Profiles.Read()
                           ?.Select(p => new ProfileView(p))
                           .ToList();
            if (profiles == null)
            {
                profiles = new List<ProfileView>();
            }
            saveExecute = Prefs.SaveExecute.Read();
        }
        static void SaveSettings()
        {
            Prefs.Profiles.Save(profiles?.Select(v => v.profile).ToArray());
            Prefs.SaveExecute.Save(saveExecute);
        }
    }
}
