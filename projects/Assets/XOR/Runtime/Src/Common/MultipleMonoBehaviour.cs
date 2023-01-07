using System;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{
    public abstract class MultipleMonoBehaviour<T> : MonoBehaviour where T : MultipleMonoBehaviour<T>
    {
        //弱引用集合(不影响GC回收)
        protected static readonly List<WeakReference> referenceInstances = new List<WeakReference>();

        private WeakReference referenceSelf = null;

        public static void ReleaseAllInstance()
        {
            foreach (var weak_ref in referenceInstances)
            {
                if (weak_ref.IsAlive)
                {
                    var t = (weak_ref.Target as MultipleMonoBehaviour<T>);
                    t.referenceSelf = null;
                    t.Release();
                    DestroyImmediate(t.gameObject); //立即删除实例对象(不触发OnDestroy-Function)
                }
            }
            referenceInstances.Clear();
        }
        public static List<MultipleMonoBehaviour<T>> GetAllInstance()
        {
            List<MultipleMonoBehaviour<T>> list = new List<MultipleMonoBehaviour<T>>();
            foreach (var weak_ref in referenceInstances)
            {
                if (weak_ref.IsAlive)
                {
                    list.Add(weak_ref.Target as MultipleMonoBehaviour<T>);
                }
            }
            return list;
        }
        public virtual void Release()
        {
            if (referenceSelf != null)
            {
                referenceInstances.Remove(referenceSelf);
                referenceSelf = null;
            }
        }
        public virtual void Init()
        {
            if (referenceSelf == null)
            {
                referenceSelf = new WeakReference(this);
                referenceInstances.Add(referenceSelf);
            }
        }

    }
}
