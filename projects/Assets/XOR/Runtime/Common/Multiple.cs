using System;
using System.Collections.Generic;

namespace XOR
{
    public abstract class Multiple<T> where T : class, new()
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
                    var t = (weak_ref.Target as Multiple<T>);
                    t.referenceSelf = null;
                    t.Release();
                }
            }
            referenceInstances.Clear();
        }
        public static List<Multiple<T>> GetAllInstance()
        {
            List<Multiple<T>> list = new List<Multiple<T>>();
            foreach (var weak_ref in referenceInstances)
            {
                if (weak_ref.IsAlive)
                {
                    list.Add(weak_ref.Target as Multiple<T>);
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
