using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HR
{
    internal class ProfileView : IDisposable
    {
        const float HEADER_WIDTH = 100f;
        internal readonly Profile profile;
        private Debugger debugger;
        private FileWacher watcher;
        private List<string> extnames;
        private DateTime nextRetry = DateTime.MinValue;

        public string Name => string.IsNullOrEmpty(profile.name) ? $"{profile.host}:{profile.port}" : profile.name;
        public bool IsOpen => debugger != null && debugger.IsOpen;
        public bool IsAlive => debugger != null && debugger.IsAlive;
        public bool IsReadyHost => !string.IsNullOrEmpty(profile.host);
        public bool IsReadyPath => !string.IsNullOrEmpty(profile.path) && Directory.Exists(profile.path);
        public bool Dirty { get; private set; }
        public bool Auto { get; private set; }

        public ProfileView(Profile profile)
        {
            this.profile = profile;
        }
        public void Start()
        {
            if (IsAlive)
                return;
            if (!IsReadyHost)
            {
                Debug.LogError($"host is empty.");
                return;
            }
            if (!IsReadyPath)
            {
                Debug.LogError($"The path is empty or does not exist: {profile.path}.");
                return;
            }
            if (debugger == null)
            {
                debugger = new Debugger();
            }
            UpdateExtnames();
            debugger.trace = profile.trace;
            debugger.ignoreCase = profile.ignoreCase;
            debugger.startupCheck = profile.startupCheck;
            debugger.Open(profile.host, profile.port);

            StartWatcher();
            Auto = this.profile.reconnect;
        }
        public void Stop()
        {
            Auto = false;
            StopWatcher();
            if (!IsAlive)
                return;
            debugger.Close();
        }
        public void Update(string filepath, string extname)
        {
            if (!IsAlive)
                return;
            if (!string.IsNullOrEmpty(extname) && (extnames == null || !extnames.Contains(extname)))
                return;
            debugger.Update(filepath);
        }
        public void Apply()
        {
            Dirty = false;
        }
        public void OnGUI()
        {
            using (new EditorGUI.DisabledScope(IsAlive || Auto))
            {
                RendererConfig();
            }
        }
        public void Tick()
        {
            if (this.IsAlive || !Auto)
                return;
            if (nextRetry == DateTime.MinValue)
            {
                nextRetry = DateTime.Now + TimeSpan.FromMilliseconds(profile.reconnectDelay);
            }
            else if (DateTime.Now > nextRetry)
            {
                nextRetry = DateTime.MinValue;
                if (IsReadyHost && IsReadyPath) Start();
            }
        }

        void UpdateExtnames()
        {
            if (extnames == null) extnames = new List<string>();
            extnames.Clear();

            Func<FileType, bool> @is = (type) => (type & profile.file) == type;

            if (@is(FileType.js)) extnames.Add(".js");
            if (@is(FileType.js_txt)) extnames.Add(".js.txt");
            if (@is(FileType.jsx)) extnames.Add(".jsx");
            if (@is(FileType.cjs)) extnames.Add(".cjs");
            if (@is(FileType.mjs)) extnames.Add(".mjs");
            if (@is(FileType.json)) extnames.Add(".json");
        }
        void StartWatcher()
        {
            if (watcher != null)
                StopWatcher();

            watcher = new FileWacher();
            watcher.AddWatcher(profile.path);
            watcher.OnChanged((filepath, type) =>
            {
                if (filepath.EndsWith(".js.txt"))
                {
                    Update(filepath, ".js.txt");
                }
                else
                {
                    Update(filepath, Path.GetExtension(filepath));
                }
            });
            watcher.Start();
            if (profile.trace) Debug.Log($"Watcher Listen: {profile.path}");
        }
        void StopWatcher()
        {
            if (watcher == null)
                return;
            watcher.Stop();
            watcher.Dispose();
            watcher = null;
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
            GUILayout.Label(Host, GUILayout.Width(HEADER_WIDTH));
            var newHost = GUILayout.TextField(profile.host);
            GUILayout.EndHorizontal();
            if (profile.host != newHost)
            {
                profile.host = newHost;
                Dirty |= true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(Port, GUILayout.Width(HEADER_WIDTH));
            var newPort = (ushort)EditorGUILayout.IntField(profile.port);
            GUILayout.EndHorizontal();
            if (profile.port != newPort)
            {
                profile.port = newPort;
                Dirty |= true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(IgnoreCase, GUILayout.Width(HEADER_WIDTH));
            var newIgnoreCase = GUILayout.Toggle(profile.ignoreCase, string.Empty);
            GUILayout.EndHorizontal();
            if (profile.ignoreCase != newIgnoreCase)
            {
                profile.ignoreCase = newIgnoreCase;
                Dirty |= true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(Trace, GUILayout.Width(HEADER_WIDTH));
            var newTrace = GUILayout.Toggle(profile.trace, string.Empty);
            GUILayout.EndHorizontal();
            if (profile.trace != newTrace)
            {
                profile.trace = newTrace;
                Dirty |= true;
            }

            RenderReconnect();

            GUILayout.BeginHorizontal();
            GUILayout.Label(StartupCheck, GUILayout.Width(HEADER_WIDTH));
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
            GUILayout.Label(WatchPath, GUILayout.Width(HEADER_WIDTH));
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
            GUILayout.Label(Reconnect, GUILayout.Width(HEADER_WIDTH));
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
                GUILayout.Label(ReconnectDelay, GUILayout.Width(HEADER_WIDTH));
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
            GUILayout.Label(Extname, GUILayout.Width(HEADER_WIDTH));
            var newExtname = (FileType)EditorGUILayout.EnumFlagsField(profile.file);
            GUILayout.EndHorizontal();
            if (profile.file != newExtname)
            {
                profile.file = newExtname;
                Dirty |= true;
            }
        }
        public void Dispose()
        {
            this.Stop();
        }
        static GUIContent Host = new GUIContent("Host", "远程调试地址");
        static GUIContent Port = new GUIContent("Port", "远程调试端口, 对应new JsEnv(loader, por)中的port参数, 其为ushort类型(0-65535)");
        static GUIContent IgnoreCase = new GUIContent("Ignore Case", "不区分文件路径中的大小写");
        static GUIContent Trace = new GUIContent("Trace", "输出脚本更新信息");
        static GUIContent StartupCheck = new GUIContent("Startup Check", "连接成功后检查所有脚本是否需要更新(将导致启动时间变长)");
        static GUIContent WatchPath = new GUIContent("Watch Path", "监听的文件夹路径(与已载入脚本路径一致, 对应ILoader.ReadFile debugpath参数)");
        static GUIContent Reconnect = new GUIContent("Reconnect", "连接断开时, 自动重连.");
        static GUIContent ReconnectDelay = new GUIContent("Reconnect Delay", "重连延迟");
        static GUIContent Extname = new GUIContent("Extname", "监听的文件类型");
    }
}
