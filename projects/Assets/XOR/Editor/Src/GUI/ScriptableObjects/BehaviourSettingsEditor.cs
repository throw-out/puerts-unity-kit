using System;
using System.Collections.Generic;
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
        private static Dictionary<Type, bool> foldoutDict;
        private Dictionary<BehaviourSettings.Category, ReorderableList> reorderableListDict;

        void OnEnable()
        {
            reorderableListDict = new Dictionary<BehaviourSettings.Category, ReorderableList>();
        }
        void OnDisable()
        {
            reorderableListDict = null;
        }


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
            GUILayout.BeginVertical();

            RenderCategories(categories);
            RenderMenu(settings);

            GUILayout.EndVertical();
        }
        void ClearReorderableList()
        {
            if (reorderableListDict == null)
                return;
            reorderableListDict.Clear();
        }
        ReorderableList GetReorderableList(BehaviourSettings.Category category, bool create = true)
        {
            if (reorderableListDict == null)
                return null;
            if (reorderableListDict.TryGetValue(category, out var reorderableList) || !create)
                return reorderableList;

            var groups = category.groups;
            if (groups == null)
            {
                groups = new List<BehaviourSettings.Group>();
                category.groups = groups;
            }

            reorderableList = new ReorderableList(
                groups,
                typeof(BehaviourSettings.Group),
                true, true, true, true
            );
            reorderableList.elementHeightCallback = (index) =>
            {
                return EditorGUIUtility.singleLineHeight;
            };
            reorderableList.drawHeaderCallback = (rect) =>
            {   //绘制表头
                rect.height = EditorGUIUtility.singleLineHeight;
                RenderTitle(rect);
            };
            reorderableList.drawElementCallback = (rect, index, selected, focused) =>
            {   //绘制元素
                if (groups == null || index < 0 || index >= groups.Count)
                    return;
                rect.height = EditorGUIUtility.singleLineHeight;
                RenderGroup(rect, category, groups[index]);
            };
            reorderableList.onRemoveCallback = (list) =>
            {
                int index = list.index;
                if (groups == null || index < 0 || index >= groups.Count)
                    return;
                groups.RemoveAt(list.index);
            };
            reorderableList.onAddCallback = (list) =>
            {
                if (groups == null)
                    return;
                groups.Add(new BehaviourSettings.Group()
                {
                    type = BehaviourSettings.GroupType.Single,
                    value = BehaviourSettings.Category.GetEnumValue(category.Type)
                });
            };
            reorderableList.onChangedCallback = (list) =>
            {

            };
            return reorderableList;
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

        void RenderCategories(List<BehaviourSettings.Category> categories)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                /* if (GUIUtil.RenderHeader(Helper.GetTitle(categories[i].Type)))
                {
                    SetFoldout(categories[i].Type, !GetFoldout(categories[i].Type, true));
                } */
                if (GetFoldout(categories[i].Type, true))
                {
                    GetReorderableList(categories[i])?.DoLayoutList();
                }
                GUILayout.Space(100f);
                break;
            }
        }
        void RenderMenu(BehaviourSettings settings)
        {
            if (GUILayout.Button(Language.Behaviour.Get("reset")))
            {
                settings.SetDefault();
                ClearReorderableList();
            }
            if (GUILayout.Button(Language.Behaviour.Get("start_services")))
            {
                if (settings.categories == null)
                    return;
                settings.categories.Add(BehaviourSettings.Category.CreateDefaultSingle<BehaviourArg0>());
            }
            if (GUILayout.Button(Language.Behaviour.Get("start_services")))
            {

            }
        }
        void RenderTitle(Rect rect)
        {
            GetRenderPositions(rect, new Vector2(20, 0), out var pos1, out var pos2, out var pos3);
            EditorGUI.LabelField(pos1, "type", string.Empty);
            EditorGUI.LabelField(pos2, "value", string.Empty);
            EditorGUI.LabelField(pos3, "count", string.Empty);
        }
        void RenderGroup(Rect rect, BehaviourSettings.Category category, BehaviourSettings.Group group)
        {
            GetRenderPositions(rect, out var pos1, out var pos2, out var pos3);

            //type
            group.type = (BehaviourSettings.GroupType)EditorGUI.EnumPopup(pos1, group.type);
            //value
            Enum value = (Enum)Enum.ToObject(category.Type, group.value);
            group.value = Convert.ToUInt32(EditorGUI.EnumFlagsField(pos2, value));

            //count
            EditorGUI.LabelField(pos3, "-", string.Empty);
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
        public static void CombineNext<T>(List<T> source, int num, int index, List<T> temp, List<List<T>> results)
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
                CombineNext(source, num - 1, i + 1, newTemp, results);
            }
        }
        /// <summary>
        /// 获取n个元素中取m个元素的组合数量数量(m大于0且m小于等于n): 公式 n! / (m! * (n - m)!)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns> 
        public static int GetCombineCountOfNM(int n, int m)
        {
            if (m <= 0 || m > n)
                return 0;
            return n.Factorial() / (m.Factorial() * (n - m).Factorial());
        }
        /// <summary>
        /// 获取n个元素取任意个元素的组合数量(长度大于0且小于等于n): 公式 2^n - 1
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int GetCombineCountOfAny(int n)
        {
            if (n < 0 || n >= 32)
                return 0;
            return 2 ^ n - 1;
        }

        /// <summary>
        /// 获取value的阶乘值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static int Factorial(this int value)
        {
            if (value <= 0)
                return 1;
            return value * Factorial(value - 1);
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