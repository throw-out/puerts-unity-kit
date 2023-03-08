using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace HR
{
    public class ManagerGUI : EditorWindow
    {

        [MenuItem("Tools/CDP/HR Manager")]
        private static void ShowWindow()
        {
            var window = GetWindow<ManagerGUI>();
            window.titleContent = new GUIContent("ManagerGUI");
            window.Show();
        }
        private List<ProfileView> profiles;
        private ProfileView current;

        private void OnEnable()
        {
            ReadProfiles();
            current = profiles.Count > 0 ? profiles[0] : null;
        }
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(200f));
            RenderProfiles();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            RenderCurrentMenu();
            GUILayout.Space(20f);
            RenderCurrentProfile();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        void RenderProfiles()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            if (GUILayout.Button("Add Profile"))
            {
                profiles.Add(new ProfileView(new Profile()));
                SaveProfiles();
                if (current == null)
                {
                    current = profiles[0];
                }
            }
            GUILayout.Space(20f);
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
                profiles.RemoveAt(removeIndex);
                SaveProfiles();
            }
        }
        void RenderCurrentMenu()
        {
            GUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(current == null || current.IsAlive))
            {
                if (GUILayout.Button("Start"))
                {
                    current?.Start();
                }
            }
            using (new EditorGUI.DisabledScope(current == null || !current.IsAlive))
            {
                if (GUILayout.Button("Stop"))
                {
                    current?.Stop();
                }
            }
            GUILayout.EndHorizontal();
        }
        void RenderCurrentProfile()
        {
            if (current == null)
                return;
            current.OnGUI();
            if (current.Dirty)
            {
                current.Apply();
                SaveProfiles();
            }
        }

        void ReadProfiles()
        {
            profiles = Prefs.Profiles.Read()
                           ?.Select(p => new ProfileView(p))
                           .ToList();
            if (profiles == null)
            {
                profiles = new List<ProfileView>();
            }
        }
        void SaveProfiles()
        {
            Prefs.Profiles.Save(profiles?.Select(v => v.profile).ToArray());
        }
    }
}
