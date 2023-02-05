using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XOR
{
    public static class TsComponentExtension
    {
        public static TsComponent[] GetTsComponents(this GameObject gameObject)
        {
            return TsComponentLifecycle.GetComponents(gameObject);
        }
        public static TsComponent[] GetTsComponents(this Component gameObject)
        {
            return TsComponentLifecycle.GetComponents(gameObject);
        }
        public static TsComponent GetTsComponent(this GameObject gameObject, string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            return GetTsComponents(gameObject)?.FirstOrDefault(component => guid.Equals(component.GetGuid()));
        }
        public static TsComponent GetTsComponent(this Component gameObject, string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            return GetTsComponents(gameObject)?.FirstOrDefault(component => guid.Equals(component.GetGuid()));
        }
    }
}
