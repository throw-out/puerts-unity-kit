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
        /// 转换绝对路径至相对与UnityEngine.Application.dataPath的路径
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static string GetLocalPath(string fullpath)
        {
            fullpath = fullpath.Replace("\\", "/");
            string similar = UnityEngine.Application.dataPath.Replace("\\", "/");
            int depth = 0;
            while (similar.Length > 0)
            {
                if (fullpath.StartsWith(similar))
                    break;
                depth++;
                similar = Path.GetDirectoryName(similar);
            }
            if (similar.Length == 0)
                return null;

            StringBuilder builder = new StringBuilder();
            if (depth > 0)
            {
                for (int i = 0; i < depth; i++)
                {
                    builder.Append("../");
                }
            }
            else
            {
                builder.Append("./");
            }
            string localpath = fullpath.Substring(similar.Length + 1);
            builder.Append(localpath);

            return builder.ToString();
        }
    }
}