using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Puerts;

namespace XOR
{
    public class MixerLoader : ILoader, IDisposable, IModuleChecker
    {
        private static readonly Regex ESMImport = new Regex(@"(^import [\S ]+|\r?\nimport [\S ]+)");
        private static readonly Regex ESMExport = new Regex(@"(^export [\S ]+|\r?\nexport [\S ]+)");
        /// <summary>
        /// 如果filepath不是合法的js文件名, 则为其追加后缀名后重试
        /// </summary>
        /// <value></value>
        private readonly string[] AppendExtensionNames = new string[]{
            ".js",
            ".cjs",
            ".mjs",
        };
        private readonly List<LoaderWraper> loaders;
        /// <summary>
        /// 如果文件不是合法的扩展名, 自动追加扩展名重试
        /// </summary>
        /// <value></value>
        public bool AppendExtension { get; set; } = true;
        /// <summary>
        /// 模块匹配规则
        /// </summary>
        public RegexMode Mode { get; set; } = RegexMode.Original | RegexMode.Content;

        public MixerLoader() : this(null) { }
        public MixerLoader(MixerLoader other)
        {
            this.loaders = new List<LoaderWraper>();
            if (other != null)
            {
                this.loaders.AddRange(other.loaders);
            }
        }

        public void Dispose()
        {
            foreach (var ploader in loaders)
            {
                ploader.Dispose();
            }
            loaders.Clear();
        }

        public bool FileExists(string filepath)
        {
            if (InnerFileExists(filepath))
            {
                return true;
            }
            if (AppendExtension && !ExtensionNameValidate(filepath))
            {
                foreach (string extName in AppendExtensionNames)
                {
                    string newFilepath = filepath + extName;
                    if (InnerFileExists(newFilepath))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string ReadFile(string filepath, out string debugpath)
        {
            string script = InnerReadFile(filepath, out debugpath);
            if (!string.IsNullOrEmpty(script))
                return script;
            if (AppendExtension && !ExtensionNameValidate(filepath))
            {
                foreach (string extName in AppendExtensionNames)
                {
                    string newFilepath = filepath + extName;
                    if (!InnerFileExists(newFilepath))
                        continue;
                    script = InnerReadFile(newFilepath, out debugpath);
                    if (!string.IsNullOrEmpty(script))
                        return script;
                }
            }
            debugpath = filepath;
            return null;
        }
        public bool IsESM(string filepath)
        {
            if (InnerIsESM(filepath))
            {
                return true;
            }
            if (AppendExtension && !ExtensionNameValidate(filepath))
            {
                foreach (string extName in AppendExtensionNames)
                {
                    string newFilepath = filepath + extName;
                    if (InnerIsESM(newFilepath))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddLoader(ILoader loader)
        {
            AddLoader(loader, 0, null);
        }
        public void AddLoader(ILoader loader, int index)
        {
            AddLoader(loader, index, null);
        }
        public void AddLoader(ILoader loader, Func<string, bool> match)
        {
            AddLoader(loader, 0, match);
        }
        public void AddLoader(ILoader loader, int index, Func<string, bool> match)
        {
            if (typeof(MixerLoader).IsAssignableFrom(loader.GetType()))
            {
                throw new ArgumentException("Nested instance is not allowed");
            }
            loaders.Add(new LoaderWraper(loader, index, match));
            loaders.Sort((v1, v2) => v1.Index > v2.Index ? 1 : v1.Index < v2.Index ? -1 : 0);
        }
        public bool RemoveLoader<T>() where T : ILoader
        {
            return RemoveLoader(typeof(T));
        }
        public bool RemoveLoader(Type type)
        {
            var loaders = GetLoaders(type);
            for (int i = 0; i < loaders.Length; i++)
            {
                RemoveLoader(loaders[i]);
            }
            return loaders.Length > 0;
        }
        public bool RemoveLoader(ILoader loader)
        {
            for (int i = loaders.Count - 1; i >= 0; i--)
            {
                if (loaders[i].Loader == loader)
                {
                    loaders.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public T[] GetLoaders<T>() where T : ILoader
        {
            return GetLoaders(typeof(T)) as T[];
        }
        public ILoader[] GetLoaders(Type type)
        {
            var results = new List<ILoader>();
            foreach (var package in this.loaders)
            {
                if (type.IsAssignableFrom(package.Loader.GetType()))
                {
                    results.Add(package.Loader);
                }
            }
            return results.ToArray();
        }

        bool InnerFileExists(string filepath)
        {
            foreach (var loader in loaders)
            {
                if (loader.FileExists(filepath))
                    return true;
            }
            return false;
        }
        string InnerReadFile(string filepath, out string debugpath)
        {
            foreach (var loader in loaders)
            {
                string script = loader.ReadFile(filepath, out debugpath);
                if (!string.IsNullOrEmpty(script))
                    return script; ;
            }
            debugpath = filepath;
            return null;
        }
        bool InnerIsESM(string filepath)
        {
            foreach (var loader in loaders)
            {
                if (!loader.FileExists(filepath))
                    continue;
                if (IsSupportMode(RegexMode.Original) && loader.IsESM(filepath))
                {
                    return true;
                }
                if (IsSupportMode(RegexMode.CommonJS))
                {
                    return false;
                }
                if (IsSupportMode(RegexMode.ESM))
                {
                    return true;
                }
                if (IsSupportMode(RegexMode.FileExtension))
                {
                    if (filepath.EndsWith(".mjs"))
                        return true;
                    if (filepath.EndsWith(".cjs"))
                        return false;
                }
                if (IsSupportMode(RegexMode.Content))
                {
                    string script = loader.ReadFile(filepath, out string debugpath);
                    return script != null && (ESMImport.IsMatch(script) || ESMExport.IsMatch(script));
                }
                break;
            }
            return false;
        }
        bool ExtensionNameValidate(string filepath)
        {
            return filepath.EndsWith(".js") ||
                filepath.EndsWith(".cjs") ||
                filepath.EndsWith(".mjs") ||
                filepath.EndsWith(".json");
        }


        bool IsSupportMode(RegexMode mode)
        {
            return (this.Mode & mode) == mode;
        }

        private class LoaderWraper
        {
            public int Index { get; private set; }
            public ILoader Loader { get; private set; }
            private Func<string, bool> match;
            public LoaderWraper(ILoader loader, int index, Func<string, bool> match)
            {
                this.Loader = loader;
                this.match = match;
                this.Index = index;
            }
            public bool FileExists(string filepath)
            {
                if (this.match != null && !this.match(filepath))
                    return false;
                if (!Loader.FileExists(filepath))
                    return false;
                if (Loader is Puerts.DefaultLoader)
                {
                    string script = Loader.ReadFile(filepath, out var debugpath);
                    return !string.IsNullOrEmpty(script);
                }
                return true;
            }
            public string ReadFile(string filepath, out string debugpath)
            {
                if (this.match != null && !this.match(filepath))
                {
                    debugpath = filepath;
                    return null;
                }
                return Loader.ReadFile(filepath, out debugpath);
            }
            public bool IsESM(string filepath)
            {
                if (this.match != null && !this.match(filepath))
                {
                    return false;
                }
                return Loader is IModuleChecker && ((IModuleChecker)Loader).IsESM(filepath);
            }
            public void Dispose()
            {
                if (typeof(IDisposable).IsAssignableFrom(Loader.GetType()))
                    ((IDisposable)Loader).Dispose();
            }
        }
        public enum RegexMode
        {
            /// <summary>
            /// 优先使用原始IModuleChecker接口, 如未实现走默认逻辑
            /// </summary>
            Original = 1,
            /// <summary>
            /// 统一处理为commonjs模块
            /// </summary>
            CommonJS = 2,
            /// <summary>
            /// 统一处理为esm模块
            /// </summary>
            ESM = 4,
            /// <summary>
            /// 文件后缀名为mjs时为esm模块, 否则为commonjs模块
            /// </summary>
            FileExtension = 8,
            /// <summary>
            /// 通过内容匹配, 存在import或export时为esm模块, 否则为commonjs模块
            /// </summary>
            Content = 16,
        }
    }

    public class FileLoader : ILoader
    {
        private readonly string outputRoot;
        private readonly string projectRoot;
        private HashSet<string> prereadFiles;
        private bool prereadAll;        //预读取全部文件(包含node_modules)

        public FileLoader(string outputRoot) : this(outputRoot, null, false) { }
        public FileLoader(string outputRoot, bool preread) : this(outputRoot, null, preread) { }
        public FileLoader(string outputRoot, string projectRoot) : this(outputRoot, projectRoot, false) { }

        public FileLoader(string outputRoot, string projectRoot, bool preread)
        {
            this.outputRoot = outputRoot;
            this.projectRoot = string.IsNullOrEmpty(projectRoot) ? outputRoot : projectRoot;
            if (preread) ReadDirectory(true);
        }
        /// <summary>预读取目录(包含node_modules目录), 减少IO次数</summary>
        public void ReadDirectory()
        {
            ReadDirectory(true);
        }
        public void ReadDirectory(bool prereadAll)
        {
            this.prereadAll = prereadAll;
            this.prereadFiles = new HashSet<string>();

            string dirPath = this.prereadAll ? projectRoot : outputRoot;
            if (Directory.Exists(dirPath))
            {
                ReadDirectory(dirPath);
            }
        }

        public bool FileExists(string filepath)
        {
            return ReadFileExists(GetFilePath(filepath));
        }

        public string ReadFile(string filepath, out string debugpath)
        {
            var path = GetFilePath(filepath);
            if (ReadFileExists(path))
            {
                debugpath = path;
                return File.ReadAllText(path);
            }
            debugpath = filepath;
            return null;
        }
        string GetFilePath(string filepath)
        {
#if UNITY_EDITOR
            var path = filepath;
            if (path.StartsWith("/") || path.StartsWith("\\"))
            {
                path = path.Substring(1);
            }
            if (!path.EndsWith(".js") && !path.EndsWith(".cjs") && !path.EndsWith(".mjs") && !path.EndsWith(".json"))
            {
                //Logger.LogWarning("unknown file extension: " + filepath);
            }

            if (path.StartsWith("node_modules/"))
                path = Path.Combine(projectRoot, path);
            else
                path = Path.Combine(outputRoot, path);
            if (ReadFileExists(path))
            {
#if UNITY_STANDALONE_WIN
                path = path.Replace("/", "\\");
#else
                path = path.Replace("\\", "/");
#endif
                return path;
            }
#endif
            return filepath;
        }

        bool ReadFileExists(string path)
        {
            if (prereadFiles != null && (
                prereadAll ||
                !path.Contains("node_modules/") &&
                !path.Contains("node_modules\\")
            ))
            {
#if UNITY_STANDALONE_WIN
                path = path.Replace("/", "\\").ToLower();
#endif
                return prereadFiles.Contains(path);
            }
            return File.Exists(path);
        }
        void ReadDirectory(string dirPath)
        {
            DirectoryInfo info = new DirectoryInfo(dirPath);
            foreach (var subDirectory in info.GetDirectories())
            {
                ReadDirectory(subDirectory.FullName);
            }
            foreach (var subFile in info.GetFiles())
            {
                string path = subFile.FullName;
#if UNITY_STANDALONE_WIN
                path = path.Replace("/", "\\").ToLower();
#endif
                prereadFiles.Add(path);
            }
        }
    }

    public class CacheLoader : ILoader, IDisposable
    {
        public string rootPath { get; set; }
        public bool ignoreCase { get; set; } = true;
        //缓存池
        private readonly Dictionary<string, string> scripts = new Dictionary<string, string>();

        public void Dispose()
        {
            this.scripts.Clear();
        }

        public bool FileExists(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                return false;
            if (filepath.StartsWith("/") || filepath.StartsWith("\\"))
            {
                filepath = filepath.Substring(1);
            }
            filepath = CombinePath(filepath);
            if (ignoreCase)
            {
                filepath = filepath.ToLower();
            }

            string script = null;
            scripts.TryGetValue(filepath, out script);
            return !string.IsNullOrEmpty(script);
        }
        public string ReadFile(string filepath, out string debugpath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                debugpath = null;
                return null;
            }
            if (filepath.StartsWith("/") || filepath.StartsWith("\\"))
            {
                filepath = filepath.Substring(1);
            }

            filepath = debugpath = CombinePath(filepath);
            if (ignoreCase)
            {
                filepath = filepath.ToLower();
            }

            string script = null;
            scripts.TryGetValue(filepath, out script);
            if (!string.IsNullOrEmpty(script))
                return script;

            return null;
        }

        public void AddScript(string filepath, string script)
        {
            filepath = CombinePath(filepath);
            if (ignoreCase)
            {
                filepath = filepath?.ToLower();
            }

            if (scripts.ContainsKey(filepath))
            {
                scripts.Remove(filepath);
            }
            scripts.Add(filepath, script);
        }
        public void AddScripts(Dictionary<string, string> scripts)
        {
            string filepath = null;
            foreach (var _script in scripts)
            {
                filepath = CombinePath(_script.Key);
                if (ignoreCase)
                {
                    filepath = filepath?.ToLower();
                }
                if (this.scripts.ContainsKey(filepath))
                {
                    this.scripts.Remove(filepath);
                }
                this.scripts.Add(filepath, _script.Value);
            }
        }
        public bool RemoveScript(string filepath)
        {
            filepath = CombinePath(filepath);
            if (ignoreCase)
            {
                filepath = filepath?.ToLower();
            }

            return scripts.Remove(filepath);
        }
        string CombinePath(string filepath)
        {
            if (!string.IsNullOrEmpty(rootPath))
            {
                return Path.Combine(rootPath, filepath);
            }
            return filepath;
        }
    }
}

