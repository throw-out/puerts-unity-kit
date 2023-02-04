using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 定义序列化类型对应的编辑器
/// 使用Assembly Definition Reference定义合并至puerts.unity.kit.xor程序集
/// </summary>
namespace XOR.Serializables.TsComponent
{
    [RenderTarget(typeof(XOR.TsComponent.Color))]
    internal class ColorRenderer : XOR.Serializables.TsComponent.Renderer
    {
        protected override void RenderValue()
        {
            var newValue = EditorGUILayout.ColorField(Node.ValueNode.colorValue);
            if (newValue != Node.ValueNode.colorValue)
            {
                Node.ValueNode.colorValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.TsComponent.MyData))]
    internal class MyDataRenderer : XOR.Serializables.TsComponent.Renderer
    {
        protected override void RenderValue()
        {
            Dirty |= RenderValueNode(Node.ValueNode);
        }
        public static bool RenderValueNode(SerializedProperty node)
        {
            bool dirty = false;
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            var memberNode = node.FindPropertyRelative("name");
            EditorGUILayout.LabelField("Name", GUILayout.Width(PropertyNameWidth));
            var newValue = EditorGUILayout.TextField(memberNode.stringValue);
            if (newValue != memberNode.stringValue)
            {
                memberNode.stringValue = newValue;
                dirty |= true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            memberNode = node.FindPropertyRelative("action");
            EditorGUILayout.LabelField("Action", GUILayout.Width(PropertyNameWidth));
            newValue = EditorGUILayout.TextField(memberNode.stringValue);
            if (newValue != memberNode.stringValue)
            {
                memberNode.stringValue = newValue;
                dirty |= true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            return dirty;
        }
    }
    [RenderTarget(typeof(XOR.TsComponent.ColorArray))]
    internal class ColorArrayRenderer : XOR.Serializables.TsComponent.ArrayRenderer
    {
        protected override void RenderMemberValue(SerializedProperty node, Type type)
        {
            var newValue = EditorGUILayout.ColorField(node.colorValue);
            if (newValue != node.colorValue)
            {
                node.colorValue = newValue;
                Dirty |= true;
            }
        }
    }
    [RenderTarget(typeof(XOR.TsComponent.MyDataArray))]
    internal class MyDataArrayRenderer : XOR.Serializables.TsComponent.ArrayRenderer
    {
        protected override void RenderMemberValue(SerializedProperty node, Type type)
        {
            Dirty |= MyDataRenderer.RenderValueNode(node);
        }
    }
}