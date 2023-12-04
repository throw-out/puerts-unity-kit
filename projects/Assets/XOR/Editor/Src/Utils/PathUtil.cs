using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace XOR
{
    internal static class PathUtil
    {
        /// <summary>
        /// 转换本地路径至绝对路径
        /// </summary>
        /// <param name="localPath">基于UnityEngine.Application.dataPath的相对路径</param>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public static string GetFullPath(string localPath, string rootPath = null)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = UnityEngine.Application.dataPath;
            }
            return Path.GetFullPath(Path.Combine(rootPath, localPath));
        }
        /// <summary>
        /// 转换绝对路径至基于UnityEngine.Application.dataPath的相对路径
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetLocalPath(string fullPath, string rootPath = null)
        {
            if (string.IsNullOrEmpty(fullPath))
                return null;
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = UnityEngine.Application.dataPath;
            }
            string[] fullPathArray = GetPathSplit(fullPath);
            string[] rootPathArray = GetPathSplit(rootPath);
            if (!IsSameDriver(fullPathArray, rootPathArray))
                return null;

            bool fork = false;
            List<string> paths = new List<string>();
            for (int i = 0; i < fullPathArray.Length; i++)
            {
                if (i >= rootPathArray.Length)
                {
                    paths.Add(fullPathArray[i]);
                }
                else if (fork || !fullPathArray[i].ToLower().Equals(rootPathArray[i].ToLower()))
                {
                    fork = true;
                    paths.Insert(0, "..");
                    paths.Add(fullPathArray[i]);
                }
            }
            for (int i = fullPathArray.Length; i < rootPathArray.Length; i++)
            {
                paths.Insert(0, "..");
            }
            return string.Join("/", paths);
        }
        static bool IsSameDriver(string[] pathArray1, string[] pathArray2)
        {
            if (pathArray1 == null || pathArray2 == null ||
                pathArray1.Length == 0 || pathArray2.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < pathArray1.Length; i++)
            {
                if (i >= pathArray2.Length)
                    return false;
                if (string.IsNullOrEmpty(pathArray1[i]))
                {
                    if (!string.IsNullOrEmpty(pathArray2[i]))
                        return false;
                    continue;
                }
                return pathArray1[i].Equals(pathArray2[i]);
            }
            return true;
        }
        static string[] GetPathSplit(string path)
        {
            string[] pathArray = path.Replace("\\", "/").Split('/');
            if (pathArray == null || pathArray.Length == 0)
                return null;
            //remove last empty
            if (string.IsNullOrEmpty(pathArray[pathArray.Length - 1]))
            {
                if (pathArray.Length == 1)
                    return null;
                var newPathArray = new string[pathArray.Length - 1];
                Array.Copy(pathArray, newPathArray, newPathArray.Length);
                pathArray = newPathArray;
            }
            return pathArray;
        }
    }
}