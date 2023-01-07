using System;
using System.Collections.Generic;
using System.IO;
using Puerts;

namespace XOR
{
    public class PackageLoader : ILoader, IDisposable
    {
        private List<Package> packages;
        public PackageLoader()
        {
            this.packages = new List<Package>();
        }
        public void Dispose()
        {
            foreach (var package in packages)
            {
                package.Dispose();
            }
            packages.Clear();
        }
        public bool FileExists(string filepath)
        {
            //Debug.Log("require: " + filepath);
#if !UNITY_EDITOR
        filepath = filepath?.ToLower();
#endif
            foreach (var package in packages)
            {
                if (package.loader.FileExists(filepath))
                    return true;
            }
            return false;
        }
        public string ReadFile(string filepath, out string debugpath)
        {
#if !UNITY_EDITOR
        filepath = filepath?.ToLower();
#endif
            foreach (var package in packages)
            {
                string script =
                    package.loader.ReadFile(filepath, out debugpath);
                if (!string.IsNullOrEmpty(script))
                    return script; ;
            }
            debugpath = filepath;
            return null;
        }

        public void AddLoader(ILoader loader, int index = 0)
        {
            packages.Add(new Package()
            {
                loader = loader,
                index = index
            });
            packages.Sort((v1, v2) =>
            {
                return v1.index > v2.index ? 1 : v1.index < v2.index ? -1 : 0;
            });
        }
        public bool RemoveLoader(ILoader loader)
        {
            for (int i = packages.Count - 1; i >= 0; i--)
            {
                if (packages[i].loader == loader)
                {
                    packages.RemoveAt(i);
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
            var result = new List<ILoader>();
            foreach (var package in this.packages)
            {
                if (type.IsAssignableFrom(package.loader.GetType()))
                {
                    result.Add(package.loader);
                }
            }
            return result.ToArray();
        }
        internal class Package
        {
            public ILoader loader;
            public int index;
            public void Dispose()
            {
                if (typeof(IDisposable).IsAssignableFrom(loader.GetType()))
                    ((IDisposable)loader).Dispose();
            }
        }
    }

    public class FileLoader : ILoader
    {
        private readonly string outputRoot;
        private readonly string projectRoot;
        public FileLoader(string outputRoot, string projectRoot = null)
        {
            this.outputRoot = outputRoot;
            this.projectRoot = string.IsNullOrEmpty(projectRoot) ? outputRoot : projectRoot;
        }
        public bool FileExists(string filepath)
        {
            //Debug.Log("require: " + filepath);
            return File.Exists(Combine(filepath));
        }

        public string ReadFile(string filepath, out string debugpath)
        {
            var path = Combine(filepath);
            if (File.Exists(path))
            {
                debugpath = path;
                //Debug.Log("file: " + path);
                return File.ReadAllText(path);
            }
            debugpath = filepath;
            return null;
        }
        string Combine(string filepath)
        {
#if UNITY_EDITOR
            var path = filepath;
            if (!path.EndsWith(".js") && !path.EndsWith(".cjs") && !path.EndsWith(".mjs") && !path.EndsWith(".json"))
                UnityEngine.Debug.LogWarning("unknown file extension: " + filepath);

            if (path.StartsWith("node_modules/"))
                path = Path.Combine(projectRoot, path);
            else
                path = Path.Combine(outputRoot, path);
            if (File.Exists(path))
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
    }

    public class BufferLoader : ILoader, IDisposable
    {
        public string rootPath { get; set; }
        //缓存池
        private Dictionary<string, string> scripts;
        public BufferLoader()
        {
            this.scripts = new Dictionary<string, string>();
        }
        public void Dispose()
        {
            this.scripts?.Clear();
        }

        public bool FileExists(string filepath)
        {
            filepath = Combine(filepath)?.ToLower();

            string script = null;
            scripts.TryGetValue(filepath, out script);
            return !string.IsNullOrEmpty(script);
        }
        public string ReadFile(string filepath, out string debugpath)
        {
            debugpath = Combine(filepath);
            filepath = debugpath?.ToLower();

            string script = null;
            scripts.TryGetValue(filepath, out script);
            if (!string.IsNullOrEmpty(script))
                return script;

            return null;
        }
        public void AddScript(string filepath, string script)
        {
            filepath = Combine(filepath)?.ToLower();

            if (scripts.ContainsKey(filepath))
            {
                scripts.Remove(filepath);
            }
            scripts.Add(filepath, script);
        }
        public bool RemoveScript(string filepath)
        {
            filepath = Combine(filepath)?.ToLower();

            return scripts.Remove(filepath);
        }
        string Combine(string filepath)
        {
            if (!string.IsNullOrEmpty(rootPath))
            {
                return Path.Combine(rootPath, filepath);
            }
            return filepath;
        }
    }
}

