using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XOR
{
    internal static class PathUtil
    {
        /// <summary>
        /// 转换本地路径至绝对路径
        /// </summary>
        /// <param name="localpath">基于UnityEngine.Application.dataPath的相对路径</param>
        /// <returns></returns>
        public static string GetFullPath(string localpath)
        {
            return Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, localpath));
        }
        /// <summary>
        /// 转换绝对路径至基于UnityEngine.Application.dataPath的相对路径
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static string GetLocalPath(string fullpath)
        {
            if (string.IsNullOrEmpty(fullpath))
                return null;
            string datapath = UnityEngine.Application.dataPath;
            if (!IsSameDriver(datapath, fullpath))
                return null;

            string[] fullpathArray = fullpath.Replace("\\", "/").Split('/');
            string[] datapathArray = datapath.Replace("\\", "/").Split('/');

            bool fork = false;
            List<string> buidler = new List<string>();
            for (int i = 0; i < fullpathArray.Length; i++)
            {
                if (i >= datapathArray.Length)
                {
                    buidler.Add(fullpathArray[i]);
                }
                else if (fork || !fullpathArray[i].ToLower().Equals(datapathArray[i].ToLower()))
                {
                    fork = true;
                    buidler.Insert(0, "..");
                    buidler.Add(fullpathArray[i]);
                }
            }
            for (int i = fullpathArray.Length; i < datapathArray.Length; i++)
            {
                buidler.Insert(0, "..");
            }

            return string.Join("/", buidler);
        }


        static bool IsSameDriver(string path1, string path2)
        {
            string dl1 = GetDriverLetter(path1, true),
                dl2 = GetDriverLetter(path2, true);
            if (string.IsNullOrEmpty(dl1) && !string.IsNullOrEmpty(dl2) ||
                !string.IsNullOrEmpty(dl1) && string.IsNullOrEmpty(dl2)
            )
            {
                return false;
            }
            return dl1.Equals(dl2);
        }
        static string GetDriverLetter(string path, bool caseLower = true)
        {
            path = path.Replace("\\", "/");
            int idx = path.IndexOf("/");
            if (idx >= 0)
            {
                path = path.Substring(0, idx + 1);
            }
            idx = path.IndexOf(":");
            if (idx >= 0)
            {
                return path.Substring(0, idx + 1);
            }
            return null;
        }
    }
}