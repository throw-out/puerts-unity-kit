using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XOR.Behaviour.Args;

namespace XOR
{
    [CustomEditor(typeof(BehaviourSettings))]
    //[CanEditMultipleObjects]
    internal class BehaviourSettingsEditor : Editor
    {
        private const float Space = 5f;
        private const float TypeWidth = 80f;
        private const float ClassWidth = 50f;
        private const float ButtonWidth = 20f;
        private const int ClassWarning = 32;
        private static Dictionary<Type, bool> foldoutDict;

        public override void OnInspectorGUI()
        {
            var root = BehaviourSettings.Load(true, true);
            if (root == null)
                return;
            if (root.categories == null)
            {
                root.categories = new List<BehaviourSettings.Category>();
                EditorUtility.SetDirty(root);
            }
            if (root.categories.Count == 0)
            {
                GUILayout.Label("Empty Configure");
            }
            else
            {
                RenderCategories(root);
            }
            GUILayout.Space(20f);
            RenderMenu(root);

            AssetDatabase.SaveAssets();
        }

        void RenderCategories(BehaviourSettings root)
        {
            foreach (var category in root.categories)
            {
                if (category.Type == null)
                {
                    continue;
                }
                GUILayout.Space(Space);
                if (GUIUtil.RenderHeader(Helper.GetTitle(category.Type)))
                {
                    SetFoldout(category.Type, !GetFoldout(category.Type, true));
                }
                if (!GetFoldout(category.Type, true))
                    continue;
                GUIUtil.RenderGroup(RenderCategory, root, category);
            }
        }
        void RenderCategory(BehaviourSettings root, BehaviourSettings.Category category)
        {
            var groups = category.groups;
            if (groups == null)
            {
                groups = new List<BehaviourSettings.Group>();
                category.groups = groups;
            }

            BehaviourSettings.Group removeGroup = null;

            int total = 0;
            RenderCategoryTitle();
            foreach (var group in groups)
            {
                var remove = RenderGroup(root, category, group, ref total);
                if (remove)
                {
                    removeGroup = group;
                }
            }
            RenderCategoryMenu(root, category, total);

            if (removeGroup != null)
            {
                category.groups.Remove(removeGroup);
                EditorUtility.SetDirty(root);
            }
        }
        void RenderCategoryTitle()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(ButtonWidth);
            GUILayout.Label(Language.Behaviour.Get("behaviour_arg_type"), GUILayout.Width(TypeWidth));
            GUILayout.Label(Language.Behaviour.Get("behaviour_arg_value"), GUILayout.ExpandWidth(true));
            GUILayout.Label(Language.Behaviour.Get("behaviour_arg_param"), Skin.labelCenter, GUILayout.Width(ClassWidth));
            GUILayout.Label(Language.Behaviour.Get("behaviour_arg_class"), Skin.labelCenter, GUILayout.Width(ClassWidth));
            GUILayout.EndHorizontal();
        }
        void RenderCategoryMenu(BehaviourSettings root, BehaviourSettings.Category category, int total)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(ButtonWidth)))
            {
                category.groups.Add(new BehaviourSettings.Group()
                {
                    type = BehaviourSettings.GroupType.Single
                });
                EditorUtility.SetDirty(root);
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{total}", Skin.labelCenter, GUILayout.Width(ClassWidth));

            GUILayout.EndHorizontal();
        }
        bool RenderGroup(BehaviourSettings root, BehaviourSettings.Category category, BehaviourSettings.Group group, ref int total)
        {
            bool ok = false;

            GUILayout.BeginHorizontal();
            //delete
            if (GUILayout.Button("-", GUILayout.Width(ButtonWidth)))
            {
                ok = true;
            }
            //type
            var newType = (BehaviourSettings.GroupType)EditorGUILayout.EnumPopup(group.type, GUILayout.Width(TypeWidth));
            if (newType != group.type)
            {
                group.type = newType;
                EditorUtility.SetDirty(root);
            }
            //value
            var newValue = Convert.ToUInt32(EditorGUILayout.EnumFlagsField((Enum)Enum.ToObject(category.Type, group.value)));
            if (newValue != group.value)
            {
                group.value = newValue;
                EditorUtility.SetDirty(root);
            }
            //param
            if (group.type == BehaviourSettings.GroupType.Combine)
            {
                var newParam = Math.Min(
                    (uint)EditorGUILayout.IntField((int)group.param, GUILayout.Width(ClassWidth)),
                    (uint)BehaviourSettings.Helper.GetEnumCountByFlags(category.Type, group.value)
                );
                if (newParam != group.param)
                {
                    group.param = newParam;
                    EditorUtility.SetDirty(root);
                }
            }
            else
            {
                GUILayout.Space(ClassWidth);
            }
            //count
            var count = category.GetCombineCount(group);
            GUILayout.Label($"{count}", count < ClassWarning ? Skin.labelCenter : Skin.labelYellow, GUILayout.Width(ClassWidth));
            GUILayout.EndHorizontal();

            total += count;

            return ok;
        }
        void RenderMenu(BehaviourSettings root)
        {
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_add")))
            {
                if (root.categories == null)
                {
                    root.categories = new List<BehaviourSettings.Category>();
                }
                Helper.PopupAddCategory(root);
            }
            GUILayout.Space(10f);
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_preference")))
            {
                if (root.categories != null && root.categories.Any(c => c.groups != null && c.groups.Count > 0))
                {
                    GUIUtil.RenderConfirm("override_current_data", root.SetPreference);
                }
                else
                {
                    root.SetPreference();
                    EditorUtility.SetDirty(root);
                }
            }
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_default")))
            {
                if (root.categories != null && root.categories.Any(c => c.groups != null && c.groups.Count > 0))
                {
                    GUIUtil.RenderConfirm("override_current_data", root.SetDefault);
                }
                else
                {
                    root.SetDefault();
                    EditorUtility.SetDirty(root);
                }
            }
            GUILayout.Space(10f);
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_generate")))
            {
                Helper.GenerateCode(root);
            }
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_generate_clear")))
            {
                Helper.ClearGenerateCode();
            }
        }

        static bool GetFoldout(Type type, bool defaultValue = false)
        {
            if (foldoutDict == null || !foldoutDict.TryGetValue(type, out var foldout))
                return defaultValue;
            return foldout;
        }
        static void SetFoldout(Type type, bool value)
        {
            if (foldoutDict == null)
            {
                foldoutDict = new Dictionary<Type, bool>();
            }
            foldoutDict[type] = value;
        }
    }

    public static class Helper
    {
        static void Combine<T>(List<T> source, int num, int index, List<T> temp, List<List<T>> results)
        {
            if (temp == null)
                return;
            if (num <= 0)
            {
                results.Add(temp);
                return;
            }

            for (int i = index; i < source.Count; i++)
            {
                var newTemp = new List<T>(temp);
                newTemp.Add(source[i]);
                Combine(source, num - 1, i + 1, newTemp, results);
            }
        }

        public static void PopupAddCategory(BehaviourSettings settings)
        {
            if (settings.categories == null)
                return;
            var @namespace = nameof(XOR.Behaviour.Args);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(t => t.FullName != null && t.FullName.StartsWith(@namespace) && t.IsEnum)
                .OrderBy(t => t.FullName)
                .ToList();
            var options = types.Select(t => t.Name).ToArray();
            var disabled = settings.categories?.Select(o => o.Type != null ? types.IndexOf(o.Type) : -1)
                .Where(index => index >= 0)
                .ToArray();
            XOR.Serializables.TsProperties.Utility.CustomMenu(options, null, disabled, disabled, (index) =>
            {
                var newCategory = new BehaviourSettings.Category();
                newCategory.Type = types[index];
                settings.categories.Add(newCategory);
                EditorUtility.SetDirty(settings);
            });
        }

        public static void GenerateCode(BehaviourSettings settings)
        {
            var exportEnums = settings.GetExportEnums();
            if (exportEnums == null || exportEnums.Count <= 0)
            {
                GUIUtil.RenderGenerateClassEmpty();
                return;
            }
            //过滤Default配置类型
            Behaviour.Factory.Clear();
            Behaviour.Default.Register();
            var generteEnums = exportEnums
                .Where(e => !Behaviour.Factory.Contains(e))
                .ToList();
            //弹窗询问生成
            GUIUtil.RenderGenerateClass(() =>
            {
                ClearGenerateCode();
                GenerateCode(generteEnums);
            }, exportEnums.Count, generteEnums.Count);
        }
        public static void ClearGenerateCode()
        {
            var saveTo = Puerts.Configure.GetCodeOutputDirectory() + "BehaviourInvokerStaticWrap.cs";
            if (File.Exists(saveTo))
            {
                File.Delete(saveTo);
                AssetDatabase.Refresh();
            }
        }
        public static void GenerateCode(IEnumerable<Enum> exportEnums)
        {
            var saveTo = Puerts.Configure.GetCodeOutputDirectory() + "BehaviourInvokerStaticWrap.cs";

            List<Tuple<string, string>> classes = new List<Tuple<string, string>>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($@"
namespace XOR.Behaviour
{{
    public static class BehaviourInvokerStaticWrap
    {{
");
            foreach (var @enum in exportEnums)
            {
                var type = @enum.GetType();
                var invokerImpl = GetInvokerImplement(type);
                if (invokerImpl == null)
                {
                    Debug.LogError($"No Invoker implementation class found: {type.FullName}");
                    continue;
                }
                var dv = Convert.ToUInt32(@enum);
                var defines = BehaviourSettings.Helper.GetEnumValues(type);
                if (defines == null || defines.Length == 0)
                    continue;

                var methodNames = defines
                    .Where(v => (dv & v) == v)
                    .Select(v => Enum.GetName(type, v))
                    .ToArray();
                if (methodNames == null || methodNames.Length == 0)
                    continue;
                var className = methodNames.Length == 1 ?
                    $"{methodNames[0]}Behaviour" :
                    $"{type.Name}Behaviour{dv}";

                classes.Add(new Tuple<string, string>(className, string.Join(" | ", methodNames.Select(mn => $"{type.FullName}.{mn}"))));
                GetArgsDeclaration(type, out string argsDeclaration, out string args);
                if (!string.IsNullOrEmpty(args))
                {
                    args = ", " + args;
                }
                sb.AppendLine($@"
        protected class {className} : {invokerImpl.FullName.Replace("+", ".")}
        {{
            {string.Join("", methodNames.Select(methodName => $@"
            private void {methodName}({argsDeclaration})
            {{
                Invoke(XOR.Behaviour.Args.{type.Name}.{methodName}{args});
            }}"))}
        }}");
            }
            sb.AppendLine($@"
        public static void Register()
        {{
            {string.Join("", classes.Select(c => $@"
            XOR.Behaviour.Factory.Register<{c.Item1}>({c.Item2});"))}
        }}
    }}
}}");

            File.WriteAllText(saveTo, sb.ToString());
            AssetDatabase.Refresh();
        }

        public static string GetTitle(Type type)
        {
            if (type == null)
                return string.Empty;
            var attribute = type.GetCustomAttribute<XOR.Behaviour.TitleAttribute>();
            if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
                return attribute.Name;
            return type.Name;
        }
        static bool GetArgsDeclaration(Type type, out string argsDeclaration, out string args)
        {
            argsDeclaration = string.Empty;
            args = string.Empty;
            if (type == null)
                return false;
            var attribute = type.GetCustomAttribute<XOR.Behaviour.ArgsAttribute>();
            if (attribute != null && attribute.Args != null)
            {
                argsDeclaration = string.Join(", ", attribute.Args.Select((t, index) => t.FullName + $" arg{index}"));
                args = string.Join(", ", attribute.Args.Select((t, index) => $"arg{index}"));
                return true;
            }
            return false;
        }
        static Dictionary<Type, Type> invokerImplements;
        static Type GetInvokerImplement(Type type)
        {
            if (invokerImplements == null)
            {
                invokerImplements = new Dictionary<Type, Type>();
            }
            if (invokerImplements.TryGetValue(type, out var impl))
            {
                return impl;
            }

            impl = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(t => t.IsClass && !t.IsGenericType && !t.IsSealed)
                .Where(t => typeof(Behaviour.Behaviour).IsAssignableFrom(t) && t.IsDefined(typeof(XOR.Behaviour.ArgsAttribute)))
                .OrderBy(t => t.FullName)
                .FirstOrDefault(t =>
                {
                    var attribute = t.GetCustomAttribute<XOR.Behaviour.ArgsAttribute>();
                    if (attribute == null || attribute.Args == null || attribute.Args.Length != 1)
                        return false;
                    return attribute.Args[0] == type;
                });
            invokerImplements.Add(type, impl);
            return impl;
        }
    }
}