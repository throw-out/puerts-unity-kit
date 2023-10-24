using System.IO;
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
            bool successfully = CodeEditor.CurrentEditor.OpenProject(filepath, line, column);
            if (successfully)
                return true;

            //尝试查找编辑器安装路径
            string editorPath = CodeEditor.CurrentEditorInstallation,
                editorName = string.IsNullOrEmpty(editorPath) ? string.Empty : Path.GetFileNameWithoutExtension(editorPath);
            if (string.IsNullOrEmpty(editorName))
                return false;

            string arguments = null;
            switch (editorName)
            {
                default:
                case "Code":        //vscode编辑器
                    arguments = CodeEditor.ParseArgument(
                        "$(ProjectPath) -g $(File):$(Line):$(Column)",
                        filepath,
                        line,
                        column
                    );
                    break;
            }
            if (string.IsNullOrEmpty(arguments))
                return false;

#if UNITY_EDITOR_WIN
            arguments = arguments.Replace("\\", "/");
#endif
            return CodeEditor.OSOpenFile(editorPath, arguments);
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