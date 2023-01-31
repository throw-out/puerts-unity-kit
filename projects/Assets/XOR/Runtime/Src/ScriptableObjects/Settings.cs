using System;
using System.IO;
using UnityEngine;

namespace XOR
{
    public abstract class ScriptableObject<T> : UnityEngine.ScriptableObject
        where T : ScriptableObject<T>
    {
        /// <summary>继承类应在static构造函数中赋值 </summary>
        protected static string resourceAssetPath;

        private static T __instance__ = null;

        public static T Instance { get => __instance__; }

        /// <summary>获取实例对象 </summary>
        public static T Load() => Load(true, false);
        /// <summary>
        /// 获取实例对象
        /// </summary>
        /// <param name="useDefault"></param>
        /// <param name="createAsset"></param>
        /// <returns></returns>
        public static T Load(bool useDefault, bool createAsset)
        {
            if (__instance__ == null)
            {
                if (!IsSetupAssetPath())
                    return null;
                __instance__ = Resources.Load<T>(resourceAssetPath);
#if UNITY_EDITOR
                if (createAsset && __instance__ == null)
                {
                    __instance__ = CreateAssetPath();
                }
#endif
                if (useDefault && __instance__ == null)
                {
                    __instance__ = UnityEngine.ScriptableObject.CreateInstance<T>();
                }
            }
            return __instance__;
        }
        static bool IsSetupAssetPath()
        {
            if (string.IsNullOrEmpty(resourceAssetPath))
            {
                Debug.LogWarning($"{typeof(T).FullName} not setup {nameof(resourceAssetPath)}");
                return false;
            }
            return true;
        }
        static T CreateAssetPath()
        {
#if UNITY_EDITOR
            string localpath = $"Assets/Resources/{resourceAssetPath}.asset",
                file = $"{Path.GetDirectoryName(UnityEngine.Application.dataPath)}/{localpath}",
                dir = Path.GetDirectoryName(file);
            if (File.Exists(file))
                throw new InvalidOperationException();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            T asset = UnityEngine.ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(asset, localpath);
            UnityEditor.AssetDatabase.Refresh();
            return asset;
#else
            return null;
#endif
        }
    }
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class Settings : ScriptableObject<Settings>
    {
        static Settings() => resourceAssetPath = "XOR/Settings";

        /// <summary>TS项目路径配置(相对路径) </summary>
        public string Project = "../TsProject/tsconfig.json";
        /// <summary>TS Editor项目路径配置 </summary>
        public string EditorProject = "../TsEditorProject/tsconfig.json";
        /// <summary>是否使用ESM模块 </summary>
        public bool IsESM = false;

        public LOGGER Logger = LOGGER.LOG | LOGGER.WARN | LOGGER.ERROR;
        public enum LOGGER
        {
            NONE = 0,
            INFO = 1,
            LOG = 2,
            WARN = 4,
            ERROR = 8,
        }
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class ComponentSettings : ScriptableObject<ComponentSettings>
    {
        static ComponentSettings() => resourceAssetPath = "XOR/ComponentSettings";
    }
}
