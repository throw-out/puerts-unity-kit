using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XOR.UI
{
    public static class UIMenu
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        static readonly HashSet<Type> OriginalTypes = new HashSet<Type>(new Type[]{
            typeof(UnityEngine.UI.Button),
            typeof(UnityEngine.UI.Toggle),
            typeof(UnityEngine.UI.Slider),
            typeof(UnityEngine.UI.Dropdown),
            typeof(UnityEngine.UI.Scrollbar),
            typeof(UnityEngine.UI.InputField),
            typeof(UnityEngine.EventSystems.EventTrigger),
        });
        static readonly HashSet<Type> WrapperTypes = new HashSet<Type>(new Type[]{
            typeof(XOR.UI.ButtonWrapper),
            typeof(XOR.UI.ToggleWrapper),
            typeof(XOR.UI.SliderWrapper),
            typeof(XOR.UI.DropdownWrapper),
            typeof(XOR.UI.ScrollbarWrapper),
            typeof(XOR.UI.InputFieldWrapper),
            typeof(XOR.UI.EventTriggerWrapper),
        });
        static readonly Dictionary<Type, Func<DefaultControls.Resources, GameObject>> creators = new Dictionary<Type, Func<DefaultControls.Resources, GameObject>>()
        {
            {typeof(UnityEngine.UI.Button), UnityEngine.UI.DefaultControls.CreateButton},
            {typeof(UnityEngine.UI.Toggle), UnityEngine.UI.DefaultControls.CreateToggle},
            {typeof(UnityEngine.UI.Slider), UnityEngine.UI.DefaultControls.CreateSlider},
            {typeof(UnityEngine.UI.Dropdown), UnityEngine.UI.DefaultControls.CreateDropdown},
            {typeof(UnityEngine.UI.Scrollbar), UnityEngine.UI.DefaultControls.CreateScrollbar},
            {typeof(UnityEngine.UI.InputField), UnityEngine.UI.DefaultControls.CreateInputField},
        };
        static readonly Dictionary<Type, Action<GameObject>> wrappers = new Dictionary<Type, Action<GameObject>>()
        {
            {typeof(UnityEngine.UI.Button), (go) => Wrapper<UnityEngine.UI.Button, XOR.UI.ButtonWrapper>(go)},
            {typeof(UnityEngine.UI.Toggle), (go) => Wrapper<UnityEngine.UI.Toggle, XOR.UI.ToggleWrapper>(go)},
            {typeof(UnityEngine.UI.Slider), (go) => Wrapper<UnityEngine.UI.Slider, XOR.UI.SliderWrapper>(go)},
            {typeof(UnityEngine.UI.Dropdown), (go) => Wrapper<UnityEngine.UI.Dropdown, XOR.UI.DropdownWrapper>(go)},
            {typeof(UnityEngine.UI.Scrollbar), (go) => Wrapper<UnityEngine.UI.Scrollbar, XOR.UI.ScrollbarWrapper>(go)},
            {typeof(UnityEngine.UI.InputField), (go) => Wrapper<UnityEngine.UI.InputField, XOR.UI.InputFieldWrapper>(go)},
            {typeof(UnityEngine.EventSystems.EventTrigger), (go) => Wrapper<UnityEngine.EventSystems.EventTrigger, XOR.UI.EventTriggerWrapper>(go)},
        };
        static readonly Dictionary<Type, Action<GameObject>> restores = new Dictionary<Type, Action<GameObject>>()
        {
            {typeof(UnityEngine.UI.Button), (go) => Restore<UnityEngine.UI.Button, XOR.UI.ButtonWrapper>(go)},
            {typeof(UnityEngine.UI.Toggle), (go) => Restore<UnityEngine.UI.Toggle, XOR.UI.ToggleWrapper>(go)},
            {typeof(UnityEngine.UI.Slider), (go) => Restore<UnityEngine.UI.Slider, XOR.UI.SliderWrapper>(go)},
            {typeof(UnityEngine.UI.Dropdown), (go) => Restore<UnityEngine.UI.Dropdown, XOR.UI.DropdownWrapper>(go)},
            {typeof(UnityEngine.UI.Scrollbar), (go) => Restore<UnityEngine.UI.Scrollbar, XOR.UI.ScrollbarWrapper>(go)},
            {typeof(UnityEngine.UI.InputField), (go) => Restore<UnityEngine.UI.InputField, XOR.UI.InputFieldWrapper>(go)},
            {typeof(UnityEngine.EventSystems.EventTrigger), (go) => Restore<UnityEngine.EventSystems.EventTrigger, XOR.UI.EventTriggerWrapper>(go)},
        };

        static bool RestoreWarning
        {
            get { return EditorPrefs.GetBool("Settings.UIWrapper.Restore", true); }
            set { EditorPrefs.SetBool("Settings.UIWrapper.Restore", value); }
        }

        #region Convert Menu

        [MenuItem("CONTEXT/Button/Wrapper")]
        [MenuItem("CONTEXT/Toggle/Wrapper")]
        [MenuItem("CONTEXT/Slider/Wrapper")]
        [MenuItem("CONTEXT/Dropdown/Wrapper")]
        [MenuItem("CONTEXT/Scrollbar/Wrapper")]
        [MenuItem("CONTEXT/InputField/Wrapper")]
        [MenuItem("CONTEXT/EventTrigger/Wrapper")]
        static void Wrapper(MenuCommand command)
        {
            if (command == null || command.context == null || !(command.context is UnityEngine.Component))
                return;
            Action<GameObject> wrapper;
            if (wrappers.TryGetValue(command.context.GetType(), out wrapper))
            {
                wrapper(((UnityEngine.Component)command.context).gameObject);
            }
        }

        [MenuItem("CONTEXT/Button/Wrapper", true)]
        [MenuItem("CONTEXT/Toggle/Wrapper", true)]
        [MenuItem("CONTEXT/Slider/Wrapper", true)]
        [MenuItem("CONTEXT/Dropdown/Wrapper", true)]
        [MenuItem("CONTEXT/Scrollbar/Wrapper", true)]
        [MenuItem("CONTEXT/InputField/Wrapper", true)]
        [MenuItem("CONTEXT/EventTrigger/Wrapper", true)]
        static bool WrapperValidate(MenuCommand command)
        {
            return command != null &&
                command.context != null &&
                OriginalTypes.Contains(command.context.GetType());
        }

        #endregion


        #region Restore Menu

        [MenuItem("CONTEXT/Button/Restore")]
        [MenuItem("CONTEXT/Toggle/Restore")]
        [MenuItem("CONTEXT/Slider/Restore")]
        [MenuItem("CONTEXT/Dropdown/Restore")]
        [MenuItem("CONTEXT/Scrollbar/Restore")]
        [MenuItem("CONTEXT/InputField/Restore")]
        [MenuItem("CONTEXT/EventTrigger/Restore")]
        static void Restore(MenuCommand command)
        {
            if (command == null || command.context == null || !(command.context is UnityEngine.Component))
                return;
            Action<GameObject> restore;
            if (restores.TryGetValue(command.context.GetType().BaseType, out restore))
            {
                restore(((UnityEngine.Component)command.context).gameObject);
            }
        }

        [MenuItem("CONTEXT/Button/Restore", true)]
        [MenuItem("CONTEXT/Toggle/Restore", true)]
        [MenuItem("CONTEXT/Slider/Restore", true)]
        [MenuItem("CONTEXT/Dropdown/Restore", true)]
        [MenuItem("CONTEXT/Scrollbar/Restore", true)]
        [MenuItem("CONTEXT/InputField/Restore", true)]
        [MenuItem("CONTEXT/EventTrigger/Restore", true)]
        static bool RestoreValidate(MenuCommand command)
        {
            return command != null &&
                command.context != null &&
                WrapperTypes.Contains(command.context.GetType());
        }

        #endregion


        #region Create UI Component

        [MenuItem("GameObject/UI/[★]Wrapper/Button")]
        static void AddButton(MenuCommand command)
        {
            CreateUIComponent<XOR.UI.ButtonWrapper>(command);
        }
        [MenuItem("GameObject/UI/[★]Wrapper/Toggle")]
        static void AddToggle(MenuCommand command)
        {
            CreateUIComponent<XOR.UI.ToggleWrapper>(command);
        }
        [MenuItem("GameObject/UI/[★]Wrapper/Slider")]
        static void AddSlider(MenuCommand command)
        {
            CreateUIComponent<XOR.UI.SliderWrapper>(command);
        }
        [MenuItem("GameObject/UI/[★]Wrapper/Dropdown")]
        static void AddDropdown(MenuCommand command)
        {
            CreateUIComponent<XOR.UI.DropdownWrapper>(command);
        }
        [MenuItem("GameObject/UI/[★]Wrapper/Scrollbar")]
        static void AddScrollbar(MenuCommand command)
        {
            CreateUIComponent<XOR.UI.ScrollbarWrapper>(command);
        }
        [MenuItem("GameObject/UI/[★]Wrapper/InputField")]
        static void AddInputField(MenuCommand command)
        {
            CreateUIComponent<XOR.UI.InputFieldWrapper>(command);
        }
        static void CreateUIComponent<T>(MenuCommand command)
            where T : UnityEngine.Component
        {
            //UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(context);       //判断是否预制体编辑场景
            Func<DefaultControls.Resources, GameObject> creator;
            if (!creators.TryGetValue(typeof(T).BaseType, out creator))
            {
                return;
            }
            GameObject go = creator(GetStandardResources());
            PlaceUIElementRoot(go, command);

            //convert to target type
            Action<GameObject> wrapper;
            if (wrappers.TryGetValue(typeof(T).BaseType, out wrapper))
            {
                wrapper(go);
            }
        }

        static Func<DefaultControls.Resources> _GetStandardResources;
        static DefaultControls.Resources GetStandardResources()
        {
            if (_GetStandardResources == null)
            {
                _GetStandardResources = DelegateUtil.CreateMemberBridge<Func<DefaultControls.Resources>>(
                    "UnityEditor.UI.MenuOptions",
                    "GetStandardResources",
                    true
                );
                if (_GetStandardResources == null)
                    throw new Exception("method missing");
            }
            return _GetStandardResources();
        }
        static Action<GameObject, MenuCommand> _PlaceUIElementRoot;
        static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            if (_PlaceUIElementRoot == null)
            {
                _PlaceUIElementRoot = DelegateUtil.CreateMemberBridge<Action<GameObject, MenuCommand>>(
                    "UnityEditor.UI.MenuOptions",
                    "PlaceUIElementRoot",
                    true
                );
                if (_PlaceUIElementRoot == null)
                    throw new Exception("method missing");
            }
            _PlaceUIElementRoot(element, menuCommand);
        }

        #endregion


        static void Wrapper<TSource, TTarget>(GameObject go)
            where TSource : UnityEngine.Component
            where TTarget : TSource
        {
            var source = go.GetComponent<TSource>();
            if (source == null)
            {
                return;
            }
            var values = GetMembers<TSource>(source);
            GameObject.DestroyImmediate(source);
            var target = go.AddComponent<TTarget>();
            SetMembers<TSource>(target, values);
            EditorUtility.SetDirty(go);
        }
        static void Restore<TSource, TTarget>(GameObject go)
            where TSource : UnityEngine.Component
            where TTarget : TSource
        {
            var target = go.GetComponent<TTarget>();
            if (target == null)
            {
                return;
            }
            if (RestoreWarning && GetEventCount(target) > 0 &&
                !EditorUtility.DisplayDialog("Warning", "Restoring component will clear events.", "Continue", "Cancel")
            )
            {
                return;
            }

            var values = GetMembers<TSource>(target);
            GameObject.DestroyImmediate(target);
            var source = go.AddComponent<TSource>();
            SetMembers<TSource>(source, values);
            EditorUtility.SetDirty(go);
        }
        static Dictionary<MemberInfo, object> GetMembers<T>(T obj)
        {
            var members = new Dictionary<MemberInfo, object>();
            foreach (var field in typeof(T).GetFields(Flags))
            {
                members.Add(field, field.GetValue(obj));
            }
            foreach (var property in typeof(T).GetProperties(Flags))
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;
                members.Add(property, property.GetValue(obj));
            }
            return members;
        }
        static void SetMembers<T>(T obj, Dictionary<MemberInfo, object> values)
        {
            foreach (var value in values)
            {
                if (value.Key is FieldInfo)
                {
                    ((FieldInfo)value.Key).SetValue(obj, value.Value);
                }
                else if (value.Key is PropertyInfo)
                {
                    ((PropertyInfo)value.Key).SetValue(obj, value.Value);
                }
            }
        }


        static int GetEventCount<TComponent>(TComponent compoennt)
            where TComponent : UnityEngine.Component
        {
            int count = 0;
            if (compoennt is ButtonWrapper button)
            {
                count = button.GetWrapperEventCount();
            }
            else if (compoennt is DropdownWrapper dropdown)
            {
                count = dropdown.GetOnValueChangedEventCount();
            }
            else if (compoennt is InputFieldWrapper inputField)
            {
                count = inputField.GetOnValueChangedEventCount() + inputField.GetOnEndEditEventCount();
            }
            else if (compoennt is ScrollbarWrapper scrollbar)
            {
                count = scrollbar.GetOnValueChangedEventCount();
            }
            else if (compoennt is SliderWrapper slider)
            {
                count = slider.GetOnValueChangedEventCount();
            }
            else if (compoennt is ToggleWrapper toggle)
            {
                count = toggle.GetOnValueChangedEventCount();
            }
            else if (compoennt is EventTriggerWrapper eventTrigger)
            {
                count = eventTrigger.GetEventCount();
            }
            return count;
        }
    }
}