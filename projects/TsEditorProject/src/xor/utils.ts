export function get(obj: object, key: any) {
    let value = obj[key];
    if (typeof (value) === "function") {
        value = value.bind(obj);
    }
    return value;
}
export function set(obj: object, key: any, value: any) {
    obj[key] = value;
}
export function getInPath(obj: object, key: string) {
    let target = obj;
    let paths = key.split("."), lastIndex = paths.length - 1;
    for (let i = 0; i < lastIndex; i++) {
        if (!target) {
            return null;
        }
        target = target[paths[i]];
    }
    let value = target[paths[lastIndex]];
    if (typeof (value) === "function") {
        value = value.bind(target);
    }
    return value;
}
export function setInPath(obj: object, key: string, value: any) {
    let target = obj;
    let paths = key.split("."), lastIndex = paths.length - 1;
    for (let i = 0; i < lastIndex; i++) {
        if (!target) {
            return null;
        }
        target = target[paths[i]];
    }
    target[paths[lastIndex]] = value;
}
export function containsKey(obj: object, key: any) {
    return key in obj;
}
export function removeKey(obj: object, key: any) {
    delete obj[key];
}
export function getKeys(obj: object) {
    let keys = Object.keys(obj);
    let result = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.String), keys.length) as CS.System.Array$1<string>;
    for (let i = 0; i < keys.length; i++) {
        result.set_Item(i, keys[i]);
    }
    return result;
}
export function length(obj: object) {
    if (Array.isArray(obj)) {
        return obj.length;
    }
    return -1;
}
export function forEach(obj: object, action: CS.System.Delegate & ((key: any, value: any) => void)) {
    if (action instanceof CS.System.Delegate && typeof (action) !== "function") {
        action = (<any>action)["Invoke"].bind(action);
    }
    for (let key in obj) {
        action(key, obj[key]);
    }
}
export function call(obj: object, methodName: string, args: CS.System.Array$1<any>) {
    let func = obj[methodName];
    if (typeof (func) !== "function")
        throw new Error();
    let _args = [];
    for (let i = 0; i < args.Length; i++) {
        _args.push(args.get_Item(i));
    }
    return func.apply(obj, _args);
}
export function cast(obj: object) {
    return obj;
}