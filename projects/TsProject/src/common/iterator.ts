import CSObject = CS.System.Object;
import CSArray = CS.System.Array;
import CSArray$1 = CS.System.Array$1;
import CSList$1 = CS.System.Collections.Generic.List$1;
import CSDictionary$2 = CS.System.Collections.Generic.Dictionary$2;
import CSIEnumerator = CS.System.Collections.IEnumerator;
import CSIEnumerator$1 = CS.System.Collections.Generic.IEnumerator$1;

type Array$1Iterator<T> = {
    /**
     * 遍历Array$1对象
     * @param callbackfn 
     */
    forEach(callbackfn: (v: T, index: number) => boolean | void): void;
    /**
     * 实现for迭代器接口
     */
    [Symbol.iterator](): {
        next(): {
            value: T,
            done: boolean
        };
    };
}
function defineArrayIterator(prototype: object) {
    prototype["forEach"] = function (callbackfn: (v: any, k: any) => void | boolean) {
        let length = this.Length;
        for (let i = 0; i < length; i++) {
            if (callbackfn(this.GetValue(i), i) === false)
                break;
        }
    }
    prototype[Symbol.iterator] = function* () {
        let length = this.Length;
        for (let i = 0; i < length; i++) {
            yield this.GetValue(i);
        }
    }
}

type List$1Iterator<T> = {
    /**
     * 遍历List$1对象
     * @param callbackfn 
     */
    forEach(callbackfn: (v: T, index: number) => boolean | void): void;
    /**
     * 实现for迭代器接口
     */
    [Symbol.iterator](): {
        next(): {
            value: T,
            done: boolean
        };
    };
}
function defineListIterator(prototype: object) {
    prototype["forEach"] = function (callbackfn: (v: any, k: any) => void | boolean) {
        let iterator = this.GetEnumerator(), index = 0;
        while (iterator.MoveNext()) {
            if (callbackfn(iterator.Current, index++) === false)
                break;
        }
    }
    prototype[Symbol.iterator] = function* () {
        let iterator = this.GetEnumerator();
        while (iterator.MoveNext()) {
            yield iterator.Current;
        }
    }
}

type Dictionary$2Iterator<TKey, TValue> = {
    /**
     * 遍历Dictionary$2对象
     * @param callbackfn 
     */
    forEach(callbackfn: (v: TValue, k?: TKey) => boolean | void): void;
    /**
     * Key集合
     */
    getKeys(): TKey[];
    /**
     * Value集合
     */
    getValues(): TValue[];
    /**
     * 实现for迭代器接口
     */
    [Symbol.iterator](): {
        next(): {
            value: [key: TKey, value: TValue],
            done: boolean
        };
    };
};
function defineDictionaryIterator(prototype: object) {
    prototype["forEach"] = function (callbackfn: (v: any, k: any) => void | boolean) {
        let iterator = this.Keys.GetEnumerator();
        while (iterator.MoveNext()) {
            let key = iterator.Current;
            if (callbackfn(this.get_Item(key), key) === false)
                break;
        }
    }
    prototype["getKeys"] = function () {
        let result = new Array<string>();
        let iterator = this.Keys.GetEnumerator();
        while (iterator.MoveNext()) {
            result.push(iterator.Current);
        }
        return result;
    }
    prototype["getValues"] = function () {
        let result = new Array<string>();
        let iterator = this.Values.GetEnumerator();
        while (iterator.MoveNext()) {
            result.push(iterator.Current);
        }
        return result;
    }
    prototype[Symbol.iterator] = function* () {
        let iterator = this.Keys.GetEnumerator();
        while (iterator.MoveNext()) {
            let key = iterator.Current;
            yield [key, this.get_Item(key)];
        }
    }
}

type IEnumerator$1Iterator<T> = {
    /**
     * 遍历List$1对象
     * @param callbackfn 
     */
    forEach(callbackfn: (v: T, index: number) => boolean | void): void;
    /**
     * 实现for迭代器接口
     */
    [Symbol.iterator](): {
        next(): {
            value: T,
            done: boolean
        };
    };
}
type IEnumeratorType<T> =
    { GetEnumerator(): CSIEnumerator$1<T>; } |
    { GetEnumerator(): CSIEnumerator; };
function defineIEnumeratorIterator(prototype: object) {
    prototype["forEach"] = function (callbackfn: (v: any, k: any) => void | boolean) {
        let iterator = this.GetEnumerator(), index = 0;
        while (iterator.MoveNext()) {
            if (callbackfn(iterator.Current, index++) === false)
                break;
        }
    }
    prototype[Symbol.iterator] = function* () {
        let iterator = this.GetEnumerator();
        while (iterator.MoveNext()) {
            yield iterator.Current;
        }
    }
}

const ITERATOR_DEFINE = Symbol(`ITERATOR_DEFINE`);

/**迭代System.Array.Array$1对象
 * @example
 * ```
 * let obj: System.Array$1<number>;
 * let objIterator = iterator(obj);
 * let jsArray = [...objIterator];              //number[]
 * ```
 */
export function iterator<T = any>(instance: CSArray$1<T>): CSArray$1<T> & Array$1Iterator<T>;
export function iterator(instance: CSArray): CSArray & Array$1Iterator<any>;                    //排在Array$1<T>参数声明之后, 降低Array参数匹配优先级
/**迭代System.Collections.Generic.List$1对象
 * @example
 * ```
 * let obj: System.Collections.Generic.List$1<number>;
 * let objIterator = iterator(obj);
 * let jsArray = [...objIterator];              //number[]
 * ```
 */
export function iterator<T = any>(instance: CSList$1<T>): CSList$1<T> & List$1Iterator<T>;
/**迭代System.Collections.Generic.Dictionary$2对象
 * @example
 * ``` 
 * let obj: System.Collections.Generic.Dictionary$2<number, string>;
 * let objIterator = iterator(obj);
 * let keys = objIterator.getKeys();            //number[]
 * let values = objIterator.getValues();        //string[]
 * let keyValuePairs = [...objIterator];        //Array<[key: number, value: string ]>
 * ```
 */
export function iterator<TKey = any, TValue = any>(instance: CSDictionary$2<TKey, TValue>): CSDictionary$2<TKey, TValue> & Dictionary$2Iterator<TKey, TValue>;
/**迭代System.Collections.IEnumerable接口实现
 * @example
 * ```
 * let obj: csharp.System.Collections.Generic.HashSet$1<number>;
 * let objIterator = iterator(obj);
 * let jsArray = [...objIterator];     //number[]
 * ```
 */
export function iterator<T extends CSObject & IEnumeratorType<TValue>, TValue = any>(instance: T): T & IEnumerator$1Iterator<TValue>;
//方法实现
export function iterator(): object {
    const instance = arguments[0] as CSObject;
    if (typeof instance !== "object") {
        return instance;
    }
    if (!(instance instanceof CSObject)) {
        throw new Error(`invalid parameter. Need a chsarp object`);
    }

    const prototype = Object.getPrototypeOf(instance);
    if (!(ITERATOR_DEFINE in prototype)) {
        const Type = instance.GetType();
        if (puer.$typeof(CSArray).IsAssignableFrom(Type)) {
            defineArrayIterator(prototype);
        }
        else {
            let type = Type, define = false;
            while (type && !define) {
                let fullname = type.FullName;
                if (!fullname) break;

                if (fullname.startsWith("System.Collections.Generic.List`1[")) {
                    defineListIterator(prototype);
                    define = true;
                }
                else if (fullname.startsWith("System.Collections.Generic.Dictionary`2[")) {
                    defineDictionaryIterator(prototype);
                    define = true;
                }
                type = type.BaseType;
            }
            if (!define) {
                if ("GetEnumerator" in instance && typeof instance["GetEnumerator"] === "function") {
                    defineIEnumeratorIterator(prototype);
                } else {
                    throw new Error(`unsupported chsarp type: ${Type.FullName}`);
                }
            }
        }
        prototype[ITERATOR_DEFINE] = true;
    }
    return instance;
}