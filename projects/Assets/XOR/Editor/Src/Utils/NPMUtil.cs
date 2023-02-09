using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XOR
{
    internal static class NPMUtil
    {
        /// <summary>
        /// 检测依赖安装情况
        /// </summary>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        public static string[] CheckDependentsInstall(string packagePath)
        {
            var content = File.ReadAllText(packagePath);
            var package = Unity.Plastic.Newtonsoft.Json.JsonConvert.DeserializeObject<Package>(content);
            if (package == null)
                return null;

            var modules = new List<string>();
            if (package.dependencies != null)
                modules.AddRange(package.dependencies.Keys);
            if (package.devDependencies != null)
                modules.AddRange(package.dependencies.Keys);

            var dirpath = Path.GetDirectoryName(packagePath);
            for (int i = modules.Count - 1; i >= 0; i--)
            {
                if (Directory.Exists(Path.Combine(dirpath, modules[i])))
                {
                    modules.RemoveAt(i);
                }
            }
            return modules.ToArray();
        }
        /// <summary>
        /// 检测脚本编译输出路径
        /// </summary>
        /// <returns></returns>
        public static bool CheckCompileOutput(string tsconfigPath)
        {
            return true;
        }
        
        [System.Serializable]
        private class Package
        {
            public Dictionary<string, string> dependencies;
            public Dictionary<string, string> devDependencies;
        }
        [System.Serializable]
        private class Tsconfig
        {
            public Dictionary<string, string> compilerOptions;
        }
    }
}
