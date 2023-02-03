using System.Collections.Generic;
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

            reorderableList.DoLayoutList();
        }

        void CreateReorderableList()
        {
            reorderableList = new ReorderableList(
                nodes,
                typeof(NodeWrap),
                true, true, true, true
            );
            reorderableList.elementHeightCallback = (index) =>
            {
                return display.GetHeight(nodes[index]);
            };
            reorderableList.drawElementCallback = (rect, index, selected, focused) =>
            {   //绘制元素
                bool dirty = display.Render(rect, nodes[index]);
                if (dirty)
                {
                    root.ApplyModifiedProperties();
                    root.Update();
                }
            };
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {   //绘制表头
                UnityEngine.GUI.Label(rect, "成员属性");
            };
            reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                nodes[list.index].RemoveFromArrayParent();
                root.ApplyModifiedProperties();
                root.Update();
                TsPropertiesHelper.RebuildNodes(root, nodes);
            };
            reorderableList.onAddCallback = (ReorderableList list) =>
            {
                XOR.Serializables.TsProperties.Utility.PopupCreate(root, nodes.Count, () =>
                {
                    root.ApplyModifiedProperties();
                });
            };
            reorderableList.onChangedCallback = (a) =>
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Index = i;
                }
                root.ApplyModifiedProperties();
                root.Update();
                TsPropertiesHelper.RebuildNodes(root, nodes);
            };
        }
    }

    internal static class TsPropertiesHelper
    {
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