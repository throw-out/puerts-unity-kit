
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

//导出名称为Object的类可能与全局域中的Object冲突, 此处生成别名在末尾再一次性导出

const $Object = __proxy__(() => csharp.System.Object);
const $Delegate = __proxy__(() => csharp.System.Delegate);
const $ICloneable = __proxy__(() => csharp.System.ICloneable);
const $MulticastDelegate = __proxy__(() => csharp.System.MulticastDelegate);
const $ValueType = __proxy__(() => csharp.System.ValueType);
const $Void = __proxy__(() => csharp.System.Void);
const $IAsyncResult = __proxy__(() => csharp.System.IAsyncResult);
const $AsyncCallback = __proxy__(() => csharp.System.AsyncCallback);
const $IntPtr = __proxy__(() => csharp.System.IntPtr);
const $Exception = __proxy__(() => csharp.System.Exception);
const $String = __proxy__(() => csharp.System.String);
const $IComparable = __proxy__(() => csharp.System.IComparable);
const $IComparable$1 = __proxy__(() => csharp.System.IComparable$1);
const $IConvertible = __proxy__(() => csharp.System.IConvertible);
const $IEquatable$1 = __proxy__(() => csharp.System.IEquatable$1);
const $Char = __proxy__(() => csharp.System.Char);
const $IDisposable = __proxy__(() => csharp.System.IDisposable);
const $Array = __proxy__(() => csharp.System.Array);
const $Boolean = __proxy__(() => csharp.System.Boolean);
const $SByte = __proxy__(() => csharp.System.SByte);
const $IFormattable = __proxy__(() => csharp.System.IFormattable);
const $Int16 = __proxy__(() => csharp.System.Int16);
const $Int32 = __proxy__(() => csharp.System.Int32);
const $Int64 = __proxy__(() => csharp.System.Int64);
const $Single = __proxy__(() => csharp.System.Single);
const $Double = __proxy__(() => csharp.System.Double);
const $Byte = __proxy__(() => csharp.System.Byte);
const $Enum = __proxy__(() => csharp.System.Enum);
const $Attribute = __proxy__(() => csharp.System.Attribute);
const $Type = __proxy__(() => csharp.System.Type);
const $UInt32 = __proxy__(() => csharp.System.UInt32);
const $UInt64 = __proxy__(() => csharp.System.UInt64);
const $MarshalByRefObject = __proxy__(() => csharp.System.MarshalByRefObject);
const $Action$1 = __proxy__(() => csharp.System.Action$1);
const $Nullable$1 = __proxy__(() => csharp.System.Nullable$1);
const $Func$1 = __proxy__(() => csharp.System.Func$1);
const $Action = __proxy__(() => csharp.System.Action);
const $Action$2 = __proxy__(() => csharp.System.Action$2);
const $DateTime = __proxy__(() => csharp.System.DateTime);
const $UInt16 = __proxy__(() => csharp.System.UInt16);
const $SystemException = __proxy__(() => csharp.System.SystemException);
const $Action$3 = __proxy__(() => csharp.System.Action$3);
const $Func$2 = __proxy__(() => csharp.System.Func$2);
const $Action$4 = __proxy__(() => csharp.System.Action$4);
const $Func$3 = __proxy__(() => csharp.System.Func$3);
const $Converter$2 = __proxy__(() => csharp.System.Converter$2);
const $Comparison$1 = __proxy__(() => csharp.System.Comparison$1);
const $Predicate$1 = __proxy__(() => csharp.System.Predicate$1);
const $Guid = __proxy__(() => csharp.System.Guid);
const $IFormatProvider = __proxy__(() => csharp.System.IFormatProvider);
const $TimeSpan = __proxy__(() => csharp.System.TimeSpan);
const $DateTimeKind = __proxy__(() => csharp.System.DateTimeKind);
const $DayOfWeek = __proxy__(() => csharp.System.DayOfWeek);
const $TypeCode = __proxy__(() => csharp.System.TypeCode);
const $Func$4 = __proxy__(() => csharp.System.Func$4);
const $RuntimeTypeHandle = __proxy__(() => csharp.System.RuntimeTypeHandle);
const $RuntimeMethodHandle = __proxy__(() => csharp.System.RuntimeMethodHandle);
const $RuntimeFieldHandle = __proxy__(() => csharp.System.RuntimeFieldHandle);
const $TypedReference = __proxy__(() => csharp.System.TypedReference);
const $Span$1 = __proxy__(() => csharp.System.Span$1);
const $ReadOnlySpan$1 = __proxy__(() => csharp.System.ReadOnlySpan$1);
const $Memory$1 = __proxy__(() => csharp.System.Memory$1);
const $ReadOnlyMemory$1 = __proxy__(() => csharp.System.ReadOnlyMemory$1);
const $StringComparison = __proxy__(() => csharp.System.StringComparison);
const $StringSplitOptions = __proxy__(() => csharp.System.StringSplitOptions);
const $CharEnumerator = __proxy__(() => csharp.System.CharEnumerator);

const $Collections = __proxy__(() => csharp.System.Collections);
const $Reflection = __proxy__(() => csharp.System.Reflection);
const $IO = __proxy__(() => csharp.System.IO);
const $Security = __proxy__(() => csharp.System.Security);
const $Text = __proxy__(() => csharp.System.Text);
const $Threading = __proxy__(() => csharp.System.Threading);
const $Globalization = __proxy__(() => csharp.System.Globalization);
const $Runtime = __proxy__(() => csharp.System.Runtime);

export {

    $Object as Object,
    $Delegate as Delegate,
    $ICloneable as ICloneable,
    $MulticastDelegate as MulticastDelegate,
    $ValueType as ValueType,
    $Void as Void,
    $IAsyncResult as IAsyncResult,
    $AsyncCallback as AsyncCallback,
    $IntPtr as IntPtr,
    $Exception as Exception,
    $String as String,
    $IComparable as IComparable,
    $IComparable$1 as IComparable$1,
    $IConvertible as IConvertible,
    $IEquatable$1 as IEquatable$1,
    $Char as Char,
    $IDisposable as IDisposable,
    $Array as Array,
    $Boolean as Boolean,
    $SByte as SByte,
    $IFormattable as IFormattable,
    $Int16 as Int16,
    $Int32 as Int32,
    $Int64 as Int64,
    $Single as Single,
    $Double as Double,
    $Byte as Byte,
    $Enum as Enum,
    $Attribute as Attribute,
    $Type as Type,
    $UInt32 as UInt32,
    $UInt64 as UInt64,
    $MarshalByRefObject as MarshalByRefObject,
    $Action$1 as Action$1,
    $Nullable$1 as Nullable$1,
    $Func$1 as Func$1,
    $Action as Action,
    $Action$2 as Action$2,
    $DateTime as DateTime,
    $UInt16 as UInt16,
    $SystemException as SystemException,
    $Action$3 as Action$3,
    $Func$2 as Func$2,
    $Action$4 as Action$4,
    $Func$3 as Func$3,
    $Converter$2 as Converter$2,
    $Comparison$1 as Comparison$1,
    $Predicate$1 as Predicate$1,
    $Guid as Guid,
    $IFormatProvider as IFormatProvider,
    $TimeSpan as TimeSpan,
    $DateTimeKind as DateTimeKind,
    $DayOfWeek as DayOfWeek,
    $TypeCode as TypeCode,
    $Func$4 as Func$4,
    $RuntimeTypeHandle as RuntimeTypeHandle,
    $RuntimeMethodHandle as RuntimeMethodHandle,
    $RuntimeFieldHandle as RuntimeFieldHandle,
    $TypedReference as TypedReference,
    $Span$1 as Span$1,
    $ReadOnlySpan$1 as ReadOnlySpan$1,
    $Memory$1 as Memory$1,
    $ReadOnlyMemory$1 as ReadOnlyMemory$1,
    $StringComparison as StringComparison,
    $StringSplitOptions as StringSplitOptions,
    $CharEnumerator as CharEnumerator,

    $Collections as Collections,
    $Reflection as Reflection,
    $IO as IO,
    $Security as Security,
    $Text as Text,
    $Threading as Threading,
    $Globalization as Globalization,
    $Runtime as Runtime,
}

