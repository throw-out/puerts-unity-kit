using System;
namespace XOR
{
    public abstract class Singleton<T> where T : class, new()
    {
        protected static T __instance = null;
        public static T Instance
        {
            get
            {
                return __instance;
            }
        }
        public static T GetInstance()
        {
            if (__instance == null)
            {
                __instance = Activator.CreateInstance<T>();
                if (__instance != null)
                {
                    (__instance as Singleton<T>).Init();
                }
            }
            return __instance;
        }
        //回收单例对象
        public static void ReleaseInstance()
        {
            if (__instance != null)
            {
                (__instance as Singleton<T>).Release();
                __instance = null;
            }
        }
        public virtual void Release()
        {
        }
        public virtual void Init()
        {
        }
    }
}
