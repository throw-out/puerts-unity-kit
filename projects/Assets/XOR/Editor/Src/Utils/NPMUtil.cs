using System.Collections;
using System.Collections.Generic;
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
            return null;
        }
        /// <summary>
        /// 检测脚本编译输出路径
        /// </summary>
        /// <returns></returns>
        public static bool CheckCompileOutput(string tsconfigPath)
        {
            return true;
        }
    }
}
