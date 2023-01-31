using System;
using System.Collections.Generic;
using System.IO;

namespace XOR
{
    internal class EditorFileWatcher : Singleton<EditorFileWatcher>, IDisposable
    {
        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private Action<string, WatcherChangeTypes> onChanged;

        public void AddWatcher(string path, string filter = null)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(path);
            if (filter != null) watcher.Filter = filter;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.Deleted += OnDeleted;
            watcher.Created += OnCreated;
            watcher.Changed += OnChanged;
            watcher.Renamed += OnRenamed;

            watcher.BeginInit();
            watchers.Add(watcher);
        }
        public void OnChanged(Action<string, WatcherChangeTypes> changed)
        {
            onChanged += changed;
        }

        void OnDeleted(object sender, FileSystemEventArgs e)
        {
            onChanged?.Invoke(e.FullPath, WatcherChangeTypes.Deleted);
        }
        void OnCreated(object sender, FileSystemEventArgs e)
        {
            onChanged?.Invoke(e.FullPath, WatcherChangeTypes.Created);
        }
        void OnChanged(object sender, FileSystemEventArgs e)
        {
            onChanged?.Invoke(e.FullPath, WatcherChangeTypes.Changed);
        }
        void OnRenamed(object sender, RenamedEventArgs e)
        {
            onChanged?.Invoke(e.OldFullPath, WatcherChangeTypes.Deleted);
            onChanged?.Invoke(e.FullPath, WatcherChangeTypes.Created);
        }
        public override void Init()
        {
            base.Init();
        }
        public override void Release()
        {
            base.Release();
            this.Dispose();
        }
        public void Dispose()
        {
            this.onChanged = null;
            foreach (var watcher in watchers)
            {
                watcher.EndInit();
            }
            watchers.Clear();
        }
    }
}