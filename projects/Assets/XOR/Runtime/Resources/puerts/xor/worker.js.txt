let List_Object = puer.$generic(CS.System.Collections.Generic.List$1, CS.System.Object);
const INVOKE_TICK = Symbol("INVOKE_TICK");
const CLOSE_EVENT = "__e_close__", REMOTE_EVENT = "__e_remote__", RESULT_EVENT = "__e_result__";
/**
 * 跨JsEnv实例交互封装
 */
class ThreadWorkerConstructor {
    /**线程是否正在工作 */
    get isAlive() { return this.worker.IsAlive; }
    /**线程是否已初始化完成 */
    get isInitialized() { return this.worker.IsInitialized; }
    get source() { return this.worker; }
    constructor(loader, options) {
        if (loader instanceof CS.XOR.ThreadWorker) {
            this.worker = loader;
            this.mainThread = false;
        }
        else {
            this.worker = CS.XOR.ThreadWorker.Create(loader, options ?? new CS.XOR.ThreadOptions());
            this.mainThread = true;
        }
        CS.XOR.ThreadWorker.VerifyThread(this.mainThread);
        this.events = new Map();
        this.register();
    }
    start(filepath, isESM) {
        if (!this.mainThread || xor.globalWorker && xor.globalWorker.worker === this.worker)
            throw new Error("Invalid operation ");
        this.worker.Run(filepath, !!isESM);
    }
    stop() {
        if (this.mainThread) {
            this.events.clear();
            this.worker.Dispose();
        }
        else {
            this.post(CLOSE_EVENT);
        }
    }
    /**异步调用事件, 无返回值
     * @param eventName
     * @param data
     * @param notResult 不获取返回值
     */
    post(eventName, data, notResult) {
        let edata;
        if (data !== undefined && data !== null && data !== void 0) {
            edata = this.pack(data);
        }
        let resultId = !notResult ? this._getResultId() : null;
        if (this.mainThread) {
            this.worker.PostToChildThread(eventName, edata, resultId);
        }
        else {
            this.worker.PostToMainThread(eventName, edata, resultId);
        }
        if (resultId) {
            return new Promise((resolve, reject) => {
                this.once(resultId, function (d) {
                    if (d.error)
                        reject(d.error);
                    else
                        resolve(d.result);
                });
            });
        }
    }
    /**同步调用事件, 并立即获取返回值
     * @param eventName
     * @param data
     * @param throwOnError
     * @returns
     */
    postSync(eventName, data, throwOnError = true) {
        let edata;
        if (data !== undefined && data !== null && data !== void 0) {
            edata = this.pack(data);
        }
        let result;
        if (this.mainThread) {
            edata = this.worker.Syncr.PostToChildThread(eventName, edata, throwOnError);
        }
        else {
            edata = this.worker.Syncr.PostToMainThread(eventName, edata, throwOnError);
        }
        //Result
        if (edata !== undefined && edata !== null && edata !== void 0) {
            result = this.unpack(edata);
        }
        return result;
    }
    /**执行一段代码, 只能由主线程调用
     * @param chunk
     * @param chunkName
     */
    eval(chunk, chunkName) {
        if (!this.mainThread || xor.globalWorker && xor.globalWorker.worker === this.worker)
            throw new Error("Invalid operation ");
        this.worker.PostEvalToChildThread(chunk, chunkName);
    }
    on() {
        let eventName = arguments[0], fn = arguments[1];
        delete fn[INVOKE_TICK];
        this._on(eventName, fn);
        return this;
    }
    /**监听事件信息(仅回调一次后自动取消注册)
     * @param eventName
     * @param fn
     * @returns
     */
    once(eventName, fn) {
        fn[INVOKE_TICK] = 1;
        this._on(eventName, fn);
        return this;
    }
    /**移除指定监听事件 */
    remove(eventName, fn) {
        let funcs = this.events.get(eventName);
        if (funcs) {
            let idx = funcs.indexOf(fn);
            if (idx >= 0) {
                funcs.splice(idx, 1);
            }
        }
    }
    /**移除所有监听事件 */
    removeAll(eventName) {
        if (eventName)
            this.events.delete(eventName);
        else
            this.events.clear();
    }
    _on(eventName, fn) {
        if (eventName && fn) {
            let funcs = this.events.get(eventName);
            if (!funcs) {
                funcs = [];
                this.events.set(eventName, funcs);
            }
            funcs.push(fn);
        }
    }
    _emit(eventName, ...args) {
        let functions = this.events.get(eventName);
        if (!functions)
            return undefined;
        let rmHandlers = new Array(), result;
        functions.forEach(func => {
            result = func.apply(undefined, args) || result;
            if (INVOKE_TICK in func && (--func[INVOKE_TICK]) <= 0) {
                rmHandlers.push(func);
            }
        });
        if (rmHandlers.length > 0) {
            this.events.set(eventName, functions.filter(func => !rmHandlers.includes(func)));
        }
        return result;
    }
    register() {
        let getValue = (data) => {
            if (data !== undefined && data !== null && data !== void 0) {
                return this.unpack(data);
            }
            return undefined;
        };
        let onmessage = (eventName, data, hasReturn = true) => {
            if (this._isResultId(eventName)) { //post return data event
                let error, result;
                if (data && data.type === CS.XOR.ThreadWorker.ValueType.Error) {
                    error = new Error(`${data.value}`);
                }
                else {
                    result = getValue(data);
                }
                this._emit(eventName, { error, result });
                return;
            }
            let result = this._emit(eventName, getValue(data));
            if (hasReturn && result !== undefined && result !== null && result !== void 0) {
                return this.pack(result);
            }
            return undefined;
        };
        if (this.mainThread) {
            this.worker.MainThreadHandler = (eventName, data) => {
                switch (eventName) {
                    case CLOSE_EVENT:
                        {
                            let closing = true;
                            let funcs = this.events.get(CLOSE_EVENT);
                            if (funcs) {
                                let _data = getValue(data);
                                for (let fn of funcs) {
                                    if (fn(_data) === false) {
                                        closing = false;
                                    }
                                }
                            }
                            if (closing)
                                this.stop();
                            return this.pack(closing);
                        }
                        break;
                    case REMOTE_EVENT:
                        {
                            let result = this.executeRemoteResolver(getValue(data));
                            if (result !== undefined && result !== null && result !== void 0) {
                                result = this.pack(result);
                            }
                            return result;
                        }
                        break;
                    default:
                        return onmessage(eventName, data, true);
                        break;
                }
            };
        }
        else {
            this.worker.ChildThreadHandler = (eventName, data) => onmessage(eventName, data, true);
            if (this.worker.Options && this.worker.Options.remote) {
                this.registerRemoteProxy();
            }
        }
    }
    //创建remote proxy, 实现在子线程内访问Unity Api
    registerRemoteProxy() {
        let createProxy = (namespace) => {
            return new Proxy(Object.create(null), {
                //getter事件
                get: (target, property) => {
                    if (!(property in target) && typeof (property) === "string") {
                        let fullName = namespace ? (namespace + '.' + property) : property;
                        //同步调用Unity Api
                        if (fullName.startsWith("UnityEngine") && fullName !== "UnityEngine.Debug") {
                            let cls = this.postSync(REMOTE_EVENT, fullName);
                            if (cls) {
                                target[property] = cls;
                            }
                            else {
                                target[property] = createProxy(fullName);
                            }
                        }
                        else {
                            let value = CS;
                            fullName.split(".").forEach(name => {
                                if (value && name) {
                                    value = value[name];
                                }
                            });
                            target[property] = value;
                        }
                    }
                    return target[property];
                },
                /*
                //setter 事件
                set: (target, property, newValue) => {
                    return false;
                }
                //method call事件
                apply: (target) => {

                },
                //new() 构造函数事件
                construct: (target, argArray, newTarget) => {
                    return null;
                }
                //*/
            });
        };
        puer["registerBuildinModule"]('csharp', createProxy(undefined));
    }
    //处理remote request, 由主线程调用
    executeRemoteResolver(data) {
        if (typeof data !== "string")
            return undefined;
        let result = CS;
        data.split(".").forEach(name => {
            if (result && name)
                result = result[name];
        });
        if ( /**typeof (result) === "object" && */this._validate(result) === PackValidate.Unsupport) {
            result = undefined;
        }
        return result;
    }
    pack(data) {
        switch (this._validate(data)) {
            case PackValidate.Json:
                {
                    let result = new CS.XOR.ThreadWorker.EventData();
                    if (typeof (data) === "object") {
                        result.type = CS.XOR.ThreadWorker.ValueType.Json;
                        result.value = JSON.stringify(data);
                    }
                    else {
                        result.type = CS.XOR.ThreadWorker.ValueType.Value;
                        result.value = data;
                    }
                    return result;
                }
                break;
            case PackValidate.Reference:
                return this._packByRefs(data, { mapping: new WeakMap(), id: 1 });
                break;
            case PackValidate.Unsupport:
                throw new Error("unsupport data");
                break;
        }
        return undefined;
    }
    unpack(data) {
        switch (data.type) {
            case CS.XOR.ThreadWorker.ValueType.Json:
                return JSON.parse(data.value);
                break;
            default:
                return this._unpackByRefs(data, new Map());
                break;
        }
        return undefined;
    }
    _packByRefs(data, refs) {
        let result = new CS.XOR.ThreadWorker.EventData();
        let t = typeof (data);
        if (t === "object" && refs.mapping.has(data)) {
            result.type = CS.XOR.ThreadWorker.ValueType.RefObject;
            result.value = refs.mapping.get(data) ?? -1;
        }
        else {
            switch (t) {
                case "object":
                    //添加对象引用
                    let id = refs.id++;
                    refs.mapping.set(data, id);
                    //创建对象引用
                    result.id = id;
                    if (data instanceof CS.System.Object) {
                        result.type = CS.XOR.ThreadWorker.ValueType.Value;
                        result.value = data;
                    }
                    else if (data instanceof ArrayBuffer) {
                        result.type = CS.XOR.ThreadWorker.ValueType.ArrayBuffer;
                        result.value = CS.XOR.BufferUtil.ToBytes(data);
                    }
                    else if (Array.isArray(data)) {
                        let list = new List_Object();
                        for (let i = 0; i < data.length; i++) {
                            let member = this._packByRefs(data[i], refs);
                            member.key = i;
                            list.Add(member);
                        }
                        result.type = CS.XOR.ThreadWorker.ValueType.Array;
                        result.value = list;
                    }
                    else {
                        let list = new List_Object();
                        Object.keys(data).forEach(key => {
                            let item = this._packByRefs(data[key], refs);
                            item.key = key;
                            list.Add(item);
                        });
                        result.type = CS.XOR.ThreadWorker.ValueType.Object;
                        result.value = list;
                    }
                    break;
                case "string":
                case "number":
                case "bigint":
                case "boolean":
                    result.type = CS.XOR.ThreadWorker.ValueType.Value;
                    result.value = data;
                    break;
                default:
                    result.type = CS.XOR.ThreadWorker.ValueType.Unknown;
                    break;
            }
        }
        return result;
    }
    _unpackByRefs(data, refs) {
        const { type: Type, value: Value, id: Id } = data;
        let result;
        switch (Type) {
            case CS.XOR.ThreadWorker.ValueType.Object:
                {
                    result = {};
                    if (Id > 0)
                        refs.set(Id, result); //add object ref
                    let list = Value;
                    for (let i = 0; i < list.Count; i++) {
                        let member = list.get_Item(i);
                        result[member.key] = this._unpackByRefs(member, refs);
                    }
                }
                break;
            case CS.XOR.ThreadWorker.ValueType.Array:
                {
                    result = [];
                    if (Id > 0)
                        refs.set(Id, result); //add object ref
                    let list = Value;
                    for (let i = 0; i < list.Count; i++) {
                        let member = list.get_Item(i);
                        result[member.key] = this._unpackByRefs(member, refs);
                    }
                }
                break;
            case CS.XOR.ThreadWorker.ValueType.ArrayBuffer:
                result = CS.XOR.BufferUtil.ToBuffer(Value);
                if (Id > 0)
                    refs.set(Id, result); //add object ref
                break;
            case CS.XOR.ThreadWorker.ValueType.RefObject:
                if (refs.has(Value)) {
                    result = refs.get(Value);
                }
                else {
                    result = `Error: ref id ${Value} not found`;
                }
                break;
            case CS.XOR.ThreadWorker.ValueType.Json:
                result = JSON.parse(data.value);
                if (Id > 0)
                    refs.set(Id, result); //add object ref
                break;
            default:
                result = Value;
                if (Id > 0)
                    refs.set(Id, result); //add object ref
                break;
        }
        return result;
    }
    /**验证data数据
     * @param data
     * @returns 0:纯json数据, 1:引用UnityObject, 2:包含js functon/js symbol等参数
     */
    _validate(data, refs) {
        let t = typeof (data);
        switch (t) {
            case "object":
                if (data === null) {
                    return PackValidate.Json;
                }
                if (data instanceof CS.System.Object ||
                    data instanceof ArrayBuffer) {
                    return PackValidate.Reference;
                }
                if (!refs)
                    refs = new WeakSet();
                if (refs.has(data)) { //引用自身
                    return PackValidate.Reference;
                }
                refs.add(data);
                if (Array.isArray(data)) {
                    for (let _d of data) {
                        let t = this._validate(_d, refs);
                        if (t !== PackValidate.Json)
                            return t;
                    }
                }
                else {
                    for (let key of Object.keys(data)) {
                        let t = this._validate(key, refs);
                        if (t !== PackValidate.Json)
                            return t;
                        t = this._validate(data[key], refs);
                        if (t !== PackValidate.Json)
                            return t;
                    }
                }
                break;
            case "symbol":
            case "function":
                return PackValidate.Unsupport;
                break;
        }
        return PackValidate.Json;
    }
    /**postSync返回值接口事件名
     * @returns
     */
    _getResultId() {
        if (!this._postIndex)
            this._postIndex = 1;
        return `${RESULT_EVENT}${this._postIndex++}`;
    }
    _isResultId(eventName) {
        return eventName && eventName.startsWith(RESULT_EVENT);
    }
}
var PackValidate;
(function (PackValidate) {
    PackValidate[PackValidate["Json"] = 0] = "Json";
    PackValidate[PackValidate["Reference"] = 1] = "Reference";
    PackValidate[PackValidate["Unsupport"] = 2] = "Unsupport";
})(PackValidate || (PackValidate = {}));
function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    _g.xor.ThreadWorker = ThreadWorkerConstructor;
    _g.xor.globalWorker = undefined;
}
register();
//export to csharp
export function bind(worker) {
    let _g = (global || globalThis || this);
    _g.xor = _g.xor || {};
    _g.xor.globalWorker = new ThreadWorkerConstructor(worker);
}
//# sourceMappingURL=worker.js.map