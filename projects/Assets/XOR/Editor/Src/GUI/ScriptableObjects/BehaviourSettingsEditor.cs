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
            var settings = BehaviourSettings.Load(true, true);
            if (settings == null)
                return;
            var categories = settings.categories;
            if (categories == null)
            {
                categories = new List<BehaviourSettings.Category>();
                settings.categories = categories;
                EditorUtility.SetDirty(settings);
            }

            if (categories == null || categories.Count == 0)
            {
                GUILayout.Label("Empty Configure");
            }
            else
            {
                RenderCategories(categories);
            }
            GUILayout.Space(20f);
            RenderMenu(settings);

            AssetDatabase.SaveAssets();
        }

        void RenderCategories(List<BehaviourSettings.Category> categories)
        {
            foreach (var category in categories)
            {
                GUILayout.Space(Space);
                if (GUIUtil.RenderHeader(Helper.GetTitle(category.Type)))
                {
                    SetFoldout(category.Type, !GetFoldout(category.Type, true));
                }
                if (!GetFoldout(category.Type, true))
                    continue;
                GUIUtil.RenderGroup(RenderCategory, category);
            }
        }
        void RenderCategory(BehaviourSettings.Category category)
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
                var remove = RenderGroup(category, group, ref total);
                if (remove)
                {
                    removeGroup = group;
                }
            }
            RenderCategoryMenu(category, total);

            if (removeGroup != null)
            {
                category.groups.Remove(removeGroup);
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
        void RenderCategoryMenu(BehaviourSettings.Category category, int total)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(ButtonWidth)))
            {
                category.groups.Add(new BehaviourSettings.Group()
                {
                    type = BehaviourSettings.GroupType.Single
                });
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{total}", Skin.labelCenter, GUILayout.Width(ClassWidth));

            GUILayout.EndHorizontal();
        }
        bool RenderGroup(BehaviourSettings.Category category, BehaviourSettings.Group group, ref int total)
        {
            bool ok = false;

            GUILayout.BeginHorizontal();
            //delete
            if (GUILayout.Button("-", GUILayout.Width(ButtonWidth)))
            {
                ok = true;
            }
            //type
            group.type = (BehaviourSettings.GroupType)EditorGUILayout.EnumPopup(group.type, GUILayout.Width(TypeWidth));
            //value
            Enum value = (Enum)Enum.ToObject(category.Type, group.value);
            group.value = Convert.ToUInt32(EditorGUILayout.EnumFlagsField(value));
            //param
            if (group.type == BehaviourSettings.GroupType.Combine)
            {
                group.param = Math.Min(
                    (uint)EditorGUILayout.IntField((int)group.param, GUILayout.Width(ClassWidth)),
                    (uint)BehaviourSettings.Helper.GetEnumCountByFlags(category.Type, group.value)
                );
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
        void RenderMenu(BehaviourSettings settings)
        {
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_add")))
            {
                if (settings.categories == null)
                {
                    settings.categories = new List<BehaviourSettings.Category>();
                }
                Helper.PopupAddCategory(settings);
            }
            GUILayout.Space(10f);
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_preference")))
            {
                if (settings.categories != null && settings.categories.Any(c => c.groups != null && c.groups.Count > 0))
                {
                    GUIUtil.RenderConfirm("override_current_data", settings.SetPreference);
                }
                else
                {
                    settings.SetPreference();
                    EditorUtility.SetDirty(settings);
                }
            }
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_default")))
            {
                if (settings.categories != null && settings.categories.Any(c => c.groups != null && c.groups.Count > 0))
                {
                    GUIUtil.RenderConfirm("override_current_data", settings.SetDefault);
                }
                else
                {
                    settings.SetDefault();
                    EditorUtility.SetDirty(settings);
                }
            }
            GUILayout.Space(10f);
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_generate")))
            {
                Helper.GenerateCode(settings);
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
            var @namespace = "XOR.Behaviour.Args";
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
            Behaviour.Factory.ClearRegister();
            Behaviour.Default.Register();
            var enums = settings.GenerateTypes()
                ?.Where(e => !Behaviour.Factory.HasRegister(e))
                .ToList();
            if (enums == null || enums.Count <= 0)
            {
                GUIUtil.RenderGenerateClassEmpty();
                return;
            }
            GUIUtil.RenderGenerateClass(() =>
            {
                ClearGenerateCode();
                GenerateCode(enums);
            }, enums.Count);
        }
        public static void ClearGenerateCode()
        {
            var saveTo = Puerts.Configure.GetCodeOutputDirectory() + "BehaviourGenerateCode.cs";
            if (File.Exists(saveTo))
            {
                File.Delete(saveTo);
                AssetDatabase.Refresh();
            }
        }
        public static void GenerateCode(IEnumerable<Enum> enums)
        {
            List<Tuple<string, string>> classes = new List<Tuple<string, string>>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($@"
namespace XOR.Behaviour
{{
    public static class StaticWrap
    {{
");
            foreach (var @enum in enums)
            {
                var dv = Convert.ToUInt32(@enum);
                var type = @enum.GetType();
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
                GetArgs(type, out string declaration, out string @params);
                if (!string.IsNullOrEmpty(@params))
                {
                    @params = ", " + @params;
                }
                sb.AppendLine(@$"
        protected class {className} : XOR.Behaviour.{type.Name}
        {{
            {string.Join("", methodNames.Select(methodName => $@"
            private void {methodName}({declaration})
            {{
                Invoke(XOR.Behaviour.Args.{type.Name}.{methodName}{@params});
            }}"))}
        }}");
            }
            sb.AppendLine(@$"
        public static void Register()
        {{
            {string.Join("", classes.Select(a => @$"
            XOR.Behaviour.Factory.Register<{a.Item1}>({a.Item2});"))}
        }}
    }}
}}");

            var saveTo = Puerts.Configure.GetCodeOutputDirectory() + "BehaviourGenerateCode.cs";
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
        public static string GetArgs(Type type)
        {
            if (type == null)
                return string.Empty;
            var attribute = type.GetCustomAttribute<XOR.Behaviour.ArgsAttribute>();
            if (attribute != null && attribute.Args != null)
                return string.Join(", ", attribute.Args.Select(t => t.FullName));
            return string.Empty;
        }
        public static bool GetArgs(Type type, out string declaration, out string @params)
        {
            declaration = string.Empty;
            @params = string.Empty;
            if (type == null)
                return false;
            var attribute = type.GetCustomAttribute<XOR.Behaviour.ArgsAttribute>();
            if (attribute != null && attribute.Args != null)
            {
                declaration = string.Join(", ", attribute.Args.Select((t, index) => t.FullName + $" arg{index}"));
                @params = string.Join(", ", attribute.Args.Select((t, index) => $"arg{index}"));
                return true;
            }
            return false;
        }
    }
}