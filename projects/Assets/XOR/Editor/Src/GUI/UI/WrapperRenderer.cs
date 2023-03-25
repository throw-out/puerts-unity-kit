using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XOR.UI
{
    internal class WrapperRenderer
    {
        const float HeaderFixed = 0.3f;
        const float Spacing = 5f;

        private SerializedObject root;
        private SerializedProperty elements;
        private ReorderableList list;
        public string Header { get; set; }

        public WrapperRenderer(SerializedProperty elements)
        {
            this.root = elements != null ? elements.serializedObject : null;
            this.elements = elements;
            this.CreateReorderableList();
        }
        public void Renderer()
        {
            if (list == null)
                return;
            list.DoLayoutList();
        }

        void CreateReorderableList()
        {
            if (elements == null || !elements.isArray)
                return;
            list = new ReorderableList(root, elements,
                false, true, true, true
            );
            list.elementHeightCallback = (index) =>
            {
                return EditorGUIUtility.singleLineHeight * 2 + Spacing + 4f;
            };
            list.drawHeaderCallback = (rect) =>
            {   //绘制表头
                EditorGUI.LabelField(rect, Header);
            };
            list.drawElementCallback = (rect, index, selected, focused) =>
            {   //绘制元素
                //EditorGUI.PropertyField(rect, elements.GetArrayElementAtIndex(index), true);
                rect.height -= Spacing - 4f;
                RenderEvent(rect, elements.GetArrayElementAtIndex(index));
                elements.serializedObject.ApplyModifiedProperties();
            };
            list.onRemoveCallback = (list) =>
            {
                elements.DeleteArrayElementAtIndex(list.index);
                root.ApplyModifiedProperties();
            };
            list.onAddCallback = (list) =>
            {
                elements.arraySize++;
                root.ApplyModifiedProperties();
            };
        }
        static void RenderEvent(Rect rect, SerializedProperty element)
        {
            float x = rect.x, y = rect.y, w = rect.width, h = rect.height;

            var targetProperty = element.FindPropertyRelative("target");
            var methodProperty = element.FindPropertyRelative("method");

            using (new EditorGUI.PropertyScope(rect, GUIContent.none, element))
            {
                //Render Type
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.EnumPopup(new Rect(new Vector2(x, y + 2f), new Vector2(w * HeaderFixed, h * 0.5f)), UnityEngine.Events.UnityEventCallState.RuntimeOnly);
                }

                //Render GameObject
                var go = Helper.GetGamObject(targetProperty.objectReferenceValue);
                var newGo = EditorGUI.ObjectField(new Rect(new Vector2(x, y + h * 0.5f), new Vector2(w * HeaderFixed, h * 0.5f)), go, typeof(GameObject), true) as GameObject;
                if (newGo != go)
                {
                    targetProperty.objectReferenceValue = newGo;
                    methodProperty.stringValue = string.Empty;      //clear method
                }

                //Render Method
                using (new EditorGUI.DisabledScope(newGo == null))
                {
                    //DropDownButton/DropDown/OffsetDropDown
                    if (GUI.Button(new Rect(new Vector2(x + w * HeaderFixed + Spacing, y + 2f), new Vector2(w * (1 - HeaderFixed) - Spacing, h * 0.5f)), "No Function", Skin.dropDown))
                    {
                        Helper.GenerteFunctionsMenu(newGo, targetProperty.objectReferenceValue, methodProperty.stringValue, (compoent, md) =>
                        {
                            targetProperty.objectReferenceValue = compoent;
                            if (md != null)
                            {
                                methodProperty.stringValue = md.name;
                            }
                            else
                            {
                                methodProperty.stringValue = string.Empty;
                            }
                            targetProperty.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
                /*/Render Parameter
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.Popup(new Rect(new Vector2(x + w * HeaderFixed + Spacing, y + h * 0.5f), new Vector2(w * (1 - HeaderFixed) - Spacing, h * 0.5f)), 0, new string[] { "No Parameter" });
                }
                //*/
            }
        }
        private static class Helper
        {
            public static GameObject GetGamObject(UnityEngine.Object target)
            {
                if (target == null)
                    return null;
                if (target is GameObject)
                {
                    return (GameObject)target;
                }
                if (target is Component)
                {
                    return ((Component)target).gameObject;
                }
                return null;
            }
            public static void GenerteFunctionsMenu(GameObject target, UnityEngine.Object obj, string methodName, Action<UnityEngine.Object, XOR.Services.MethodDeclaration> callback)
            {
                var program = EditorApplicationUtil.GetProgram();
                if (program == null || program.state != XOR.Services.ProgramState.Completed)
                {
                    GUIUtil.RenderMustLaunchServices();
                    return;
                }
                var menu = new UnityEditor.GenericMenu();
                menu.allowDuplicateNames = true;

                var components = target.GetComponents<XOR.TsComponent>();
                var itemHeader = string.Empty;
                for (int i = 0; i < components.Length; i++)
                {
                    itemHeader = components.Length > 1 ? $"{nameof(TsComponent)}-{i + 1}/" : $"{nameof(TsComponent)}/";
                    var component = components[i];
                    menu.AddItem(new GUIContent($"{itemHeader}Non Function"), false, () => callback(component, null));
                    menu.AddSeparator(itemHeader);

                    var statement = program.GetStatement(component.GetGuid());
                    if (statement == null || !(statement is XOR.Services.TypeDeclaration type) || !type.HasMethods())
                    {
                        menu.AddDisabledItem(new GUIContent($"{itemHeader}Empty"), false);
                        continue;
                    }
                    foreach (var method in type.GetMethods())
                    {
                        menu.AddItem(new GUIContent($"{itemHeader}{method.name}"), false, () => callback(component, method));
                    }
                }

                menu.ShowAsContext();
            }
        }
    }
}