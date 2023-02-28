using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace XOR
{
    /// <summary>
    /// TsComponent实例生命周期管理
    /// </summary>
    internal static class TsComponentLifecycle
    {
        private static readonly Dictionary<int, List<WeakReference<GameObject>>> objectReferences =
            new Dictionary<int, List<WeakReference<GameObject>>>();
        private static readonly Dictionary<WeakReference<GameObject>, List<TsComponent>> componentReferences =
            new Dictionary<WeakReference<GameObject>, List<TsComponent>>();
        private static readonly HashSet<TsComponent> pending = new HashSet<TsComponent>();

        private static Runtime runtime;

        public static void Register(Puerts.JsEnv env)
        {
            runtime = new Runtime(env);

            if (pending.Count > 0)
            {
                var components = pending.ToArray();
                pending.Clear();
                CreateJSObject(components);
            }
        }
        public static void Unregister()
        {
            runtime = null;
        }

        public static WeakReference<GameObject> GetReference(GameObject gameObject)
        {
            return GetReference(gameObject, true);
        }
        public static void AddComponent(WeakReference<GameObject> reference, TsComponent component)
        {
            if (reference == null || IsNullPointer(component))
                return;
            if (!componentReferences.TryGetValue(reference, out var components))
            {
                components = new List<TsComponent>();
                componentReferences.Add(reference, components);
            }
            components.Add(component);
            if (!component.Registered)
            {
                if (runtime != null && runtime.IsAlive)
                {
                    CreateJSObject(component);
                }
                else
                {
                    pending.Add(component);
                }
            }
        }
        public static void DestroyComponent(WeakReference<GameObject> reference, TsComponent component)
        {
            if (reference == null || IsNullPointer(component))
                return;
            pending.Remove(component);
            if (componentReferences.TryGetValue(reference, out var components))
            {
                for (int i = components.Count - 1; i >= 0; i--)
                {
                    if (component.Equals(components[i]))
                    {
                        components.RemoveAt(i);
                    }
                }
            }
            if (components != null && components.Count == 0)
            {
                componentReferences.Remove(reference);
            }
        }
        public static TsComponent[] GetComponents(Component component)
        {
            return GetComponents(GetReference(component.gameObject, false));
        }
        public static TsComponent[] GetComponents(GameObject gameObject)
        {
            return GetComponents(GetReference(gameObject, false));
        }
        public static TsComponent[] GetComponents(WeakReference<GameObject> reference)
        {
            if (reference == null)
                return null;
            if (componentReferences.TryGetValue(reference, out var components))
            {
                return components.ToArray();
            }
            return null;
        }
        public static void DestroyComponents(WeakReference<GameObject> reference)
        {
            if (reference == null)
                return;
            if (componentReferences.TryGetValue(reference, out var components))
            {
                componentReferences.Remove(reference);
                for (int i = components.Count - 1; i >= 0; i--)
                {
                    components[i].Release();
                    pending.Remove(components[i]);
                }
            }
        }
        public static void GC(bool destroyed = true)
        {
            WeakReference<GameObject> reference = null;

            int[] hashCodes = objectReferences.Keys.ToArray();
            for (int i = 0; i < hashCodes.Length; i++)
            {
                objectReferences.TryGetValue(hashCodes[i], out var list);
                for (int j = list.Count - 1; j >= 0; j--)
                {
                    reference = list[j];
                    if (!reference.TryGetTarget(out var current) || destroyed && current == null)
                    {
                        DestroyComponents(reference);
                        list.RemoveAt(j);
                    }
                    else if (destroyed && componentReferences.TryGetValue(reference, out var components))
                    {
                        for (int k = components.Count - 1; k >= 0; k--)
                        {
                            if (components[k] == null)
                            {
                                components[k].Release();
                            }
                        }
                    }
                }
                if (list == null || list.Count == 0)
                {
                    objectReferences.Remove(hashCodes[i]);
                }
            }
        }
        public static void PrintStatus()
        {
            StringBuilder builder = new StringBuilder();

            WeakReference<GameObject> reference = null;
            int[] hashCodes = objectReferences.Keys.ToArray();
            for (int i = 0; i < hashCodes.Length; i++)
            {
                objectReferences.TryGetValue(hashCodes[i], out var list);
                for (int j = 0; j < list.Count; j++)
                {
                    builder.AppendLine();
                    reference = list[j];
                    if (reference.TryGetTarget(out var current) && componentReferences.TryGetValue(reference, out var components))
                    {
                        builder.Append($"-GameObject({current.GetHashCode()}): \t{current.name}");
                        for (int k = 0; k < components.Count; k++)
                        {
                            builder.AppendLine();
                            builder.Append("    -");
                            builder.Append(components[k] == null ? "null" : nameof(TsComponent));
                            builder.Append($"({components[k].GetHashCode()}): \t{components[k].GetGuid()}");
                        }
                    }
                    else
                    {
                        builder.Append($"-UNKNWON({current.GetHashCode()}): \t{current.name}");
                    }
                }
            }
            string result = builder.ToString().TrimStart();
            UnityEngine.Debug.Log(!string.IsNullOrEmpty(result) ? result : "Empty");
        }

        static bool IsNullPointer(object obj)
        {
            return (obj?.GetHashCode() ?? 0) == 0;
        }
        static WeakReference<GameObject> GetReference(GameObject gameObject, bool create)
        {
            if (IsNullPointer(gameObject))
                return null;
            int hashCode = gameObject.GetHashCode();
            WeakReference<GameObject> reference = null;
            if (objectReferences.TryGetValue(hashCode, out var list))
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    reference = list[i];
                    if (!reference.TryGetTarget(out var curent))
                    {
                        DestroyComponents(reference);
                        list.RemoveAt(i);
                        continue;
                    }
                    if (gameObject.Equals(curent))
                    {
                        return reference;
                    }
                }
            }
            if (create)
            {
                reference = new WeakReference<GameObject>(gameObject);
                if (list == null)
                {
                    list = new List<WeakReference<GameObject>>();
                    objectReferences.Add(hashCode, list);
                }
                list.Add(reference);
            }
            return reference;
        }
        static void CreateJSObject(params TsComponent[] components)
        {
            foreach (var component in components)
            {
                if (component == null)
                    continue;
                if (string.IsNullOrEmpty(component.GetGuid()))
                    continue;
                component.JSObject = runtime.Create(component);
                if (component.JSObject == null)
                {
                    Logger.LogWarning($"{component.name} {nameof(XOR.TsComponent)} JSObject create fail: {component.GetGuid()}");
                }
            }
        }

        private class Runtime
        {
            private readonly Puerts.JsEnv env;
            private readonly HashSet<string> resolvePaths;
            private Func<TsComponent, string, Puerts.JSObject> create;
            public bool IsAlive => true;
            public Runtime(Puerts.JsEnv env)
            {
                this.env = env;
                this.resolvePaths = new HashSet<string>();
                this.env.UsingFunc<TsComponent, string, Puerts.JSObject>();
            }
            /// <summary>
            /// 创建JSObject对象
            /// </summary>
            /// <param name="component"></param>
            /// <returns></returns>
            public Puerts.JSObject Create(TsComponent component)
            {
                if (create == null)
                {
                    create = this.env.ComponentJSObjectCreator();
                    if (create == null)
                    {
                        Logger.LogWarning($"XOR Modules Unregisted.");
                        return null;
                    }
                }
                Puerts.JSObject result = create.Invoke(component, component.GetGuid());

                //execute module, after retry create js object
                if (result == null && !resolvePaths.Contains(component.GetPath()))
                {
                    resolvePaths.Add(component.GetPath());
                    if (!string.IsNullOrEmpty(component.GetPath()))
                    {
                        env.Load(component.GetPath());
                        result = create.Invoke(component, component.GetGuid());
                    }
                }
                return result;
            }
        }
    }
}
