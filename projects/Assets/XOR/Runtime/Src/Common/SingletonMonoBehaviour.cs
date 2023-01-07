using UnityEngine;
namespace XOR
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour
        where T : SingletonMonoBehaviour<T>
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
                __instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (__instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    __instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }
                if (__instance != null)
                {
                    (__instance as SingletonMonoBehaviour<T>).Init();
                }
            }
            return __instance;
        }

        //回收单例对象
        public static void ReleaseInstance()
        {
            if (__instance != null)
            {
                var singleton = __instance as SingletonMonoBehaviour<T>;
                Destroy(singleton.gameObject);
                singleton.Release();
                __instance = null;
            }
        }
        
        private bool __is_null_ = false;
        public virtual void Release()
        {
            this.__is_null_ = true;
        }
        public virtual void Init()
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return this.__is_null_;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
}