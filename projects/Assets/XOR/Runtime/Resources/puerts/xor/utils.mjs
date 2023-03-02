export function get(obj, key) {
    let value = obj[key];
    if (typeof (value) === "function") {
        value = value.bind(obj);
    }
    return value;
}
export function set(obj, key, value) {
    obj[key] = value;
}
export function getInPath(obj, key) {
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
export function setInPath(obj, key, value) {
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
export function containsKey(obj, key) {
    return key in obj;
}
export function removeKey(obj, key) {
    delete obj[key];
}
export function getKeys(obj) {
    let keys = Object.keys(obj);
    let result = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.String), keys.length);
    for (let i = 0; i < keys.length; i++) {
        result.set_Item(i, keys[i]);
    }
    return result;
}
export function length(obj) {
    if (Array.isArray(obj)) {
        return obj.length;
    }
    return -1;
}
export function forEach(obj, action) {
    if (action instanceof CS.System.Delegate && typeof (action) !== "function") {
        action = action["Invoke"].bind(action);
    }
    for (let key in obj) {
        action(key, obj[key]);
    }
}
export function call(obj, methodName, args) {
    let func = obj[methodName];
    if (typeof (func) !== "function")
        throw new Error();
    let _args = [];
    for (let i = 0; i < args.Length; i++) {
        _args.push(args.get_Item(i));
    }
    return func.apply(obj, _args);
}
export function cast(obj) {
    return obj;
}
//# sourceMappingURL=utils.js.map