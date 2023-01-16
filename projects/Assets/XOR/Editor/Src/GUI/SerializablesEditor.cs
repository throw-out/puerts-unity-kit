using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{
    internal abstract class Drawer
    {
#if UNITY_2019_1_OR_NEWER
        protected const string OPTIONS_STYLE = "PaneOptions";
#else
        protected const string OPTIONS_STYLE = "Icon.Options";
#endif
        protected const float HEIGHT_SPACING = HEIGHT_SPACING_HALF * 2;
        protected const float HEIGHT_SPACING_HALF = 2f;
        protected const float OPTIONS_WIDTH = 16f;
        protected const float QUAD_WIDTH = OPTIONS_WIDTH + 4f;

    }
}
