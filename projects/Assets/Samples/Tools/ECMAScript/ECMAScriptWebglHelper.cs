using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
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
            InjectDirectory(rootPath, manifest, isESM, isAppendExtensionName, 0);
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
        static void InjectDirectory(string dirPath, HashSet<string> manifest, bool isESM, bool isAppendExtensionName, int depth)
        {
            string prefix = depth > 0 ? GetDepthPath(depth) : string.Empty,
                suffix = !isAppendExtensionName ? string.Empty : isESM ? ".mjs" : ".cjs";

            DirectoryInfo current = new DirectoryInfo(dirPath);

            foreach (var file in current.GetFiles())
            {
                if (!targetExtensionNames.Contains(file.Extension))
                    continue;
                InjectFile(file.FullName, manifest, isESM, prefix, suffix);
            }
            foreach (var dir in current.GetDirectories())
            {
                InjectDirectory(dir.FullName, manifest, isESM, isAppendExtensionName, depth + 1);
            }
        }
        static void InjectFile(string filePath, HashSet<string> manifest, bool isESM, string prefix, string suffix)
        {
            string content = File.ReadAllText(filePath);

            foreach (var name in manifest)
            {
                if (isESM)
                {
                    content = content.Replace($"from \"{name}\"", $"from \"{prefix}puerts/modules/{name}{suffix}\"");
                }
                else
                {
                    content = content.Replace($"require(\"{name}\")", $"require(\"{prefix}puerts/modules/{name}{suffix}\")");
                }
            }
            File.WriteAllText(filePath, content);
        }

        static string GetDepthPath(int depth)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                if (i > 0) builder.Append("/");
                builder.Append("..");
            }
            builder.Append("/");
            return builder.ToString();
        }
#endif
    }
}
