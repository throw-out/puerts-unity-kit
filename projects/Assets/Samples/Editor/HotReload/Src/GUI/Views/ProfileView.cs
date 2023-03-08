using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HR
{
    internal class ProfileView
    {
        const float HEADER_WIDTH = 100f;
        internal readonly Profile profile;
        private Debugger debugger;

        public string Name => string.IsNullOrEmpty(profile.name) ? $"{profile.host}:{profile.port}" : profile.name;
        public bool IsAlive => debugger != null && debugger.IsAlive;
        public bool Dirty { get; private set; }
        public bool Auto
        {
            get => profile.auto;
            set
            {
                if (profile.auto == value)
                    return;
                profile.auto = value;
                Dirty = true;
            }
        }

        public ProfileView(Profile profile)
        {
            this.profile = profile;
        }
        public void Start()
        {
            if (IsAlive)
                return;
            if (string.IsNullOrEmpty(profile.host))
                return;
            if (debugger == null)
            {
                debugger = new Debugger();
            }
            debugger.trace = profile.trace;
            debugger.ignoreCase = profile.ignoreCase;
            debugger.Open(profile.host, profile.port);
        }
        public void Stop()
        {
            if (debugger == null)
                return;
            debugger.Close();
        }
        public void OnGUI()
        {
            using (new EditorGUI.DisabledScope(IsAlive))
            {
                RendererConfig();
            }
        }
        public void Apply()
        {
            Dirty = false;
        }

        void RendererConfig()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.Width(HEADER_WIDTH));
            var newName = GUILayout.TextField(profile.name);
            GUILayout.EndHorizontal();
            if (profile.name != newName)
            {
                profile.name = newName;
                Dirty |= true;
            }

            RenderPath();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Host", GUILayout.Width(HEADER_WIDTH));
            var newHost = GUILayout.TextField(profile.host);
            GUILayout.EndHorizontal();
            if (profile.host != newHost)
            {
                profile.host = newHost;
                Dirty |= true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Port", GUILayout.Width(HEADER_WIDTH));
            var newPort = (ushort)EditorGUILayout.IntField(profile.port);
            GUILayout.EndHorizontal();
            if (profile.port != newPort)
            {
                profile.port = newPort;
                Dirty |= true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ignore Case", GUILayout.Width(HEADER_WIDTH));
            var newIgnoreCase = GUILayout.Toggle(profile.ignoreCase, string.Empty);
            GUILayout.EndHorizontal();
            if (profile.ignoreCase != newIgnoreCase)
            {
                profile.ignoreCase = newIgnoreCase;
                Dirty |= true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Trace", GUILayout.Width(HEADER_WIDTH));
            var newTrace = GUILayout.Toggle(profile.trace, string.Empty);
            GUILayout.EndHorizontal();
            if (profile.trace != newTrace)
            {
                profile.trace = newTrace;
                Dirty |= true;
            }

            RenderReconnect();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Startup Check", GUILayout.Width(HEADER_WIDTH));
            var newStartupCheck = GUILayout.Toggle(profile.startupCheck, string.Empty);
            GUILayout.EndHorizontal();
            if (profile.startupCheck != newStartupCheck)
            {
                profile.startupCheck = newStartupCheck;
                Dirty |= true;
            }

            RenderFileType();
        }
        void RenderPath()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Watch Path", GUILayout.Width(HEADER_WIDTH));
            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.TextField(profile.path);
            }
            if (GUILayout.Button("Select", GUILayout.Width(100f)))
            {
                var newPath = EditorUtility.OpenFolderPanel(
                    "Select Watch Folder",
                    profile.path,
                    string.Empty
                );
                if (!string.IsNullOrEmpty(newPath) && newPath != profile.path)
                {
                    profile.path = newPath;
                    Dirty |= true;
                }
            }
            GUILayout.EndHorizontal();
        }
        void RenderReconnect()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Reconnect", GUILayout.Width(HEADER_WIDTH));
            var newReconnect = GUILayout.Toggle(profile.reconnect, string.Empty);
            GUILayout.EndHorizontal();
            if (profile.reconnect != newReconnect)
            {
                profile.reconnect = newReconnect;
                Dirty |= true;
            }

            using (new EditorGUI.DisabledScope(!newReconnect))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Reconnect Delay(ms)", GUILayout.Width(HEADER_WIDTH));
                var newDelay = (ulong)EditorGUILayout.LongField((long)profile.reconnectDelay);
                GUILayout.EndHorizontal();
                if (profile.reconnectDelay != newDelay)
                {
                    profile.reconnectDelay = newDelay;
                    Dirty |= true;
                }
            }
        }
        void RenderFileType()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Extname", GUILayout.Width(HEADER_WIDTH));
            var newExtname = (FileType)EditorGUILayout.EnumFlagsField(profile.file);
            GUILayout.EndHorizontal();
            if (profile.file != newExtname)
            {
                profile.file = newExtname;
                Dirty |= true;
            }
        }
    }
}
