using System;
using UnityEngine;

namespace XOR
{
    internal static class Skin
    {
        public const float LineSpace = 4f;
        public const float RowSpace = 2f;

        public static readonly Accessor<GUIStyle> headerBox = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle("HeaderButton");
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        });
        public static readonly Accessor<GUIStyle> groupBox = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle("GroupBox");
            style.padding = new RectOffset()
            {
                left = 4,
                right = 4,
                top = 4,
                bottom = 4,
            };
            style.margin = new RectOffset();
            return style;
        });

        public static readonly Accessor<GUIStyle> label = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            style.richText = true;
            return style;
        });
        public static readonly Accessor<GUIStyle> labelBold = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            return style;
        });
        public static readonly Accessor<GUIStyle> labelCenter = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            return style;
        });
        public static readonly Accessor<GUIStyle> labelClear = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.clear;
            style.hover = style.focused = style.active =
            style.onNormal = style.onHover = style.onActive = style.onFocused = style.normal;
            return style;
        });

        public static readonly Accessor<GUIStyle> labelGreen = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.green;
            style.hover = style.focused = style.active =
            style.onNormal = style.onHover = style.onActive = style.onFocused = style.normal;
            return style;
        });
        public static readonly Accessor<GUIStyle> labelYellow = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.yellow;
            style.hover = style.focused = style.active =
            style.onNormal = style.onHover = style.onActive = style.onFocused = style.normal;
            return style;
        });
        public static readonly Accessor<GUIStyle> labelGray = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.gray;
            style.hover = style.focused = style.active =
            style.onNormal = style.onHover = style.onActive = style.onFocused = style.normal;
            return style;
        });
        public static readonly Accessor<GUIStyle> labelArea = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            style.richText = true;
            style.stretchWidth = false;
            style.stretchHeight = true;
            style.wordWrap = true;
            return style;
        });

        public static readonly Accessor<GUIStyle> arrawLeft = new Accessor<GUIStyle>(() =>
        {
            return "ArrowNavigationLeft";
        });
        public static readonly Accessor<GUIStyle> arrawRight = new Accessor<GUIStyle>(() =>
        {
            return "ArrowNavigationRight";
        });
        public static readonly Accessor<GUIStyle> infoIcon = new Accessor<GUIStyle>(() =>
        {
            return "CN EntryInfoIconSmall";
        });
        public static readonly Accessor<GUIStyle> warnIcon = new Accessor<GUIStyle>(() =>
        {
            return "CN EntryWarnIconSmall";
        });
        public static readonly Accessor<GUIStyle> errorIcon = new Accessor<GUIStyle>(() =>
        {
            return "CN EntryErrorIconSmall";
        });


        public static readonly Accessor<GUIStyle> dropDown = new Accessor<GUIStyle>(() =>
        {
            //DropDownButton/DropDown/OffsetDropDown
            GUIStyle style = new GUIStyle("DropDownButton");
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        });


        public class Accessor<T>
            where T : class
        {
            private T _value;
            private Func<T> _creator;

            public Accessor(Func<T> creator)
            {
                this._creator = creator;
            }

            public T GetValue()
            {
                if (this._value == null)
                {
                    this._value = this._creator();
                }
                return this._value;
            }

            public static implicit operator T(Accessor<T> v)
            {
                if (v == null)
                    return default;
                return v.GetValue();
            }
        }
    }
}