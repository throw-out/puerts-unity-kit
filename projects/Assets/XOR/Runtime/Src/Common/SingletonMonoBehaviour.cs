using System;
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
                if (__instance != null && __instance.__isDestroyed)
                {
                    __instance = null;
                }
                return __instance;
            }
        }
        public static T GetInstance()
        {
            if (__instance == null || __instance.__isDestroyed)
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
                DestroyImmediate(singleton.gameObject);
                singleton.Release();
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