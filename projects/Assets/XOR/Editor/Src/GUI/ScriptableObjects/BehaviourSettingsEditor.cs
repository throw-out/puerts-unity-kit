using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                }
            }
            GUILayout.Space(10f);
            if (GUILayout.Button(Language.Behaviour.Get("behaviour_arg_generate")))
            {
                Helper.GenerateCode(settings);
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

        static void GetRenderPositions(Rect rect, out Rect pos1, out Rect pos2, out Rect pos3)
        {
            GetRenderPositions(rect, Vector2.zero, out pos1, out pos2, out pos3);
        }
        static void GetRenderPositions(Rect rect, Vector2 offset, out Rect pos1, out Rect pos2, out Rect pos3)
        {
            float x = rect.x,
                y = rect.y,
                width = rect.width,
                height = rect.width;
            x += offset.x;
            y += offset.y;
            pos1 = new Rect(x, y, width * 0.3f, height);
            pos2 = new Rect(pos1.x + pos1.width, y, width * 0.5f, height);
            pos3 = new Rect(pos2.x + pos2.width, y, width * 0.2f, height);
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
            });
        }

        public static void GenerateCode(BehaviourSettings settings)
        {
            GUIUtil.RenderGenerateClass(() => { }, 100);
        }
        public static string GetTitle(Type type)
        {
            if (type == null)
                return string.Empty;
            var title = type.GetCustomAttribute<XOR.Behaviour.TitleAttribute>();
            if (title != null)
                return title.Name;
            return type.Name;
        }
    }
}