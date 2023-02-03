using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace XOR
{
    internal class EditorFileWatcher : Singleton<EditorFileWatcher>, IDisposable
    {
        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private readonly Queue<FileSystemEventArgs> changedEvents = new Queue<FileSystemEventArgs>();
        private Action<string, WatcherChangeTypes> onChanged;
        private bool started = false;

        public void AddWatcher(string path, string filter = null)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.BeginInit();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite;
            watcher.IncludeSubdirectories = true;
            if (filter != null) watcher.Filter = filter;
            watcher.Deleted += (sender, e) => this.Enqueue(e);
            watcher.Created += (sender, e) => this.Enqueue(e);
            watcher.Changed += (sender, e) => this.Enqueue(e);
            watcher.Renamed += (sender, e) => this.Enqueue(e);
            watcher.EndInit();

            lock (watcher)
            {
                watchers.Add(watcher);
            }
            if (this.started) this.StartWatchers();
        }
        public void OnChanged(Action<string, WatcherChangeTypes> changed)
        {
            onChanged += changed;
        }
        public void Start(bool runInTask = false)
        {
            if (this.IsDestroyed)
                throw new InvalidOperationException();
            if (this.started)
                return;
            this.started = true;
            if (runInTask)
            {
                Task.Run(this.StartWatchers);
            }
            else
            {
                this.StartWatchers();
            }
        }
        public void Stop()
        {
            if (!this.started)
                return;
            this.started = false;
            this.StopWatchers();
        }
        void StartWatchers()
        {
            lock (watchers)
            {
                foreach (var watcher in watchers)
                {
                    if (this.IsDestroyed) break;
                    watcher.EnableRaisingEvents = true;
                }
            }
        }
        void StopWatchers()
        {
            lock (watchers)
            {
                foreach (var watcher in watchers)
                {
                    watcher.EnableRaisingEvents = false;
                }
            }
        }
        void Enqueue(FileSystemEventArgs e)
        {
            PrintWarningIfDestroyed(e);
            lock (changedEvents)
            {
                changedEvents.Enqueue(e);
            }
        }
        void Enqueue(RenamedEventArgs e)
        {
            PrintWarningIfDestroyed(e);
            lock (changedEvents)
            {
                changedEvents.Enqueue(new FileSystemEventArgs(WatcherChangeTypes.Deleted, Path.GetDirectoryName(e.OldFullPath), e.OldName));
                changedEvents.Enqueue(new FileSystemEventArgs(WatcherChangeTypes.Created, Path.GetDirectoryName(e.FullPath), e.Name));
            }
        }
        void PrintWarningIfDestroyed(FileSystemEventArgs e)
        {
            if (!this.IsDestroyed)
                return;
            Logger.LogWarning(
$@"⚠⚠⚠ System.IO.FileSystemWatcher实例未按照预期正常关闭, 它在当前Unity版本下无法正常工作, 请更新至最新LTS版后再次尝试.
Unity: {UnityEngine.Application.unityVersion}
Thread: {System.Threading.Thread.CurrentThread.ManagedThreadId}
Event: {Enum.GetName(typeof(WatcherChangeTypes), e.ChangeType)} - {e.FullPath}
");
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
            this.Stop();
            this.onChanged = null;
            lock (watchers)
            {
                foreach (var watcher in watchers)
                {
                    watcher.Dispose();
                }
                this.watchers.Clear();
            }
            this.changedEvents.Clear();
            GC.SuppressFinalize(this);
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