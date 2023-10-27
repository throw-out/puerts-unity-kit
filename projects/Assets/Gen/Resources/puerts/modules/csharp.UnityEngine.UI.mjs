
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

//导出名称为Object的类可能与全局域中的Object冲突, 此处生成别名在末尾再一次性导出

const $AnimationTriggers = __proxy__(() => csharp.UnityEngine.UI.AnimationTriggers);
const $Selectable = __proxy__(() => csharp.UnityEngine.UI.Selectable);
const $Button = __proxy__(() => csharp.UnityEngine.UI.Button);
const $CanvasUpdate = __proxy__(() => csharp.UnityEngine.UI.CanvasUpdate);
const $ICanvasElement = __proxy__(() => csharp.UnityEngine.UI.ICanvasElement);
const $CanvasUpdateRegistry = __proxy__(() => csharp.UnityEngine.UI.CanvasUpdateRegistry);
const $ColorBlock = __proxy__(() => csharp.UnityEngine.UI.ColorBlock);
const $ClipperRegistry = __proxy__(() => csharp.UnityEngine.UI.ClipperRegistry);
const $IClipper = __proxy__(() => csharp.UnityEngine.UI.IClipper);
const $Clipping = __proxy__(() => csharp.UnityEngine.UI.Clipping);
const $RectMask2D = __proxy__(() => csharp.UnityEngine.UI.RectMask2D);
const $IClippable = __proxy__(() => csharp.UnityEngine.UI.IClippable);
const $Dropdown = __proxy__(() => csharp.UnityEngine.UI.Dropdown);
const $Graphic = __proxy__(() => csharp.UnityEngine.UI.Graphic);
const $MaskableGraphic = __proxy__(() => csharp.UnityEngine.UI.MaskableGraphic);
const $IMaterialModifier = __proxy__(() => csharp.UnityEngine.UI.IMaterialModifier);
const $IMaskable = __proxy__(() => csharp.UnityEngine.UI.IMaskable);
const $Text = __proxy__(() => csharp.UnityEngine.UI.Text);
const $ILayoutElement = __proxy__(() => csharp.UnityEngine.UI.ILayoutElement);
const $Image = __proxy__(() => csharp.UnityEngine.UI.Image);
const $FontData = __proxy__(() => csharp.UnityEngine.UI.FontData);
const $FontUpdateTracker = __proxy__(() => csharp.UnityEngine.UI.FontUpdateTracker);
const $GraphicRaycaster = __proxy__(() => csharp.UnityEngine.UI.GraphicRaycaster);
const $GraphicRegistry = __proxy__(() => csharp.UnityEngine.UI.GraphicRegistry);
const $InputField = __proxy__(() => csharp.UnityEngine.UI.InputField);
const $AspectRatioFitter = __proxy__(() => csharp.UnityEngine.UI.AspectRatioFitter);
const $ILayoutSelfController = __proxy__(() => csharp.UnityEngine.UI.ILayoutSelfController);
const $ILayoutController = __proxy__(() => csharp.UnityEngine.UI.ILayoutController);
const $CanvasScaler = __proxy__(() => csharp.UnityEngine.UI.CanvasScaler);
const $ContentSizeFitter = __proxy__(() => csharp.UnityEngine.UI.ContentSizeFitter);
const $LayoutGroup = __proxy__(() => csharp.UnityEngine.UI.LayoutGroup);
const $ILayoutGroup = __proxy__(() => csharp.UnityEngine.UI.ILayoutGroup);
const $GridLayoutGroup = __proxy__(() => csharp.UnityEngine.UI.GridLayoutGroup);
const $HorizontalOrVerticalLayoutGroup = __proxy__(() => csharp.UnityEngine.UI.HorizontalOrVerticalLayoutGroup);
const $HorizontalLayoutGroup = __proxy__(() => csharp.UnityEngine.UI.HorizontalLayoutGroup);
const $ILayoutIgnorer = __proxy__(() => csharp.UnityEngine.UI.ILayoutIgnorer);
const $LayoutElement = __proxy__(() => csharp.UnityEngine.UI.LayoutElement);
const $LayoutRebuilder = __proxy__(() => csharp.UnityEngine.UI.LayoutRebuilder);
const $LayoutUtility = __proxy__(() => csharp.UnityEngine.UI.LayoutUtility);
const $VerticalLayoutGroup = __proxy__(() => csharp.UnityEngine.UI.VerticalLayoutGroup);
const $Mask = __proxy__(() => csharp.UnityEngine.UI.Mask);
const $MaskUtilities = __proxy__(() => csharp.UnityEngine.UI.MaskUtilities);
const $Navigation = __proxy__(() => csharp.UnityEngine.UI.Navigation);
const $RawImage = __proxy__(() => csharp.UnityEngine.UI.RawImage);
const $ScrollRect = __proxy__(() => csharp.UnityEngine.UI.ScrollRect);
const $Scrollbar = __proxy__(() => csharp.UnityEngine.UI.Scrollbar);
const $SpriteState = __proxy__(() => csharp.UnityEngine.UI.SpriteState);
const $Slider = __proxy__(() => csharp.UnityEngine.UI.Slider);
const $StencilMaterial = __proxy__(() => csharp.UnityEngine.UI.StencilMaterial);
const $Toggle = __proxy__(() => csharp.UnityEngine.UI.Toggle);
const $ToggleGroup = __proxy__(() => csharp.UnityEngine.UI.ToggleGroup);
const $VertexHelper = __proxy__(() => csharp.UnityEngine.UI.VertexHelper);
const $BaseMeshEffect = __proxy__(() => csharp.UnityEngine.UI.BaseMeshEffect);
const $IMeshModifier = __proxy__(() => csharp.UnityEngine.UI.IMeshModifier);
const $Shadow = __proxy__(() => csharp.UnityEngine.UI.Shadow);
const $Outline = __proxy__(() => csharp.UnityEngine.UI.Outline);
const $PositionAsUV1 = __proxy__(() => csharp.UnityEngine.UI.PositionAsUV1);

const $DefaultControls = __proxy__(() => csharp.UnityEngine.UI.DefaultControls);

export {

    $AnimationTriggers as AnimationTriggers,
    $Selectable as Selectable,
    $Button as Button,
    $CanvasUpdate as CanvasUpdate,
    $ICanvasElement as ICanvasElement,
    $CanvasUpdateRegistry as CanvasUpdateRegistry,
    $ColorBlock as ColorBlock,
    $ClipperRegistry as ClipperRegistry,
    $IClipper as IClipper,
    $Clipping as Clipping,
    $RectMask2D as RectMask2D,
    $IClippable as IClippable,
    $Dropdown as Dropdown,
    $Graphic as Graphic,
    $MaskableGraphic as MaskableGraphic,
    $IMaterialModifier as IMaterialModifier,
    $IMaskable as IMaskable,
    $Text as Text,
    $ILayoutElement as ILayoutElement,
    $Image as Image,
    $FontData as FontData,
    $FontUpdateTracker as FontUpdateTracker,
    $GraphicRaycaster as GraphicRaycaster,
    $GraphicRegistry as GraphicRegistry,
    $InputField as InputField,
    $AspectRatioFitter as AspectRatioFitter,
    $ILayoutSelfController as ILayoutSelfController,
    $ILayoutController as ILayoutController,
    $CanvasScaler as CanvasScaler,
    $ContentSizeFitter as ContentSizeFitter,
    $LayoutGroup as LayoutGroup,
    $ILayoutGroup as ILayoutGroup,
    $GridLayoutGroup as GridLayoutGroup,
    $HorizontalOrVerticalLayoutGroup as HorizontalOrVerticalLayoutGroup,
    $HorizontalLayoutGroup as HorizontalLayoutGroup,
    $ILayoutIgnorer as ILayoutIgnorer,
    $LayoutElement as LayoutElement,
    $LayoutRebuilder as LayoutRebuilder,
    $LayoutUtility as LayoutUtility,
    $VerticalLayoutGroup as VerticalLayoutGroup,
    $Mask as Mask,
    $MaskUtilities as MaskUtilities,
    $Navigation as Navigation,
    $RawImage as RawImage,
    $ScrollRect as ScrollRect,
    $Scrollbar as Scrollbar,
    $SpriteState as SpriteState,
    $Slider as Slider,
    $StencilMaterial as StencilMaterial,
    $Toggle as Toggle,
    $ToggleGroup as ToggleGroup,
    $VertexHelper as VertexHelper,
    $BaseMeshEffect as BaseMeshEffect,
    $IMeshModifier as IMeshModifier,
    $Shadow as Shadow,
    $Outline as Outline,
    $PositionAsUV1 as PositionAsUV1,

    $DefaultControls as DefaultControls,
}

