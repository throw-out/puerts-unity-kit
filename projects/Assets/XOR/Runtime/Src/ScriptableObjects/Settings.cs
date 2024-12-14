using System;
using System.IO;
using UnityEngine;

namespace XOR
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class Settings : ScriptableObjectBase<Settings>
    {
        static Settings() => ResourceAssetPath = "XOR/Settings";

        /// <summary>TS项目路径配置(相对路径) </summary>
        public string project = "../TsProject/tsconfig.json";
        /// <summary>是否使用ESM模块 </summary>
        public bool isESM = false;
        /// <summary>启用模块路径 </summary>
        public bool autoLoadScript = true;
        /// <summary>使用ts metadata缓存 </summary>
        public bool cached = true;

        /// <summary>Wacther类型 </summary>
        public WacthType watchType = WacthType.Nodejs;
        public enum WacthType
        {
            None,
            Csharp,
            Nodejs
        }

        public LOGGER logger = LOGGER.LOG | LOGGER.WARN | LOGGER.ERROR;
        public enum LOGGER
        {
            NONE = 0,
            INFO = 1,
            LOG = 2,
            WARN = 4,
            ERROR = 8,
        }

        /// <summary>输出更新详细的日志</summary>
        public bool verbose;
    }
}
