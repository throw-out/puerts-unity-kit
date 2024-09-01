using System;
using System.IO;
using UnityEngine;

namespace XOR
{

    public abstract class ScriptableObjectBase<T> : UnityEngine.ScriptableObject
        where T : ScriptableObjectBase<T>
    {
        protected virtual void OnCreate()
        {
        }
        /// <summary>继承类应在static构造函数中赋值 </summary>
        protected static string ResourceAssetPath { get; set; }

        private static T __instance__ = null;

        public static T Instance { get => __instance__; }

        /// <summary>获取实例对象 </summary>
        public static T GetInstance() => Load(true, false);
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
                if (!SetupAssetPath() && !useDefault)
                    return null;
                __instance__ = !string.IsNullOrEmpty(ResourceAssetPath) ? Resources.Load<T>(ResourceAssetPath) : null;
#if UNITY_EDITOR
                if (createAsset && __instance__ == null)
                {
                    __instance__ = CreateInstance();
                }
#endif
                if (useDefault && __instance__ == null)
                {
                    __instance__ = UnityEngine.ScriptableObject.CreateInstance<T>();
                }
            }
            return __instance__;
        }
        static bool SetupAssetPath()
        {
            if (ResourceAssetPath == null)
            {
                UnityEngine.ScriptableObject.CreateInstance<T>();       //invoke static constructor
                if (ResourceAssetPath == null)
                {
                    ResourceAssetPath = string.Empty;
                }
            }
            if (string.IsNullOrEmpty(ResourceAssetPath))
            {
                Debug.LogWarning($"{typeof(T).FullName} not set value{nameof(ResourceAssetPath)}");
                return false;
            }
            return true;
        }
        static T CreateInstance()
        {
#if UNITY_EDITOR
            string localpath = $"Assets/Resources/{ResourceAssetPath}.asset",
                file = $"{Path.GetDirectoryName(UnityEngine.Application.dataPath)}/{localpath}",
                dir = Path.GetDirectoryName(file);
            if (File.Exists(file))
                throw new InvalidOperationException();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            T asset = UnityEngine.ScriptableObject.CreateInstance<T>();
            asset.OnCreate();
            UnityEditor.AssetDatabase.CreateAsset(asset, localpath);
            UnityEditor.AssetDatabase.Refresh();
            return asset;
#else
            return null;
#endif
        }
    }
}