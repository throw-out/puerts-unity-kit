using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XOR
{
    internal class EditorFileWatcher : Singleton<EditorFileWatcher>, IDisposable
    {
        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private readonly Queue<FileSystemEventArgs> changedEvents = new Queue<FileSystemEventArgs>();
        private Action<string, WatcherChangeTypes> onChanged;

        public void AddWatcher(string path, string filter = null)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(path);
            if (filter != null) watcher.Filter = filter;

            watcher.Deleted += (sender, e) => this.Enqueue(e);
            watcher.Created += (sender, e) => this.Enqueue(e);
            watcher.Changed += (sender, e) => this.Enqueue(e);
            watcher.Renamed += (sender, e) => this.Enqueue(e);
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite;
            watcher.IncludeSubdirectories = true;
            //watcher.BeginInit();

            watcher.EnableRaisingEvents = true;
            watchers.Add(watcher);
        }
        public void OnChanged(Action<string, WatcherChangeTypes> changed)
        {
            onChanged += changed;
        }

        void Enqueue(FileSystemEventArgs e)
        {
            UnityEngine.Debug.Log($"Change({System.Threading.Thread.CurrentThread.ManagedThreadId}): {e.FullPath}");
            lock (changedEvents)
            {
                changedEvents.Enqueue(e);
            }
            UnityEngine.Debug.Log($"Change Events: {changedEvents.Count}| {this.GetHashCode()}| {this.IsDestroyed}| {string.Join(",", this.watchers.Select(o => o.EnableRaisingEvents))}");
        }
        void Enqueue(RenamedEventArgs e)
        {
            lock (changedEvents)
            {
                changedEvents.Enqueue(new FileSystemEventArgs(WatcherChangeTypes.Deleted, Path.GetDirectoryName(e.OldFullPath), e.OldName));
                changedEvents.Enqueue(new FileSystemEventArgs(WatcherChangeTypes.Created, Path.GetDirectoryName(e.FullPath), e.Name));
            }
        }
        void Tick()
        {
            if (changedEvents.Count == 0)
                return;
            List<FileSystemEventArgs> events = new List<FileSystemEventArgs>();
            lock (changedEvents)
            {
                while (changedEvents.Count > 0 && events.Count < 5)
                {
                    events.Add(changedEvents.Dequeue());
                }
            }
            foreach (var e in events)
            {
                onChanged?.Invoke(e.FullPath, e.ChangeType);
            }
        }
        public override void Init()
        {
            base.Init();
            this.RegisterHandlers();
        }
        public override void Release()
        {
            base.Release();
            this.Dispose();
        }
        public void Dispose()
        {
            this.UnregisterHandlers();
            this.onChanged = null;
            foreach (var watcher in watchers)
            {
                //watcher.EndInit();
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                //GC.SuppressFinalize(watcher);
            }
            watchers.Clear();
            changedEvents.Clear();
        }
        void RegisterHandlers()
        {
            EditorApplicationHandler.update += Tick;
            EditorApplicationHandler.dispose += Dispose;
        }
        void UnregisterHandlers()
        {
            EditorApplicationHandler.update -= Tick;
            EditorApplicationHandler.dispose -= Dispose;
        }
    }
}