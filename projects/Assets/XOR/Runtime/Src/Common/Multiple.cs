using System;
using System.Collections.Generic;

namespace XOR
{
    public abstract class Multiple<T>
        where T : Multiple<T>
    {
        //弱引用集合(不影响GC回收)
        protected static readonly List<WeakReference> referenceInstances = new List<WeakReference>();

        private WeakReference referenceSelf = null;

        public static void ReleaseAllInstances()
        {
            foreach (var weakRef in referenceInstances)
            {
                if (weakRef.IsAlive)
                {
                    var t = (weakRef.Target as T);
                    t.referenceSelf = null;
                    t.Release();
                }
            }
            referenceInstances.Clear();
        }
        public static List<T> GetAllInstances()
        {
            List<T> list = new List<T>();
            foreach (var weakRef in referenceInstances)
            {
                if (weakRef.IsAlive)
                {
                    list.Add(weakRef.Target as T);
                }
            }
            return list;
        }

        private bool __is_null_ = false;
        public virtual void Release()
        {
            this.__is_null_ = true;
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
