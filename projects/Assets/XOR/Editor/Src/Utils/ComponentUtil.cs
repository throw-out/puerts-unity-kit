using System;
using System.Reflection;

namespace XOR
{
    internal static class ComponentUtil
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        static Func<TsComponent, string> _guidGetter;
        static Func<TsComponent, string> _routeGetter;
        static Func<TsComponent, string> _versionGetter;
        static Action<TsComponent, string> _guidSetter;
        static Action<TsComponent, string> _routeSetter;
        static Action<TsComponent, string> _versionSetter;

        public static string GetGuid(TsComponent component)
        {
            if (_guidGetter == null)
            {
                _guidGetter = DelegateUtil.CreateFieldDelegate<Func<TsComponent, string>>(
                    typeof(TsComponent).GetField(InnerExplicit.GuidField, Flags)
                );
            }
            return _guidGetter(component);
        }
        public static string GetRoute(TsComponent component)
        {
            if (_routeGetter == null)
            {
                _routeGetter = DelegateUtil.CreateFieldDelegate<Func<TsComponent, string>>(
                    typeof(TsComponent).GetField(InnerExplicit.RouteField, Flags)
                );
            }
            return _routeGetter(component);
        }
        public static string GetVersion(TsComponent component)
        {
            if (_versionGetter == null)
            {
                _versionGetter = DelegateUtil.CreateFieldDelegate<Func<TsComponent, string>>(
                    typeof(TsComponent).GetField(InnerExplicit.VersionField, Flags)
                );
            }
            return _versionGetter(component);
        }
        public static void SetGuid(TsComponent component, string value)
        {
            if (_guidSetter == null)
            {
                _guidSetter = DelegateUtil.CreateFieldDelegate<Action<TsComponent, string>>(
                    typeof(TsComponent).GetField(InnerExplicit.GuidField, Flags)
                );
            }
            _guidSetter(component, value);
        }
        public static void SetRoute(TsComponent component, string value)
        {
            if (_routeSetter == null)
            {
                _routeSetter = DelegateUtil.CreateFieldDelegate<Action<TsComponent, string>>(
                    typeof(TsComponent).GetField(InnerExplicit.RouteField, Flags)
                );
            }
            _routeSetter(component, value);
        }
        public static void SetVersion(TsComponent component, string value)
        {
            if (_versionSetter == null)
            {
                _versionSetter = DelegateUtil.CreateFieldDelegate<Action<TsComponent, string>>(
                    typeof(TsComponent).GetField(InnerExplicit.VersionField, Flags)
                );
            }
            _versionSetter(component, value);
        }

        private class InnerExplicit : TsComponent
        {
            public static string GuidField => nameof(guid);
            public static string RouteField => nameof(route);
            public static string VersionField => nameof(route);
        }
    }
}