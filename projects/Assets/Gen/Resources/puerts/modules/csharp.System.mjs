
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


export default csharp.System;


export const Object = __proxy__(() => csharp.System.Object);
export const Delegate = __proxy__(() => csharp.System.Delegate);
export const ICloneable = __proxy__(() => csharp.System.ICloneable);
export const MulticastDelegate = __proxy__(() => csharp.System.MulticastDelegate);
export const ValueType = __proxy__(() => csharp.System.ValueType);
export const Void = __proxy__(() => csharp.System.Void);
export const IAsyncResult = __proxy__(() => csharp.System.IAsyncResult);
export const AsyncCallback = __proxy__(() => csharp.System.AsyncCallback);
export const IntPtr = __proxy__(() => csharp.System.IntPtr);
export const Exception = __proxy__(() => csharp.System.Exception);
export const String = __proxy__(() => csharp.System.String);
export const IComparable = __proxy__(() => csharp.System.IComparable);
export const IComparable$1 = __proxy__(() => csharp.System.IComparable$1);
export const IConvertible = __proxy__(() => csharp.System.IConvertible);
export const IEquatable$1 = __proxy__(() => csharp.System.IEquatable$1);
export const Char = __proxy__(() => csharp.System.Char);
export const IDisposable = __proxy__(() => csharp.System.IDisposable);
export const Array = __proxy__(() => csharp.System.Array);
export const Boolean = __proxy__(() => csharp.System.Boolean);
export const SByte = __proxy__(() => csharp.System.SByte);
export const IFormattable = __proxy__(() => csharp.System.IFormattable);
export const Int16 = __proxy__(() => csharp.System.Int16);
export const Int32 = __proxy__(() => csharp.System.Int32);
export const Int64 = __proxy__(() => csharp.System.Int64);
export const Single = __proxy__(() => csharp.System.Single);
export const Double = __proxy__(() => csharp.System.Double);
export const Byte = __proxy__(() => csharp.System.Byte);
export const Enum = __proxy__(() => csharp.System.Enum);
export const Attribute = __proxy__(() => csharp.System.Attribute);
export const Type = __proxy__(() => csharp.System.Type);
export const UInt32 = __proxy__(() => csharp.System.UInt32);
export const UInt64 = __proxy__(() => csharp.System.UInt64);
export const MarshalByRefObject = __proxy__(() => csharp.System.MarshalByRefObject);
export const Action$1 = __proxy__(() => csharp.System.Action$1);
export const Nullable$1 = __proxy__(() => csharp.System.Nullable$1);
export const Func$1 = __proxy__(() => csharp.System.Func$1);
export const Action = __proxy__(() => csharp.System.Action);
export const Action$2 = __proxy__(() => csharp.System.Action$2);
export const DateTime = __proxy__(() => csharp.System.DateTime);
export const UInt16 = __proxy__(() => csharp.System.UInt16);
export const SystemException = __proxy__(() => csharp.System.SystemException);
export const Action$3 = __proxy__(() => csharp.System.Action$3);
export const Func$2 = __proxy__(() => csharp.System.Func$2);
export const Action$4 = __proxy__(() => csharp.System.Action$4);
export const Func$3 = __proxy__(() => csharp.System.Func$3);
export const Converter$2 = __proxy__(() => csharp.System.Converter$2);
export const Comparison$1 = __proxy__(() => csharp.System.Comparison$1);
export const Predicate$1 = __proxy__(() => csharp.System.Predicate$1);
export const Guid = __proxy__(() => csharp.System.Guid);
export const IFormatProvider = __proxy__(() => csharp.System.IFormatProvider);
export const TimeSpan = __proxy__(() => csharp.System.TimeSpan);
export const DateTimeKind = __proxy__(() => csharp.System.DateTimeKind);
export const DayOfWeek = __proxy__(() => csharp.System.DayOfWeek);
export const TypeCode = __proxy__(() => csharp.System.TypeCode);
export const Func$4 = __proxy__(() => csharp.System.Func$4);
export const RuntimeTypeHandle = __proxy__(() => csharp.System.RuntimeTypeHandle);
export const RuntimeMethodHandle = __proxy__(() => csharp.System.RuntimeMethodHandle);
export const RuntimeFieldHandle = __proxy__(() => csharp.System.RuntimeFieldHandle);
export const TypedReference = __proxy__(() => csharp.System.TypedReference);
export const Span$1 = __proxy__(() => csharp.System.Span$1);
export const ReadOnlySpan$1 = __proxy__(() => csharp.System.ReadOnlySpan$1);
export const Memory$1 = __proxy__(() => csharp.System.Memory$1);
export const ReadOnlyMemory$1 = __proxy__(() => csharp.System.ReadOnlyMemory$1);
export const StringComparison = __proxy__(() => csharp.System.StringComparison);
export const StringSplitOptions = __proxy__(() => csharp.System.StringSplitOptions);
export const CharEnumerator = __proxy__(() => csharp.System.CharEnumerator);


export const Collections = __proxy__(() => csharp.System.Collections);
export const Reflection = __proxy__(() => csharp.System.Reflection);
export const IO = __proxy__(() => csharp.System.IO);
export const Security = __proxy__(() => csharp.System.Security);
export const Text = __proxy__(() => csharp.System.Text);
export const Threading = __proxy__(() => csharp.System.Threading);
export const Globalization = __proxy__(() => csharp.System.Globalization);
export const Runtime = __proxy__(() => csharp.System.Runtime);
