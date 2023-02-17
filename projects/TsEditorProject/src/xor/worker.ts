import $CS = CS;

let List_Object = puer.$generic($CS.System.Collections.Generic.List$1, $CS.System.Object) as {
    new(): $CS.System.Collections.Generic.List$1<$CS.System.Object>
};

const INVOKE_TICK = Symbol("INVOKE_TICK");

const CLOSE_EVENT = "close",
    RESULT_EVENT = "##__result__##",
    REMOTE_EVENT = "##__remote__##";

type Construct<T = any> = Function & { new(...args: any[]): T };

type RemoteRequest = {
    readonly method: "getter",
    readonly key: string,
    readonly type: string,
    readonly instance?: $CS.System.Object;
} | {
    readonly method: "setter",
    readonly key: string,
    readonly value: any,
    readonly type: string,
    readonly instance?: $CS.System.Object;
} | {
    readonly method: "apply",
    readonly key: string,
    readonly args: any[],
    readonly type: string,
    readonly instance?: $CS.System.Object;
} | {
    readonly method: "construct",
    readonly type: string,
    readonly args: any[],
};

/**
 * 跨JsEnv实例交互封装
 */
class ThreadWorkerConstructor {
    private readonly mainThread: boolean;
    private readonly worker: $CS.XOR.ThreadWorker;
    private readonly events: Map<string, Function[]>;
    private _postIndex: number;
    private _remoteRegistered: boolean;

    /**线程是否正在工作 */
    public get isAlive() { return this.worker.IsAlive; }
    /**线程是否已初始化完成 */
    public get isInitialized() { return this.worker.IsInitialized; }
    public get source() { return this.worker; }

    constructor(loader: $CS.Puerts.ILoader, options?: $CS.XOR.ThreadOptions) {
        if (loader instanceof $CS.XOR.ThreadWorker) {
            this.worker = loader;
            this.mainThread = false;
        } else {
            this.worker = $CS.XOR.ThreadWorker.Create(loader, options ?? new $CS.XOR.ThreadOptions());
            this.mainThread = true;
        }
        $CS.XOR.ThreadWorker.VerifyThread(this.mainThread);
        this.events = new Map();
        this.register();
    }
    public start(filepath: string, isESM?: boolean) {
        if (!this.mainThread || xor.globalWorker && xor.globalWorker.worker === this.worker)
            throw new Error("Invalid operation ");
        this.worker.Run(filepath, !!isESM);
    }
    public stop() {
        if (this.mainThread) {
            this.events.clear();
            this.worker.Dispose();
        } else {
            this.post(CLOSE_EVENT);
        }
    }
    /**异步调用事件, 无返回值
     * @param eventName 
     * @param data 
     * @param notResult 不获取返回值 
     */
    public post<TResult = void>(eventName: string, data?: any, notResult?: true): Promise<TResult> {
        let edata: $CS.XOR.ThreadWorker.EventData;
        if (data !== undefined && data !== null && data !== void 0) {
            edata = this.pack(data);
        }
        let resultId = !notResult ? this._getResultId() : null;
        if (this.mainThread) {
            this.worker.PostToChildThread(eventName, edata, resultId);
        } else {
            this.worker.PostToMainThread(eventName, edata, resultId);
        }
        if (resultId) {
            return new Promise<TResult>((resolve, reject) => {
                this.once(resultId, function (d: { error: Error, result: TResult }) {
                    if (d.error) reject(d.error);
                    else resolve(d.result);
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
    public postSync<TResult = any>(eventName: string, data?: any, throwOnError: boolean = true): TResult {
        let edata: $CS.XOR.ThreadWorker.EventData;
        if (data !== undefined && data !== null && data !== void 0) {
            edata = this.pack(data);
        }
        let result: TResult;
        if (this.mainThread) {
            edata = this.worker.Syncr.PostToChildThread(eventName, edata, throwOnError);
        } else {
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
    public eval(chunk: string, chunkName?: string) {
        if (!this.mainThread || xor.globalWorker && xor.globalWorker.worker === this.worker)
            throw new Error("Invalid operation ");
        this.worker.PostEvalToChildThread(chunk, chunkName);
    }

    /**创建Type Construct的remote Proxy对象, 以便于在子线程中调用(仅)主线程方法(仅限受支持的C#类型, 仅限子线程调用)
     * @param construct 
     */
    public remote<T extends $CS.System.Object>(construct: Construct): Construct;
    /**创建C#对象的Remote Proxy对象, 以便于在子线程中调用(仅)主线程方法(仅限受支持的C#类型, 仅限子线程调用)
     * @param instance 
     */
    public remote<T extends $CS.System.Object>(instance: T): T;
    public remote() {
        if (this.mainThread || !xor.globalWorker || xor.globalWorker.worker !== this.worker)
            throw new Error("Invalid operation ");
        if (typeof (arguments[0]) === "function") {
            if (this._remoteRegistered) {
                return arguments[0];
            }
            let cls: Construct = arguments[0],
                fullName: string = puer.$typeof(cls).FullName.replace(/\+/g, ".");
            if (!this._isProxyType(fullName, cls)) {
                return cls;
            }
            return this._createTypeProxy(fullName, cls);
        }
        else {
            let instance = arguments[0];
            if (!instance || !(instance instanceof $CS.System.Object)) {
                return instance;
            }
            let fullName: string = instance.GetType().FullName.replace(/\+/g, "."),
                cls: Construct = Object.getPrototypeOf(instance).constructor;
            if (!this._isProxyType(fullName, cls)) {
                return instance;
            }
            return this._createInstanceProxy(fullName, cls, instance);
        }
    }

    /**监听ThreadWorker close消息(从子线程中请求), 只能由主线程处理, 返回flase将阻止ThreadWorker实例销毁
     * @param eventName 
     * @param fn 
     */
    public on(eventName: typeof CLOSE_EVENT, fn: () => void | false): this;
    /**监听事件信息
     * @param eventName 
     * @param fn 
     */
    public on<T = any, TResult = void>(eventName: string, fn: (data?: T) => TResult): this;
    public on() {
        let eventName: string = arguments[0], fn: Function = arguments[1];
        delete fn[INVOKE_TICK];
        this._on(eventName, fn);
        return this;
    }
    /**监听事件信息(仅回调一次后自动取消注册)
     * @param eventName 
     * @param fn 
     * @returns 
     */
    public once(eventName: string, fn: Function): this {
        fn[INVOKE_TICK] = 1;
        this._on(eventName, fn);
        return this;
    }
    /**移除指定监听事件 */
    public remove(eventName: string, fn: Function) {
        let funcs = this.events.get(eventName);
        if (funcs) {
            let idx = funcs.indexOf(fn);
            if (idx >= 0) {
                funcs.splice(idx, 1);
            }
        }
    }
    /**移除所有监听事件 */
    public removeAll(eventName?: string) {
        if (eventName)
            this.events.delete(eventName);
        else
            this.events.clear();
    }

    private _on(eventName: string, fn: Function) {
        if (eventName && fn) {
            let funcs = this.events.get(eventName);
            if (!funcs) {
                funcs = [];
                this.events.set(eventName, funcs);
            }
            funcs.push(fn);
        }
    }
    private _emit(eventName: string, ...args: any[]) {
        let functions = this.events.get(eventName);
        if (!functions)
            return undefined;
        let rmHandlers = new Array<Function>(), result: any;
        functions.forEach(func => {
            result = func.apply(undefined, args) || result;
            if (INVOKE_TICK in func && (--(<number>func[INVOKE_TICK])) <= 0) {
                rmHandlers.push(func);
            }
        });
        if (rmHandlers.length > 0) {
            this.events.set(eventName, functions.filter(func => !rmHandlers.includes(func)));
        }
        return result;
    }

    private register() {
        let getValue = (data: $CS.XOR.ThreadWorker.EventData) => {
            if (data !== undefined && data !== null && data !== void 0) {
                return this.unpack(data);
            }
            return undefined;
        };
        let onmessage = (eventName: string, data: $CS.XOR.ThreadWorker.EventData, hasReturn: boolean = true): $CS.XOR.ThreadWorker.EventData => {
            if (this._isResultId(eventName)) {          //post return data event
                let error: Error, result: any;
                if (data && data.type === $CS.XOR.ThreadWorker.ValueType.Error) {
                    error = new Error(`${data.value}`);
                } else {
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
                            if (closing) this.stop();
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
        } else {
            this.worker.ChildThreadHandler = (eventName, data) => onmessage(eventName, data, true);
            if (this.worker.Options && this.worker.Options.remote) {
                this._remoteRegistered = true;
                this.registerRemoteProxy();
            }
        }
    }
    //创建remote proxy, 实现在子线程内访问Unity Api
    private registerRemoteProxy() {
        let createProxy = (namespace: string) => {
            return new Proxy(Object.create(null), {
                //getter事件
                get: (target, name) => {
                    if (!(name in target) && typeof (name) === "string") {
                        let fullName = namespace ? (namespace + '.' + name) : name;

                        let value: any = $CS;
                        fullName.split(".").forEach(name => {
                            if (value && name) { value = value[name]; }
                        });
                        if (this._isProxyType(fullName, value)) {
                            target[name] = this._createTypeProxy(fullName, value);
                        }
                        else if (typeof (value) === "object") {
                            target[name] = createProxy(fullName);
                        }
                        else {
                            target[name] = value;
                        }
                    }
                    return target[name];
                },
            });
        }
        const csharpModule = createProxy(undefined);
        puer["registerBuildinModule"]('csharp', csharpModule);
        let _g: any = (global || globalThis);
        _g.CS = csharpModule;
        _g.csharp = csharpModule;
    }
    //处理remote request, 由主线程调用
    private executeRemoteResolver(data: RemoteRequest): any {
        if (!data) {
            return undefined;
        }
        let cls: Construct = CS as any;
        data.type?.split(".").forEach(name => {
            if (cls && name) cls = cls[name];
        });
        if (!cls || typeof (cls) !== "function") {
            return undefined;
        }
        let result: any;
        switch (data.method) {
            case "getter":
                if (data.instance) {
                    result = data.instance[data.key];
                } else {
                    result = cls[data.key];
                }
                break;
            case "setter":
                if (data.instance) {
                    data.instance[data.key] = data.value;
                } else {
                    cls[data.key] = data.value;
                }
                break;
            case "apply":
                let fn: Function = data.instance ? cls.prototype[data.key] : cls[data.key];
                if (fn) {
                    result = fn.apply(data.instance, data.args);
                }
                break;
            case "construct":
                result = data.args ? new cls(...data.args) : new cls();
                break;
            default:
                console.error('无效的参数调用');
                break;
        }
        if (/**typeof (result) === "object" && */ this._validate(result) === PackValidate.Unsupport) {
            result = undefined;
        }
        return result;
    }

    private pack(data: any): $CS.XOR.ThreadWorker.EventData {
        switch (this._validate(data)) {
            case PackValidate.Json:
                {
                    let result = new $CS.XOR.ThreadWorker.EventData();
                    if (typeof (data) === "object") {
                        result.type = $CS.XOR.ThreadWorker.ValueType.Json;
                        result.value = JSON.stringify(data);
                    } else {
                        result.type = $CS.XOR.ThreadWorker.ValueType.Value;
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
    private unpack(data: $CS.XOR.ThreadWorker.EventData): any {
        switch (data.type) {
            case $CS.XOR.ThreadWorker.ValueType.Json:
                return JSON.parse(data.value);
                break;
            default:
                return this._unpackByRefs(data, new Map());
                break;
        }
        return undefined;
    }
    private _packByRefs(data: any, refs: { readonly mapping: WeakMap<object, number>, id: number }): $CS.XOR.ThreadWorker.EventData {
        let result = new $CS.XOR.ThreadWorker.EventData();

        let t = typeof (data);
        if (t === "object" && refs.mapping.has(data)) {
            result.type = $CS.XOR.ThreadWorker.ValueType.RefObject;
            result.value = refs.mapping.get(data) ?? -1;
        } else {
            switch (t) {
                case "object":
                    //添加对象引用
                    let id = refs.id++;
                    refs.mapping.set(data, id);
                    //创建对象引用
                    result.id = id;
                    if (data instanceof $CS.System.Object) {
                        result.type = $CS.XOR.ThreadWorker.ValueType.Value;
                        result.value = data;
                    }
                    else if (data instanceof ArrayBuffer) {
                        result.type = $CS.XOR.ThreadWorker.ValueType.ArrayBuffer;
                        result.value = $CS.XOR.BufferUtil.ToBytes(data);
                    }
                    else if (Array.isArray(data)) {
                        let list = new List_Object();
                        for (let i = 0; i < data.length; i++) {
                            let member = this._packByRefs(data[i], refs);
                            member.key = i;
                            list.Add(member);
                        }
                        result.type = $CS.XOR.ThreadWorker.ValueType.Array;
                        result.value = list;
                    } else {
                        let list = new List_Object();
                        Object.keys(data).forEach(key => {
                            let item = this._packByRefs(data[key], refs);
                            item.key = key;
                            list.Add(item);
                        });
                        result.type = $CS.XOR.ThreadWorker.ValueType.Object;
                        result.value = list;
                    }
                    break;
                case "string":
                case "number":
                case "bigint":
                case "boolean":
                    result.type = $CS.XOR.ThreadWorker.ValueType.Value;
                    result.value = data;
                    break;
                default:
                    result.type = $CS.XOR.ThreadWorker.ValueType.Unknown;
                    break;
            }
        }
        return result;
    }
    private _unpackByRefs(data: $CS.XOR.ThreadWorker.EventData, refs: Map<number, object>) {
        const { type, value, id } = data;

        let result: any;
        switch (type) {
            case $CS.XOR.ThreadWorker.ValueType.Object:
                {
                    result = {};
                    if (id > 0) refs.set(id, result);                   //add object ref
                    let list = value as $CS.System.Collections.Generic.List$1<$CS.XOR.ThreadWorker.EventData>;
                    for (let i = 0; i < list.Count; i++) {
                        let member = list.get_Item(i);
                        if (member.key == "type") {
                            debugger;
                        }
                        result[member.key] = this._unpackByRefs(member, refs);
                    }
                }
                break;
            case $CS.XOR.ThreadWorker.ValueType.Array:
                {
                    result = [];
                    if (id > 0) refs.set(id, result);                   //add object ref
                    let list = value as $CS.System.Collections.Generic.List$1<$CS.XOR.ThreadWorker.EventData>;
                    for (let i = 0; i < list.Count; i++) {
                        let member = list.get_Item(i);
                        result[member.key] = this._unpackByRefs(member, refs);
                    }
                }
                break;
            case $CS.XOR.ThreadWorker.ValueType.ArrayBuffer:
                result = $CS.XOR.BufferUtil.ToBuffer(value);
                if (id > 0) refs.set(id, result);                       //add object ref
                break;
            case $CS.XOR.ThreadWorker.ValueType.RefObject:
                if (refs.has(value)) {
                    result = refs.get(value);
                } else {
                    result = `Error: ref id ${value} not found`;
                }
                break;
            case $CS.XOR.ThreadWorker.ValueType.Json:
                result = JSON.parse(data.value);
                if (id > 0) refs.set(id, result);                       //add object ref
                break;
            default:
                result = value;
                if (id > 0) refs.set(id, result);                       //add object ref
                break;
        }
        return result;
    }
    /**验证data数据
     * @param data 
     * @returns 0:纯json数据, 1:引用UnityObject, 2:包含js functon/js symbol等参数
     */
    private _validate(data: any, refs?: WeakSet<object>,): PackValidate {
        let t = typeof (data);
        switch (t) {
            case "object":
                if (data === null) {
                    return PackValidate.Json;
                }
                if (data instanceof $CS.System.Object ||
                    data instanceof ArrayBuffer
                ) {
                    return PackValidate.Reference;
                }

                if (!refs) refs = new WeakSet();
                if (refs.has(data)) {   //引用自身
                    return PackValidate.Reference;
                }
                refs.add(data);
                if (Array.isArray(data)) {
                    for (let _d of data) {
                        let t = this._validate(_d, refs);
                        if (t !== PackValidate.Json) return t;
                    }
                } else {
                    for (let key of Object.keys(data)) {
                        let t = this._validate(key, refs);
                        if (t !== PackValidate.Json) return t;

                        t = this._validate(data[key], refs);
                        if (t !== PackValidate.Json) return t;
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
    private _getResultId() {
        if (!this._postIndex) this._postIndex = 1;
        return `${RESULT_EVENT}${this._postIndex++}`;
    }
    private _isResultId(eventName: string) {
        return eventName && eventName.startsWith(RESULT_EVENT);
    }

    /**remote proxy方法 */
    private _isProxyType(fullName: string, cls: Construct) {
        if (typeof (cls) !== "function") {
            return false;
        }
        let type = puer.$typeof(cls);
        if (!type || !type.IsClass) {
            return false;
        }
        return fullName.startsWith("UnityEngine") && fullName !== "UnityEngine.Debug";
    }
    private _createTypeProxy(fullName: string, cls: Construct) {
        let methodProxies: { [key: string]: Function } = {};
        let descriptors = Object.getOwnPropertyDescriptors(cls);
        Object.keys(descriptors).forEach(key => {
            let d = descriptors[key];
            if (!d || d.set || d.get || typeof (d.value) !== "function")
                return;
            methodProxies[key] = null;
        });

        return new Proxy(cls, {
            get: (target, name) => {
                if (typeof (name) !== "string") {
                    return cls[name];
                }
                //get method proxy
                if (name in methodProxies) {
                    let func = methodProxies[name];
                    if (!func) {
                        func = Object.getOwnPropertyDescriptor(cls, name).value;
                        func = methodProxies[name] = this._createMethodProxy(fullName, name, func);
                    }
                    return func;
                }
                //getter
                else {
                    let event: RemoteRequest = {
                        method: "getter",
                        type: fullName,
                        key: name as string
                    };
                    if (this._validate(event) === PackValidate.Unsupport) {
                        throw new Error("Invalid parameter exception");
                    }
                    return this.postSync(REMOTE_EVENT, event);
                }
            },
            set: (target, name, newValue) => {
                if (typeof (name) !== "string") {
                    cls[name] = newValue;
                    return true;
                }
                let event: RemoteRequest = {
                    method: "setter",
                    type: fullName,
                    key: name as string,
                    value: newValue
                };
                if (this._validate(event) === PackValidate.Unsupport) {
                    throw new Error("Invalid parameter exception");
                }
                this.postSync(REMOTE_EVENT, event);
                return true;
            },
            construct: (target, argArray, newTarget) => {
                let event: RemoteRequest = {
                    method: "construct",
                    type: fullName,
                    args: argArray
                };
                if (this._validate(event) === PackValidate.Unsupport) {
                    throw new Error("Invalid parameter exception");
                }
                return this.postSync(REMOTE_EVENT, event);
            }
        });
    }
    private _createMethodProxy(fullName: string, name: string, fn: Function, instance?: any) {
        return new Proxy(fn, {
            apply: (target, thisArg, argArray) => {
                let event: RemoteRequest = {
                    method: "apply",
                    key: name as string,
                    args: argArray,
                    type: fullName,
                    instance: instance,
                };
                if (this._validate(event) === PackValidate.Unsupport) {
                    throw new Error("Invalid parameter exception");
                }
                return this.postSync(REMOTE_EVENT, event);
            }
        })
    }
    private _createInstanceProxy<T extends $CS.System.Object>(fullName: string, cls: Construct, instance: T): T {
        let methodProxies: { [key: string]: Function } = {};
        let descriptors = Object.getOwnPropertyDescriptors(cls.prototype);
        Object.keys(descriptors).forEach(key => {
            let d = descriptors[key];
            if (!d || d.set || d.get || typeof (d.value) !== "function")
                return;
            methodProxies[key] = null;
        });
        return new Proxy(instance, {
            get: (target, name) => {
                if (typeof (name) !== "string") {
                    return instance[name];
                }
                //get method proxy
                if (name in methodProxies) {
                    let func = methodProxies[name];
                    if (!func) {
                        func = Object.getOwnPropertyDescriptor(cls.prototype, name).value;
                        func = methodProxies[name] = this._createMethodProxy(fullName, name, func, instance);
                    }
                    return func;
                }
                //getter
                else {
                    let event: RemoteRequest = {
                        method: "getter",
                        instance: instance,
                        key: name as string,
                        type: fullName,
                    };
                    if (this._validate(event) === PackValidate.Unsupport) {
                        throw new Error("Invalid parameter exception");
                    }
                    return this.postSync(REMOTE_EVENT, event);
                }
            },
            set: (target, name, newValue) => {
                if (typeof (name) !== "string") {
                    instance[name] = newValue;
                    return true;
                }
                let event: RemoteRequest = {
                    method: "setter",
                    instance: instance,
                    key: name as string,
                    value: newValue,
                    type: fullName,
                };
                if (this._validate(event) === PackValidate.Unsupport) {
                    throw new Error("Invalid parameter exception");
                }
                this.postSync(REMOTE_EVENT, event);
                return true;
            },
        });
    }
}
enum PackValidate {
    Json = 0,
    Reference = 1,
    Unsupport = 2,
}
function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    _g.xor.ThreadWorker = ThreadWorkerConstructor;
    _g.xor.globalWorker = undefined;
}
register();

/**接口声明 */
declare global {
    namespace xor {
        class ThreadWorker extends ThreadWorkerConstructor { }
        /**
         * 只能在JsWorker内部访问, 与主线程交互的对象
         */
        const globalWorker: ThreadWorker;
    }
}

//export to csharp
export function bind(worker: $CS.XOR.ThreadWorker) {
    let _g = (global || globalThis || this);
    _g.xor = _g.xor || {};
    _g.xor.globalWorker = new ThreadWorkerConstructor(<any>worker);
}