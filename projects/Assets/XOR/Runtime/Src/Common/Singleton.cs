using System;
namespace XOR
{
    public abstract class Singleton<T>
        where T : Singleton<T>
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

        private bool __isDestroyed = false;
        public bool IsDestroyed => __isDestroyed;
        public virtual void Release()
        {
            this.__isDestroyed = true;
        }
        public virtual void Init()
        {
            if (this.__isDestroyed)
                throw new InvalidOperationException();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return this.__isDestroyed;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
}
