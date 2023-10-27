using System;
using System.Collections.Generic;
using System.IO;

namespace Puerts
{
    public static class ECMAScriptWebglHelper
    {
        public static void Inject(string rootPath, bool isESM, bool isAppendExtensionName = true)
        {
#if UNITY_EDITOR
            var manifest = new ECMAScriptLoader().Manifest;
            if (manifest == null || manifest.Count == 0)
                return;
            if (!Directory.Exists(rootPath))
                throw new ArgumentException("Invaild Path");

            InjectSettings settings = new InjectSettings()
            {
                manifest = manifest,
                isESM = isESM,
                prefix = string.Empty,
                suffix = !isAppendExtensionName ? string.Empty : isESM ? ".mjs" : ".cjs",
            };
            InjectDirectory(rootPath, settings);
#else
            throw new InvalidOperationException();
#endif
        }

#if UNITY_EDITOR
        static readonly HashSet<string> targetExtensionNames = new HashSet<string>(){
           ".mjs",
           ".cjs",
           ".js",
        };
        static void InjectDirectory(string dirPath, InjectSettings settings)
        {
            DirectoryInfo current = new DirectoryInfo(dirPath);

            foreach (var file in current.GetFiles())
            {
                if (!targetExtensionNames.Contains(file.Extension))
                    continue;
                string content = File.ReadAllText(file.FullName);
                if (!settings.Inject(content, out string newContent))
                    continue;
                File.WriteAllText(file.FullName, newContent);
            }
            settings.AddDepth();
            foreach (var dir in current.GetDirectories())
            {
                InjectDirectory(dir.FullName, new InjectSettings(settings));
            }
        }

        struct InjectSettings
        {
            public HashSet<string> manifest;
            public bool isESM;
            public string prefix;
            public string suffix;

            public InjectSettings(InjectSettings other)
            {
                this.manifest = other.manifest;
                this.isESM = other.isESM;
                this.prefix = other.prefix;
                this.suffix = other.suffix;
            }
            public void AddDepth()
            {
                this.prefix += "../";
            }
            public bool Inject(string content, out string newContent)
            {
                newContent = content;
                foreach (var name in manifest)
                {
                    newContent = newContent.Replace(GetReplaceOldString(name), GetReplaceNewString(name));
                }
                return content != newContent;
            }
            string GetReplaceOldString(string name)
            {
                return isESM ?
                    $"from \"{name}\"" :
                    $"require(\"{name}\")";
            }
            string GetReplaceNewString(string name)
            {
                return isESM ?
                    $"from \"{prefix}puerts/modules/{name}{suffix}\"" :
                    $"require(\"{prefix}puerts/modules/{name}{suffix}\")";
            }
        }
#endif
    }
}
