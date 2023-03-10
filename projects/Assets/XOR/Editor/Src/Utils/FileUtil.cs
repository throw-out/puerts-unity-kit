using Unity.CodeEditor;

namespace XOR
{
    internal static class FileUtil
    {
        /// <summary>
        /// 在IDE中编辑文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="line">行数</param>
        /// <param name="column">列数</param>
        /// <returns></returns>
        public static bool OpenFileInIDE(string filepath, int line = 0, int column = 0)
        {
#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
            return CodeEditor.CurrentEditor.OpenProject(filepath, line, column);
#else
            UnityEngine.Debug.LogWarning($"Unsupport unity version: {UnityEngine.Application.unityVersion}");
            return false;
#endif
        }
        /// <summary>
        /// 同步.csproj文件
        /// </summary>
        public static bool SyncIDE()
        {
#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
            Unity.CodeEditor.CodeEditor.CurrentEditor.SyncAll();
            return true;
#else
            UnityEngine.Debug.LogWarning($"Unsupport unity version: {UnityEngine.Application.unityVersion}");
            return false;
#endif
        }
    }
}