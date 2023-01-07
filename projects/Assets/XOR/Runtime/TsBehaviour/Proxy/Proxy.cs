using System;
using UnityEngine;

namespace XOR
{
    public abstract class Proxy : MonoBehaviour, IDisposable
    {
        public Proxy()
        {
            this.Init();
        }
        ~Proxy()
        {
            Release();
        }

        public virtual void Init()
        {
        }
        public virtual void Release()
        {
        }
        public void Dispose()
        {
            Release();
        }
    }
    public abstract class ProxyAction : Proxy
    {
        public virtual Action callback { get; set; }

        public override void Release()
        {
            base.Release();
            callback = null;
        }
    }
    public abstract class ProxyAction<T> : Proxy
    {
        public virtual Action<T> callback { get; set; }

        public override void Release()
        {
            base.Release();
            callback = null;
        }
    }
}

