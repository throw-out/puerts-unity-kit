using System.Collections.Generic;
using UnityEngine;

namespace Puerts
{
    public class ECMAScriptLoader : ILoader, IModuleChecker
    {
        private readonly ILoader other;
        private readonly ILoader inner;
        private bool isESM;

        public HashSet<string> Manifest { get; private set; }

        public ECMAScriptLoader() : this(null, new Puerts.DefaultLoader()) { }
        public ECMAScriptLoader(ILoader other) : this(other, new Puerts.DefaultLoader()) { }
        public ECMAScriptLoader(ILoader other, ILoader inner)
        {
            this.other = other;
            this.inner = inner;
            this.Init();
        }
        public bool FileExists(string filepath)
        {
            if (Manifest.Contains(filepath))
            {
                return this.inner.FileExists(ResolveModulePath(filepath, true));
            }
            if (other != null)
            {
                return other.FileExists(filepath);
            }
            return false;
        }
        public string ReadFile(string filepath, out string debugpath)
        {
            if (Manifest.Contains(filepath))
            {
                return this.inner.ReadFile(ResolveModulePath(filepath, true), out debugpath);
            }
            if (other != null)
            {
                return other.ReadFile(filepath, out debugpath);
            }
            debugpath = null;
            return null;
        }
        public bool IsESM(string filepath)
        {
            if (Manifest.Contains(filepath))
            {
                return this.isESM;
            }
            if (other != null && other is IModuleChecker checker)
            {
                return checker.IsESM(filepath);
            }
            return false;
        }

        void Init()
        {
            if (Manifest != null)
                return;
            Manifest = new HashSet<string>();

            var asset = Resources.Load<TextAsset>(ResolveManifestPath(false));
            if (asset == null || string.IsNullOrEmpty(asset.text))
                return;

            foreach (string line in asset.text.Replace("\r\n", "\n").Split('\n'))
            {
                if (line.StartsWith("isESM:"))
                {
                    this.isESM = "True".Equals(line.Substring(6));
                }
                else
                {
                    Manifest.Add(line);
                }
            }
        }
        string ResolveModulePath(string filepath, bool extensionName)
        {
            string path = $"puerts/modules/{filepath}";
            if (extensionName)
            {
                path += (this.isESM ? ".mjs" : ".cjs");
            }
            return path;
        }
        string ResolveManifestPath(bool extensionName)
        {
            string path = "puerts/modules/manifest";
            if (extensionName)
            {
                path += ".txt";
            }
            return path;
        }
    }
}