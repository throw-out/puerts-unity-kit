
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


export default csharp;


export const Sample_02 = __proxy__(() => csharp.Sample_02);
export const Sample_03 = __proxy__(() => csharp.Sample_03);
export const Sample_04 = __proxy__(() => csharp.Sample_04);
export const Sample_05 = __proxy__(() => csharp.Sample_05);
export const Sample_10 = __proxy__(() => csharp.Sample_10);
export const Starter = __proxy__(() => csharp.Starter);
export const MyData = __proxy__(() => csharp.MyData);


export const System = __proxy__(() => csharp.System);
export const UnityEngine = __proxy__(() => csharp.UnityEngine);
export const Puerts = __proxy__(() => csharp.Puerts);
export const XOR = __proxy__(() => csharp.XOR);
export const PuertsBridge = __proxy__(() => csharp.PuertsBridge);
export const Unity = __proxy__(() => csharp.Unity);
