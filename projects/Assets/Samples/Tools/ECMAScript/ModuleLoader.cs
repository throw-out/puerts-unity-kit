using System;
using System.Collections.Generic;
using Puerts;

namespace Puerts
{
    public class ModuleLoader : ILoader, IModuleChecker
    {
        private ILoader loader;
        private Func<string, bool> match;
        private Func<string, string> resolve;
        public ModuleLoader(ILoader loader, Func<string, string> resolve) : this(loader, null, resolve)
        {
        }
        public ModuleLoader(ILoader loader, Func<string, bool> match) : this(loader, match, null)
        {
        }
        public ModuleLoader(ILoader loader, Func<string, bool> match, Func<string, string> resolve)
        {
            this.loader = loader;
            this.match = match;
            this.resolve = resolve;
        }
        public bool FileExists(string filepath)
        {
            if (match != null && !match(filepath))
            {
                return false;
            }
            return loader.FileExists(ResolvePath(filepath));
        }
        public string ReadFile(string filepath, out string debugpath)
        {
            if (match != null && !match(filepath))
            {
                debugpath = filepath;
                return null;
            }
            return loader.ReadFile(ResolvePath(filepath), out debugpath);
        }
        public bool IsESM(string filepath)
        {
            if (loader is IModuleChecker)
            {
                return ((IModuleChecker)loader).IsESM(ResolvePath(filepath));
            }
            return false;
        }

        string ResolvePath(string filepath)
        {
            if (resolve != null)
            {
                return resolve(filepath);
            }
            return filepath;
        }

        private static readonly string CSharpCriticalName = "csharp.";
        private static readonly HashSet<string> CriticalModuleNames = new HashSet<string>(new string[]{
            "csharp",
            "puerts",
        });
        static bool MatchModule(string filepath)
        {
            return CriticalModuleNames.Contains(filepath) || filepath != null && filepath.StartsWith(CSharpCriticalName);
        }
        static string ResolveESM(string filepath)
        {
            return $"puerts/modules/{filepath}.mjs";
        }
        static string ResolveESCommonjs(string filepath)
        {
            return $"puerts/modules/{filepath}.cjs";
        }
        public static ILoader ESM()
        {
            return new ModuleLoader(new Puerts.DefaultLoader(), MatchModule, ResolveESM);
        }
        public static ILoader Commonjs()
        {
            return new ModuleLoader(new Puerts.DefaultLoader(), MatchModule, ResolveESCommonjs);
        }
    }
}