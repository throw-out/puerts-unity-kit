﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace HR
{
    using WebSocketState = WebSocketSharp.WebSocketState;

    public class Debugger
    {
        const int MAX_SCRIPTS_CACHE_SIZE = 1000000;
        private static readonly Regex ESMImport = new Regex(@"(^import [\S ]+|\r?\nimport [\S ]+)");
        private static readonly Regex ESMExport = new Regex(@"(^export [\S ]+|\r?\nexport [\S ]+)");

        /// <summary>忽略文件路径大小写 </summary>
        public bool ignoreCase { get; set; }
        public bool trace { get; set; }
        public bool startupCheck { get; set; }

        private CDP.Chrome chrome;
        private CDP.Domains.Debugger debugger;
        private CDP.Domains.Runtime runtime;

        private HashSet<string> scriptUrls;
        private Dictionary<string, string> scriptParsed;
        private Dictionary<string, string> scriptFailedToParse;
        private Dictionary<string, Locker> scriptLocks;
        private Dictionary<string, string> scriptSources;

        public bool IsOpen
        {
            get => this.chrome != null && this.chrome.State == WebSocketState.Open;
        }
        public bool IsAlive
        {
            get => this.chrome != null && (this.chrome.State == WebSocketState.Connecting || this.chrome.State == WebSocketState.Open);
        }

        public async void Open(string host, ushort port)
        {
            if (IsAlive)
                throw new InvalidOperationException("socket is opened!");
            this.Close();

            try
            {
                if (this.trace) Debug.Log($"connect: ws://{host}:{port}");

                this.chrome = new CDP.Chrome(host, port);
                this.debugger = new CDP.Domains.Debugger(this.chrome);
                this.runtime = new CDP.Domains.Runtime(this.chrome);

                this.debugger.OnScriptParsed(ScriptParsedHandler);
                this.debugger.OnScriptFailedToParse(ScriptFailedToParseHandler);

                //connect to server
                await ConnectTo(this.chrome);
                if (this.trace) Debug.Log($"connect success!");

                //enable
                await this.runtime.Enable();
                await this.debugger.Enable(new CDP.Domains.Debugger.EnableParameters()
                {
                    maxScriptsCacheSize = MAX_SCRIPTS_CACHE_SIZE,
                });
            }
            catch (Exception e)
            {
                this.Close();
                Debug.LogError(e);
            }
        }
        public void Close()
        {
            if (this.chrome == null)
                return;
            var chrome = this.chrome;

            this.chrome = null;
            this.debugger = null;
            this.runtime = null;
            this.scriptUrls = null;
            this.scriptParsed = null;
            this.scriptFailedToParse = null;

            chrome.Clear();
            chrome.Close();
        }
        public void Update(string filepath)
        {
            if (HasSystemIllegalCharacters(filepath) || !File.Exists(filepath))
                return;
            this.Update(filepath, File.ReadAllText(filepath));
        }
        public void Update(string filepath, string scriptSource)
        {
            if (!this.IsOpen)
                throw new InvalidOperationException("socket is closed!");

            if (this.trace)
            {
                Debug.Log($"chaneg: {filepath}");
            }
            filepath = GetNormalizePath(filepath, this.ignoreCase);
            this.PushUpdate(GetNormalizePath(filepath, this.ignoreCase), scriptSource);
        }
        public IEnumerable<string> GetScriptUrls()
        {
            return this.scriptUrls;
        }
        public async Task<string> GetScriptSource(string filepath)
        {
            if (!this.IsOpen)
                throw new InvalidOperationException("socket is closed!");
            filepath = GetNormalizePath(filepath, this.ignoreCase);

            string scriptId;
            if (this.scriptParsed == null || !this.scriptParsed.TryGetValue(filepath, out scriptId))
                return null;
            var exist = await this.debugger.GetScriptSource(new CDP.Domains.Debugger.GetScriptSourceParameters()
            {
                scriptId = scriptId
            });
            if (exist != null)
            {
                return exist.scriptSource;
            }
            return null;
        }

        async void PushUpdate(string filepath, string scriptSource)
        {
            string scriptId;
            if (this.scriptParsed == null || !this.scriptParsed.TryGetValue(filepath, out scriptId))
                return;
            if (!ESMImport.IsMatch(scriptSource) && !ESMExport.IsMatch(scriptSource))
            {
                scriptSource = ("(function (exports, require, module, __filename, __dirname) { " + scriptSource + "\n});");
            }
            //当前缓存的数据源已经同步
            if (GetLockScriptSource(scriptId) == scriptSource)
                return;
            //lock request
            var @lock = await Lock(scriptId);
            SetLockScriptSource(scriptId, scriptSource);
            try
            {
                if (this.debugger == null)
                    return;

                if (this.trace) Debug.Log($"check: {scriptId} - {filepath}");
                var exist = await this.debugger.GetScriptSource(new CDP.Domains.Debugger.GetScriptSourceParameters()
                {
                    scriptId = scriptId
                });
                if (exist == null || exist.scriptSource == scriptSource || this.debugger == null)
                    return;

                if (this.trace) Debug.Log($"send: {scriptId} - {filepath}");
                var resposne = await this.debugger.SetScriptSource(new CDP.Domains.Debugger.SetScriptSourceParameters()
                {
                    scriptId = scriptId,
                    scriptSource = scriptSource
                });
                if (this.trace) Debug.Log($"completed: {scriptId} - {filepath}" /**+ Newtonsoft.Json.JsonConvert.SerializeObject(resposne)*/);
            }
            finally
            {
                SetLockScriptSource(scriptId, null);
                @lock.Release();    //release lock
            }
        }
        async Task<Locker.IHandler> Lock(string key)
        {
            if (this.scriptLocks == null)
            {
                this.scriptLocks = new Dictionary<string, Locker>();
            }
            Locker locker;
            if (!this.scriptLocks.TryGetValue(key, out locker))
            {
                locker = new Locker();
                this.scriptLocks.Add(key, locker);
            }
            return await locker.Acquire(0);
        }
        void SetLockScriptSource(string key, string scriptSource)
        {
            if (this.scriptSources == null)
            {
                this.scriptSources = new Dictionary<string, string>();
            }
            this.scriptSources[key] = scriptSource;
        }
        string GetLockScriptSource(string key)
        {
            string scriptSource;
            if (this.scriptSources != null && this.scriptSources.TryGetValue(key, out scriptSource))
            {
                return scriptSource;
            }
            return null;
        }
        void ScriptParsedHandler(CDP.Domains.Debugger.OnScriptParsedParameters data)
        {
            if (data == null || string.IsNullOrEmpty(data.url) || string.IsNullOrEmpty(data.scriptId))
                return;

            if (this.trace) Debug.Log($"scriptParsed: {data.url}");

            var scriptId = data.scriptId;
            var filepath = GetNormalizePath(data.url, this.ignoreCase);

            if (this.scriptUrls == null)
            {
                this.scriptUrls = new HashSet<string>();
            }
            if (this.scriptParsed == null)
            {
                this.scriptParsed = new Dictionary<string, string>();
            }
            this.scriptParsed[scriptId] = filepath;
            this.scriptParsed[filepath] = scriptId;
            this.scriptUrls.Add(data.url);

            if (this.startupCheck) Update(data.url);
        }
        void ScriptFailedToParseHandler(CDP.Domains.Debugger.OnScriptFailedToParseParameters data)
        {
            if (data == null || string.IsNullOrEmpty(data.url) || string.IsNullOrEmpty(data.scriptId))
                return;
            if (this.trace) Debug.Log($"scriptFailedToParse: {data.url}");

            var scriptId = data.scriptId;
            var filepath = GetNormalizePath(data.url, this.ignoreCase);

            if (this.scriptFailedToParse == null)
            {
                this.scriptFailedToParse = new Dictionary<string, string>();
            }
            this.scriptFailedToParse[scriptId] = filepath;
            this.scriptFailedToParse[filepath] = scriptId;
        }

        static char[] systemIllegalCharacters = new char[]{
#if UNITY_STANDALONE_WIN
            '*',
            '?',
            '"',
            '<',
            '>',
            '|',
#endif
        };
        static bool HasSystemIllegalCharacters(string path)
        {
            return systemIllegalCharacters.Length > 0 && systemIllegalCharacters.FirstOrDefault(@char => path.Contains(@char)) != default(char);
        }
        static string GetNormalizePath(string path, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            if (!HasSystemIllegalCharacters(path))
            {
                path = Path.GetFullPath(path);
            }
            if (ignoreCase)
            {
                path = path.ToLower();
            }
            return path.Replace("\\", "/");
        }
        static async Task ConnectTo(CDP.Chrome chrome)
        {
            chrome.Start();
            await new CDP.Promise((resolve, reject) =>
            {
                if (chrome.IsOpen)
                {
                    resolve();
                    return;
                }
                Action<string> next = (error) =>
                {
                    if (resolve == null)
                        return;
                    if (error != null)
                    {
                        reject(new Exception(error));
                    }
                    else
                    {
                        resolve();
                    }
                    resolve = null;
                    reject = null;
                };
                chrome.Once("open", () => next(null));
                chrome.Once<ushort, string>("close", (code, reson) => next(reson));
            });
        }

        class Locker
        {
            public bool IsLocked => locked;
            public int Pending => handlers.Count;
            private bool locked;
            private List<Tuple<int, Action>> handlers =
                new List<Tuple<int, Action>>();
            private Handler handler;

            public async Task<IHandler> Acquire(int priority)
            {
                await this.Lock(priority);

                this.handler = new Handler(Move);
                return this.handler;
            }
            public void Reset()
            {
                this.locked = false;
                this.handlers = null;
                if (this.handler != null)
                {
                    this.handler.Reset();
                    this.handler = null;
                }
            }

            private async Task Lock(int priority)
            {
                while (this.locked)
                {
                    await new CDP.Promise(resolve =>
                    {
                        this.handlers.Add(new Tuple<int, Action>(priority, resolve));
                    });
                }
                this.locked = true;
            }
            private void Move()
            {
                this.locked = false;
                if (this.handlers == null || this.handlers.Count == 0)
                    return;
                this.handlers.OrderBy(o => o.Item1);

                var first = this.handlers[0];
                this.handlers.RemoveAt(0);
                first.Item2();
            }

            public interface IHandler
            {
                void Release();
            }
            class Handler : IHandler
            {
                public Action callback { get; private set; }
                public Handler(Action callback)
                {
                    this.callback = callback;
                }
                public void Reset()
                {
                    this.callback = null;
                }
                public void Release()
                {
                    if (this.callback == null)
                        return;
                    var fn = this.callback;
                    this.callback = null;
                    fn();
                }
            }
        }
    }
}
