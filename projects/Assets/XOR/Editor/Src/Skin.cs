using System;
using UnityEngine;

namespace XOR
{
    internal static class Skin
    {
        //HeaderButton      亮
        //GroupBox          暗
        //HelpBox           暗

        internal static readonly Accessor<GUIStyle> headerBox = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle("HeaderButton");
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        });
        internal static readonly Accessor<GUIStyle> groupBox = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle("GroupBox");
            style.padding = new RectOffset();
            style.margin = new RectOffset();
            return style;
        });
        internal static readonly Accessor<GUIStyle> label = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            style.richText = true;
            return style;
        });
        internal static readonly Accessor<GUIStyle> labelBold = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            return style;
        });

        internal static readonly Accessor<GUIStyle> labelBoldGreen = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.green;
            return style;
        });
        internal static readonly Accessor<GUIStyle> labelBoldYellow = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.yellow;
            return style;
        });
        internal static readonly Accessor<GUIStyle> labelBoldGray = new Accessor<GUIStyle>(() =>
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.gray;
            return style;
        });


        internal static readonly Accessor<GUIStyle> ErrorIcon = new Accessor<GUIStyle>(() =>
        {
            //Wizard Error
            return null;
        });

        /// <summary>红色圆 </summary>
        internal static readonly Accessor<GUIStyle> WinBtnCloseMac = new Accessor<GUIStyle>(() =>
        {
            return "WinBtnCloseMac";
        });
        /// <summary>灰色圆 </summary>
        internal static readonly Accessor<GUIStyle> WinBtnInactiveMac = new Accessor<GUIStyle>(() =>
        {
            return "WinBtnInactiveMac";
        });
        /// <summary>绿色圆 </summary>
        internal static readonly Accessor<GUIStyle> WinBtnMaxMac = new Accessor<GUIStyle>(() =>
        {
            return "WinBtnMaxMac";
        });
        /// <summary>黄色圆 </summary>
        internal static readonly Accessor<GUIStyle> WinBtnMinMac = new Accessor<GUIStyle>(() =>
        {
            return "WinBtnMinMac";
        });


        internal class Accessor<T>
            where T : new()
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