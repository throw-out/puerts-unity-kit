
const csharp = (function () {
    let _g = this || global || globalThis;
    return _g['CS'] || _g['csharp'] || require('csharp');
})();


function __proxy__(getter) {
    let target;
    function tryload() {
        if (!getter) return;
        target = getter();
        getter = null;
    };
    return new Proxy(tryload, {
        apply: function (_, thisArg, argArray) {
            tryload();
            target.apply(thisArg, argArray);
        },
        construct: function (_, argArray, newTarget) {
            tryload();
            return new target(...argArray);
        },
        get: function (_, property) {
            tryload();
            return target[property];
        },
        set: function (_, property, newValue) {
            tryload();
            target[property] = newValue;
            return true;
        },
        defineProperty: function (_, property, attributes) {
            tryload();
            Object.defineProperty(target, property, attributes);
            return true;
        },
        deleteProperty: function (_, property) {
            tryload();
            delete target[property];
            return true;
        },
        getOwnPropertyDescriptor: function (_, property) {
            tryload();
            return Object.getOwnPropertyDescriptor(target, property);
        },
        getPrototypeOf: function (_) {
            tryload();
            return Object.getPrototypeOf(target);
        },
        setPrototypeOf: function (_, newValue) {
            tryload();
            Object.setPrototypeOf(target, newValue);
            return true;
        },
        has: function (_, property) {
            tryload();
            return property in target;
        },
        isExtensible: function (_) {
            tryload();
            return Object.isExtensible(target);
        },
        ownKeys: function (_) {
            tryload();
            return Reflect.ownKeys(target)?.filter(key => Object.getOwnPropertyDescriptor(target, key)?.configurable);
        },
        preventExtensions: function (_) {
            tryload();
            Object.preventExtensions(target);
            return true;
        },

    });
}


export default csharp.UnityEngine.UI;


export const AnimationTriggers = __proxy__(() => csharp.UnityEngine.UI.AnimationTriggers);
export const Selectable = __proxy__(() => csharp.UnityEngine.UI.Selectable);
export const Button = __proxy__(() => csharp.UnityEngine.UI.Button);
export const CanvasUpdate = __proxy__(() => csharp.UnityEngine.UI.CanvasUpdate);
export const ICanvasElement = __proxy__(() => csharp.UnityEngine.UI.ICanvasElement);
export const CanvasUpdateRegistry = __proxy__(() => csharp.UnityEngine.UI.CanvasUpdateRegistry);
export const ColorBlock = __proxy__(() => csharp.UnityEngine.UI.ColorBlock);
export const ClipperRegistry = __proxy__(() => csharp.UnityEngine.UI.ClipperRegistry);
export const IClipper = __proxy__(() => csharp.UnityEngine.UI.IClipper);
export const Clipping = __proxy__(() => csharp.UnityEngine.UI.Clipping);
export const RectMask2D = __proxy__(() => csharp.UnityEngine.UI.RectMask2D);
export const IClippable = __proxy__(() => csharp.UnityEngine.UI.IClippable);
export const Dropdown = __proxy__(() => csharp.UnityEngine.UI.Dropdown);
export const Graphic = __proxy__(() => csharp.UnityEngine.UI.Graphic);
export const MaskableGraphic = __proxy__(() => csharp.UnityEngine.UI.MaskableGraphic);
export const IMaterialModifier = __proxy__(() => csharp.UnityEngine.UI.IMaterialModifier);
export const IMaskable = __proxy__(() => csharp.UnityEngine.UI.IMaskable);
export const Text = __proxy__(() => csharp.UnityEngine.UI.Text);
export const ILayoutElement = __proxy__(() => csharp.UnityEngine.UI.ILayoutElement);
export const Image = __proxy__(() => csharp.UnityEngine.UI.Image);
export const FontData = __proxy__(() => csharp.UnityEngine.UI.FontData);
export const FontUpdateTracker = __proxy__(() => csharp.UnityEngine.UI.FontUpdateTracker);
export const GraphicRaycaster = __proxy__(() => csharp.UnityEngine.UI.GraphicRaycaster);
export const GraphicRegistry = __proxy__(() => csharp.UnityEngine.UI.GraphicRegistry);
export const InputField = __proxy__(() => csharp.UnityEngine.UI.InputField);
export const AspectRatioFitter = __proxy__(() => csharp.UnityEngine.UI.AspectRatioFitter);
export const ILayoutSelfController = __proxy__(() => csharp.UnityEngine.UI.ILayoutSelfController);
export const ILayoutController = __proxy__(() => csharp.UnityEngine.UI.ILayoutController);
export const CanvasScaler = __proxy__(() => csharp.UnityEngine.UI.CanvasScaler);
export const ContentSizeFitter = __proxy__(() => csharp.UnityEngine.UI.ContentSizeFitter);
export const LayoutGroup = __proxy__(() => csharp.UnityEngine.UI.LayoutGroup);
export const ILayoutGroup = __proxy__(() => csharp.UnityEngine.UI.ILayoutGroup);
export const GridLayoutGroup = __proxy__(() => csharp.UnityEngine.UI.GridLayoutGroup);
export const HorizontalOrVerticalLayoutGroup = __proxy__(() => csharp.UnityEngine.UI.HorizontalOrVerticalLayoutGroup);
export const HorizontalLayoutGroup = __proxy__(() => csharp.UnityEngine.UI.HorizontalLayoutGroup);
export const ILayoutIgnorer = __proxy__(() => csharp.UnityEngine.UI.ILayoutIgnorer);
export const LayoutElement = __proxy__(() => csharp.UnityEngine.UI.LayoutElement);
export const LayoutRebuilder = __proxy__(() => csharp.UnityEngine.UI.LayoutRebuilder);
export const LayoutUtility = __proxy__(() => csharp.UnityEngine.UI.LayoutUtility);
export const VerticalLayoutGroup = __proxy__(() => csharp.UnityEngine.UI.VerticalLayoutGroup);
export const Mask = __proxy__(() => csharp.UnityEngine.UI.Mask);
export const MaskUtilities = __proxy__(() => csharp.UnityEngine.UI.MaskUtilities);
export const Navigation = __proxy__(() => csharp.UnityEngine.UI.Navigation);
export const RawImage = __proxy__(() => csharp.UnityEngine.UI.RawImage);
export const ScrollRect = __proxy__(() => csharp.UnityEngine.UI.ScrollRect);
export const Scrollbar = __proxy__(() => csharp.UnityEngine.UI.Scrollbar);
export const SpriteState = __proxy__(() => csharp.UnityEngine.UI.SpriteState);
export const Slider = __proxy__(() => csharp.UnityEngine.UI.Slider);
export const StencilMaterial = __proxy__(() => csharp.UnityEngine.UI.StencilMaterial);
export const Toggle = __proxy__(() => csharp.UnityEngine.UI.Toggle);
export const ToggleGroup = __proxy__(() => csharp.UnityEngine.UI.ToggleGroup);
export const VertexHelper = __proxy__(() => csharp.UnityEngine.UI.VertexHelper);
export const BaseMeshEffect = __proxy__(() => csharp.UnityEngine.UI.BaseMeshEffect);
export const IMeshModifier = __proxy__(() => csharp.UnityEngine.UI.IMeshModifier);
export const Shadow = __proxy__(() => csharp.UnityEngine.UI.Shadow);
export const Outline = __proxy__(() => csharp.UnityEngine.UI.Outline);
export const PositionAsUV1 = __proxy__(() => csharp.UnityEngine.UI.PositionAsUV1);


export const DefaultControls = __proxy__(() => csharp.UnityEngine.UI.DefaultControls);
