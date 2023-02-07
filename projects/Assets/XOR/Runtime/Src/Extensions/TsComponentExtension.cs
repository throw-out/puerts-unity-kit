using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XOR
{
    public static class TsComponentExtension
    {
        public static Puerts.JSObject[] GetTsComponents(this GameObject gameObject)
        {
            return TsComponentLifecycle.GetComponents(gameObject)
                ?.Select(o => { o.TryInit(); return o.JSObject; })
                .ToArray();
        }
        public static Puerts.JSObject[] GetTsComponents(this Component component)
        {
            return TsComponentLifecycle.GetComponents(component)
                ?.Select(o => { o.TryInit(); return o.JSObject; })
                .ToArray();
        }
        public static Puerts.JSObject GetTsComponent(this GameObject gameObject, string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            return TsComponentLifecycle.GetComponents(gameObject)
                ?.FirstOrDefault(o =>
                {
                    if (guid.Equals(o.GetGuid()))
                    {
                        o.TryInit();
                        return true;
                    }
                    return false;
                })
                ?.JSObject;
        }
        public static Puerts.JSObject GetTsComponent(this Component component, string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            return TsComponentLifecycle.GetComponents(component)
                ?.FirstOrDefault(o =>
                {
                    if (guid.Equals(o.GetGuid()))
                    {
                        o.TryInit();
                        return true;
                    }
                    return false;
                })
                ?.JSObject;
        }
    }
}
