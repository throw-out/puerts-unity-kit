using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XOR.Serializables;
using XOR.Services;

namespace XOR
{
    [CustomEditor(typeof(TsProperties))]

    internal class TsPropertiesEditor : Editor
    {
        private static bool menuFoldout = true;
        private static bool memberFoldout = true;
        private static bool diagnosisFoldout = true;

        private TsProperties component;
        private ReorderableList reorderableList;

        private RootWrap root;
        private List<NodeWrap> nodes;
        private XOR.Serializables.TsProperties.Display display;

        void OnEnable()
        {
            component = target as TsProperties;
            //初始化节点信息
            root = RootWrap.Create(serializedObject, typeof(TsProperties));
            nodes = new List<NodeWrap>();
            display = XOR.Serializables.TsProperties.Display.Create(root);
            CreateReorderableList();
        }
        void OnDisable()
        {
            component = null;
        }
        public override void OnInspectorGUI()
        {
            if (component == null)
            {
                return;
            }
            TsPropertiesHelper.RebuildNodes(root, nodes);

            EditorGUILayout.BeginVertical();
            RenderMenu();
            RenderMembers();
            RenderDiagnosis();
            EditorGUILayout.EndVertical();
        }

        void RenderMenu()
        {
            if (GUIUtil.RenderHeader("菜单"))
            {
                menuFoldout = !menuFoldout;
            }
            if (menuFoldout)
            {
                GUIUtil.RenderGroup(_RenderMenu);
            }
        }
        void RenderMembers()
        {
            if (GUIUtil.RenderHeader("成员属性"))
            {
                memberFoldout = !memberFoldout;
            }
            if (memberFoldout)
            {
                reorderableList.DoLayoutList();
            }
        }
        void RenderDiagnosis()
        {
            if (GUIUtil.RenderHeader("诊断"))
            {
                diagnosisFoldout = !diagnosisFoldout;
            }
            if (diagnosisFoldout)
            {
                GUIUtil.RenderGroup(_RenderDiagnosis);
            }
        }

        void _RenderMenu()
        {
            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Declare"))
            {
                XOR.Serializables.TsProperties.Utility.CustomMenu(new string[] {
                    "public",
                    "protected",
                    "private",
                    "FullType/public",
                    "FullType/protected",
                    "FullType/private",
                }, null, null, null, selectIndex =>
                {
                    string code = XOR.Serializables.TsProperties.Utility.GenerateDeclareCode(
                        component.GetProperties(),
                        (selectIndex % 3) == 0 ? "declare public" : (selectIndex % 3) == 1 ? "declare protected" : "declare private",
                        selectIndex >= 0
                    );
                    var editor = new TextEditor();
                    editor.text = code;
                    editor.OnFocus();
                    editor.Copy();
                    Debug.Log("已复制到剪贴板:\n" + code);
                });
            }
            if (GUILayout.Button("Parsed Declare"))
            {
                var editor = new TextEditor();
                editor.OnFocus();
                editor.Paste();
                var code = editor.text;
                if (string.IsNullOrEmpty(code))
                    return;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            Prefs.CheckKeyRedefinition.SetValue(EditorGUILayout.Toggle(string.Empty, Prefs.CheckKeyRedefinition, GUILayout.Width(20f)));
            EditorGUILayout.LabelField("check key redefinition");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            Prefs.CheckKeyValidity.SetValue(EditorGUILayout.Toggle(string.Empty, Prefs.CheckKeyValidity, GUILayout.Width(20f)));
            EditorGUILayout.LabelField("check key validity");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
        }
        void _RenderDiagnosis()
        {
            int DiagnosisCount = 0;
            XOR.Serializables.ResultPair[] pairs = (Prefs.CheckKeyRedefinition || Prefs.CheckKeyValidity) ? component.GetProperties() : null;
            if (pairs != null && pairs.Length > 0)
            {
                List<string> usedKeys = new List<string>();
                for (int i = 0; i < pairs.Length; i++)
                {
                    string key = pairs[i].key;
                    if (Prefs.CheckKeyRedefinition)
                    {
                        if (usedKeys.Contains(key))
                        {
                            GUILayout.Space(5f);
                            GUILayout.BeginHorizontal("HelpBox");
                            GUILayout.Label(string.Empty, Skin.warnIcon);
                            GUILayout.Label($"redefinition key at {i} (other at {usedKeys.IndexOf(key)})\n", Skin.labelArea);
                            GUILayout.EndHorizontal();
                            DiagnosisCount++;
                        }
                        else usedKeys.Add(key);
                    }
                    if (Prefs.CheckKeyValidity && !XOR.Serializables.TsProperties.Utility.IsValidKey(key))
                    {
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal("HelpBox");
                        GUILayout.Label(string.Empty, Skin.warnIcon);
                        GUILayout.Label($"invail key definition at {i} ({key})\n");
                        GUILayout.EndHorizontal();
                        DiagnosisCount++;
                    }
                }
            }
            if (DiagnosisCount == 0)
            {
                GUILayout.Label("Empty");
            }
        }

        void CreateReorderableList()
        {
            reorderableList = new ReorderableList(
                nodes,
                typeof(NodeWrap),
                true, false, true, true
            );
            reorderableList.elementHeightCallback = (index) =>
            {
                return display.GetHeight(nodes[index]);
            };
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {   //绘制表头
                //UnityEngine.GUI.Label(rect, "成员属性");
            };
            reorderableList.drawElementCallback = (rect, index, selected, focused) =>
            {   //绘制元素
                bool dirty = display.Render(rect, nodes[index]);
                if (dirty)
                {
                    TsPropertiesHelper.ChangePropertyEvent(component, nodes[index].Key);
                    root.ApplyModifiedProperties();
                    root.Update();
                }
            };
            reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                nodes[list.index].RemoveFromArrayParent();
                root.ApplyModifiedProperties();
                TsPropertiesHelper.RebuildNodes(root, nodes);
            };
            reorderableList.onAddCallback = (ReorderableList list) =>
            {
                XOR.Serializables.TsProperties.Utility.PopupCreate(root, nodes.Count, () =>
                {
                    root.ApplyModifiedProperties();
                    TsPropertiesHelper.RebuildNodes(root, nodes);
                });
            };
            reorderableList.onChangedCallback = (a) =>
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Index = i;
                }
                root.ApplyModifiedProperties();
                //TsPropertiesHelper.RebuildNodes(root, nodes);
            };
        }
    }

    internal static class TsPropertiesHelper
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        static FieldInfo _onPropertyChange;
        static FieldInfo onPropertyChange
        {
            get
            {
                if (_onPropertyChange == null) _onPropertyChange = typeof(TsProperties).GetField("onPropertyChange", Flags);
                return _onPropertyChange;
            }
        }

        public static void ChangePropertyEvent(TsProperties component, string key)
        {
            Action<string, object> func = onPropertyChange?.GetValue(component) as Action<string, object>;
            if (func == null)
            {
                return;
            }
            ComponentWrap<TsProperties> cw = ComponentWrap<TsProperties>.Create();
            IPair pair = cw.GetProperty(component, key);
            if (pair == null)
            {
                return;
            }
            func(pair.Key, pair.Value);
        }

        public static void RebuildNodes(RootWrap root, List<NodeWrap> outputNodes)
        {
            if (outputNodes == null)
                return;
            outputNodes.Clear();
            //创建当前需要渲染的节点信息
            foreach (var info in root.FieldMapping)
            {
                SerializedProperty arrayParent = root.GetProperty(info.Key);
                for (int i = 0; i < arrayParent.arraySize; i++)
                {
                    outputNodes.Add(new NodeWrap(
                        arrayParent.GetArrayElementAtIndex(i),
                        info.Value.Element.Type,
                        i,
                        arrayParent
                    ));
                }
            }
            outputNodes.Sort((n1, n2) => n1.Index < n2.Index ? -1 : n1.Index < n2.Index ? 1 : 0);
        }

        public static void SetDirty(UnityEngine.Object obj)
        {
            if (obj == null)
                return;
            EditorUtility.SetDirty(obj);
        }
    }
}