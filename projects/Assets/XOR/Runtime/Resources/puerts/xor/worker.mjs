var $CS = CS;
const INVOKE_TICK = Symbol("INVOKE_TICK");
const CLOSE_EVENT = "close", RESULT_EVENT = "##__result__##", REMOTE_EVENT = "##__remote__##", REMOTE_REMOTE_OBJECT = Symbol("__remote_object__"), REMOTE_LOCAL_OBJECT = Symbol("__local_object__");
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
        if (loader instanceof $CS.XOR.ThreadWorker) {
            this.worker = loader;
            this.mainThread = false;
        }
        else {
            this.worker = $CS.XOR.ThreadWorker.Create(loader, options ?? new $CS.XOR.ThreadOptions());
            this.mainThread = true;
        }
        $CS.XOR.ThreadWorker.VerifyThread(this.mainThread);
        this.events = new Map();
        this.register();
    }
    start(filepath) {
        if (!this.mainThread || xor.globalWorker && xor.globalWorker.worker === this.worker)
            throw new Error("Invalid operation ");
        this.worker.Run(filepath);
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
        let parameter;
        if (data !== undefined && data !== null && data !== void 0) {
            parameter = utils.encode(data);
        }
        let resultId = !notResult ? this._getResultId() : null;
        if (this.mainThread) {
            this.worker.PostToChildThread(eventName, parameter, resultId);
        }
        else {
            this.worker.PostToMainThread(eventName, parameter, resultId);
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
        let parameter;
        if (data !== undefined && data !== null && data !== void 0) {
            parameter = utils.encode(data);
        }
        let result;
        if (this.mainThread) {
            parameter = this.worker.Syncr.PostToChildThread(eventName, parameter, throwOnError);
        }
        else {
            parameter = this.worker.Syncr.PostToMainThread(eventName, parameter, throwOnError);
        }
        //Result
        if (parameter !== undefined && parameter !== null && parameter !== void 0) {
            if (parameter.IsError) {
                throw new Error(`${parameter.Exception}`);
            }
            result = utils.decode(parameter);
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
    remote() {
        if (this.mainThread || !xor.globalWorker || xor.globalWorker.worker !== this.worker)
            throw new Error("Invalid operation ");
        if (typeof (arguments[0]) === "function") {
            if (this._remoteRegistered) {
                return arguments[0];
            }
            let cls = arguments[0], fullName = puer.$typeof(cls).FullName.replace(/\+/g, ".");
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
            let remoteObject = instance[REMOTE_REMOTE_OBJECT];
            if (!remoteObject) {
                let fullName = instance.GetType().FullName.replace(/\+/g, "."), cls = Object.getPrototypeOf(instance).constructor;
                if (!this._isProxyType(fullName, cls)) {
                    return instance;
                }
                remoteObject = this._createInstanceProxy(fullName, cls, instance);
                Object.defineProperty(instance, REMOTE_REMOTE_OBJECT, {
                    configurable: false,
                    enumerable: false,
                    writable: false,
                    value: remoteObject,
                });
                Object.defineProperty(instance, REMOTE_LOCAL_OBJECT, {
                    configurable: false,
                    enumerable: false,
                    writable: false,
                    value: instance,
                });
            }
            return remoteObject;
        }
    }
    /**从remote对象上获取原始对象
     * @param instance
     * @returns
     */
    local(instance) {
        if (!instance) {
            return instance;
        }
        return instance[REMOTE_LOCAL_OBJECT] ?? instance;
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
                return utils.decode(data);
            }
            return undefined;
        };
        let onmessage = (eventName, data, hasReturn = true) => {
            if (this._isResultId(eventName)) { //post return data event
                let error, result;
                if (data && data.IsError) {
                    error = new Error(`${data.Exception}`);
                }
                else {
                    result = getValue(data);
                }
                this._emit(eventName, { error, result });
                return;
            }
            let result = this._emit(eventName, getValue(data));
            if (hasReturn && result !== undefined && result !== null && result !== void 0) {
                return utils.encode(result);
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
                            return utils.encode(closing);
                        }
                        break;
                    case REMOTE_EVENT:
                        {
                            let result = this.executeRemoteResolver(getValue(data));
                            if (result !== undefined && result !== null && result !== void 0) {
                                result = utils.encode(result);
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
                this._remoteRegistered = true;
                this.registerRemoteProxy();
            }
        }
    }
    //创建remote proxy, 实现在子线程内访问Unity Api
    registerRemoteProxy() {
        let createProxy = (namespace) => {
            return new Proxy(Object.create(null), {
                //getter事件
                get: (target, name) => {
                    if (!(name in target) && typeof (name) === "string") {
                        let fullName = namespace ? (namespace + '.' + name) : name;
                        let value = $CS;
                        fullName.split(".").forEach(name => {
                            if (value && name) {
                                value = value[name];
                            }
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
        };
        const csharpModule = createProxy(undefined);
        puer["registerBuildinModule"]('csharp', csharpModule);
        let _g = (global || globalThis);
        _g.CS = csharpModule;
        _g.csharp = csharpModule;
    }
    //处理remote request, 由主线程调用
    executeRemoteResolver(data) {
        if (!data) {
            return undefined;
        }
        let cls = CS;
        data.type?.split(".").forEach(name => {
            if (cls && name)
                cls = cls[name];
        });
        if (!cls || typeof (cls) !== "function") {
            return undefined;
        }
        let result;
        switch (data.method) {
            case "getter":
                if (data.instance) {
                    result = data.instance[data.key];
                }
                else {
                    result = cls[data.key];
                }
                break;
            case "setter":
                if (data.instance) {
                    data.instance[data.key] = data.value;
                }
                else {
                    cls[data.key] = data.value;
                }
                break;
            case "apply":
                let fn = data.instance ? cls.prototype[data.key] : cls[data.key];
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
        if ( /**typeof (result) === "object" && */!utils.isSupported(result)) {
            result = undefined;
        }
        return result;
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
    /**remote proxy方法 */
    _isProxyType(fullName, cls) {
        if (typeof (cls) !== "function") {
            return false;
        }
        let type = puer.$typeof(cls);
        if (!type || !type.IsClass) {
            return false;
        }
        return fullName.startsWith("UnityEngine") && fullName !== "UnityEngine.Debug";
    }
    _createTypeProxy(fullName, cls) {
        let methodProxies = {};
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
                    let event = {
                        method: "getter",
                        type: fullName,
                        key: name
                    };
                    if (!utils.isSupported(event)) {
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
                let event = {
                    method: "setter",
                    type: fullName,
                    key: name,
                    value: newValue
                };
                if (!utils.isSupported(event)) {
                    throw new Error("Invalid parameter exception");
                }
                this.postSync(REMOTE_EVENT, event);
                return true;
            },
            construct: (target, argArray, newTarget) => {
                let event = {
                    method: "construct",
                    type: fullName,
                    args: argArray
                };
                if (!utils.isSupported(event)) {
                    throw new Error("Invalid parameter exception");
                }
                return this.postSync(REMOTE_EVENT, event);
            }
        });
    }
    _createMethodProxy(fullName, name, fn, instance) {
        return new Proxy(fn, {
            apply: (target, thisArg, argArray) => {
                let event = {
                    method: "apply",
                    key: name,
                    args: argArray,
                    type: fullName,
                    instance: instance,
                };
                if (!utils.isSupported(event)) {
                    throw new Error("Invalid parameter exception");
                }
                return this.postSync(REMOTE_EVENT, event);
            }
        });
    }
    _createInstanceProxy(fullName, cls, instance) {
        let methodProxies = {};
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
                    let event = {
                        method: "getter",
                        instance: instance,
                        key: name,
                        type: fullName,
                    };
                    if (!utils.isSupported(event)) {
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
                let event = {
                    method: "setter",
                    instance: instance,
                    key: name,
                    value: newValue,
                    type: fullName,
                };
                if (!utils.isSupported(event)) {
                    throw new Error("Invalid parameter exception");
                }
                this.postSync(REMOTE_EVENT, event);
                return true;
            },
        });
    }
}
var utils;
(function (utils) {
    let ObjectLevel;
    (function (ObjectLevel) {
        /**纯json对象 */
        ObjectLevel[ObjectLevel["Json"] = 0] = "Json";
        /**含引用的对象(引用C# Object或js Object) */
        ObjectLevel[ObjectLevel["Reference"] = 1] = "Reference";
        /**不支持: 含有fucntion/symbol等参数 */
        ObjectLevel[ObjectLevel["Unsupport"] = 2] = "Unsupport";
    })(ObjectLevel || (ObjectLevel = {}));
    function _getObjectLevel(data, refs) {
        let t = typeof (data);
        switch (t) {
            case "object":
                if (data === null) {
                    return ObjectLevel.Json;
                }
                if (data instanceof CS.System.Object || data instanceof ArrayBuffer) {
                    return ObjectLevel.Reference;
                }
                refs ?? (refs = new WeakSet());
                if (refs.has(data)) { //引用自身
                    return ObjectLevel.Reference;
                }
                refs.add(data);
                if (Array.isArray(data)) {
                    for (let _d of data) {
                        let t = _getObjectLevel(_d, refs);
                        if (t !== ObjectLevel.Json)
                            return t;
                    }
                }
                else {
                    for (let key of Object.keys(data)) {
                        let t = _getObjectLevel(key, refs);
                        if (t !== ObjectLevel.Json)
                            return t;
                        t = _getObjectLevel(data[key], refs);
                        if (t !== ObjectLevel.Json)
                            return t;
                    }
                }
                break;
            case "symbol":
            case "function":
                return ObjectLevel.Unsupport;
                break;
        }
        return ObjectLevel.Json;
    }
    let DataTypes;
    (function (DataTypes) {
        DataTypes[DataTypes["Undefined"] = 0] = "Undefined";
        DataTypes[DataTypes["Null"] = 1] = "Null";
        DataTypes[DataTypes["Boolean"] = 2] = "Boolean";
        DataTypes[DataTypes["Integer"] = 3] = "Integer";
        DataTypes[DataTypes["Number"] = 4] = "Number";
        DataTypes[DataTypes["Bigint"] = 5] = "Bigint";
        DataTypes[DataTypes["String"] = 6] = "String";
        DataTypes[DataTypes["ArrayBuffer"] = 7] = "ArrayBuffer";
        DataTypes[DataTypes["Array"] = 8] = "Array";
        DataTypes[DataTypes["Object"] = 9] = "Object";
        DataTypes[DataTypes["Reference"] = 10] = "Reference";
    })(DataTypes || (DataTypes = {}));
    class BufferWriter {
        constructor() {
            this._position = 0;
            this._free = 0;
            this._size = 0;
        }
        toData() {
            if (this._size === 0)
                return Buffer.alloc(0);
            const allocated = this._resolved ?? [];
            if (this._allocating && this._position > 0) {
                if (this._free === 0)
                    allocated.push(this._allocating);
                else
                    allocated.push(this._allocating.subarray(0, this._position));
            }
            return Buffer.concat(allocated, this._size);
        }
        writeInt8(value) {
            const size = 1;
            if (this._free >= size) {
                this._allocating.writeInt8(value, this._position);
                this._move(size);
            }
            else {
                let buffer = Buffer.alloc(size);
                buffer.writeInt8(value);
                this._append(buffer);
            }
        }
        writeUInt8(value) {
            const size = 1;
            if (this._free >= size) {
                this._allocating.writeUInt8(value, this._position);
                this._move(size);
            }
            else {
                let buffer = Buffer.alloc(size);
                buffer.writeUInt8(value);
                this._append(buffer);
            }
        }
        writeInt16(value) {
            const size = 2;
            if (this._free >= size) {
                this._allocating.writeInt16BE(value, this._position);
                this._move(size);
            }
            else {
                let buffer = Buffer.alloc(size);
                buffer.writeInt16BE(value);
                this._append(buffer);
            }
        }
        writeUInt16(value) {
            const size = 2;
            if (this._free >= size) {
                this._allocating.writeUInt16BE(value, this._position);
                this._move(size);
            }
            else {
                let buffer = Buffer.alloc(size);
                buffer.writeUInt16BE(value);
                this._append(buffer);
            }
        }
        writeInt32(value) {
            const size = 4;
            if (this._free >= size) {
                this._allocating.writeInt32BE(value, this._position);
                this._move(size);
            }
            else {
                let buffer = Buffer.alloc(size);
                buffer.writeInt32BE(value);
                this._append(buffer);
            }
        }
        writeUInt32(value) {
            const size = 4;
            if (this._free >= size) {
                this._allocating.writeUInt32BE(value, this._position);
                this._move(size);
            }
            else {
                let buffer = Buffer.alloc(size);
                buffer.writeUInt32BE(value);
                this._append(buffer);
            }
        }
        writeInt64(value) {
            const size = 8;
            if (this._free >= size) {
                this._allocating.writeBigInt64BE(value, this._position);
                this._move(size);
            }
            else {
                let buffer = Buffer.alloc(size);
                buffer.writeBigInt64BE(value);
                this._append(buffer);
            }
        }
        writeDouble(value) {
            const size = 8;
            if (this._free >= size) {
                this._allocating.writeDoubleBE(value, this._position);
                this._move(size);
            }
            else {
                let buffer = Buffer.alloc(size);
                buffer.writeDoubleBE(value);
                this._append(buffer);
            }
        }
        writeBuffer(value) {
            this.writeUInt32(value.length);
            this._append(Buffer.isBuffer(value) ? value : Buffer.from(value));
        }
        writeBoolean(value) {
            this.writeUInt8(value ? 1 : 0);
        }
        writeString(value, encoding) {
            let buffer = Buffer.from(value, encoding ?? 'utf8');
            this.writeBuffer(buffer);
        }
        _append(buffer) {
            const size = buffer.length;
            if (this._free === 0) {
                this._alloc(size * 2);
            }
            if (this._free >= size) {
                //有充足的空间, 拷贝全部数据
                buffer.copy(this._allocating, this._position, 0, size);
                this._move(size);
                return;
            }
            else {
                //进行分片拷贝数据
                const resolved = this._free, unresolved = size - resolved;
                //追加数据到末尾
                buffer.copy(this._allocating, this._position, 0, resolved);
                this._move(resolved);
                //分配新空间, 然后拷贝未处理的数据
                this._alloc(unresolved);
                buffer.copy(this._allocating, this._position, resolved, size);
                this._move(unresolved);
            }
        }
        _move(size) {
            this._position += size;
            this._free -= size;
            this._size += size;
        }
        _alloc(expectedSize) {
            if (this._allocating) {
                this._resolved ?? (this._resolved = []);
                this._resolved.push(this._allocating);
            }
            let allocSize = Math.min(BufferWriter.ALLOCATE_STEP_MAX, Math.max(BufferWriter.ALLOCATE_STEP_MIN, this._size * 2));
            if (allocSize < expectedSize) {
                allocSize = expectedSize;
            }
            this._allocating = Buffer.alloc(allocSize);
            this._free = allocSize;
            this._position = 0;
        }
    }
    /**初始空间分配大小 */
    BufferWriter.ALLOCATE_STEP_MIN = 4 * 1024;
    /**最大递增分配空间大小 */
    BufferWriter.ALLOCATE_STEP_MAX = 4 * 1024 * 1024;
    class BufferReader {
        constructor(buffer) {
            this.buffer = Buffer.isBuffer(buffer) ? buffer : Buffer.from(buffer);
            this._position = 0;
        }
        readInt8() {
            let result = this.buffer.readInt8(this._position);
            this._position += 1;
            return result;
        }
        readUInt8() {
            let result = this.buffer.readUInt8(this._position);
            this._position += 1;
            return result;
        }
        readInt16() {
            let result = this.buffer.readInt16BE(this._position);
            this._position += 2;
            return result;
        }
        readUInt16() {
            let result = this.buffer.readUInt16BE(this._position);
            this._position += 2;
            return result;
        }
        readInt32() {
            let result = this.buffer.readInt32BE(this._position);
            this._position += 4;
            return result;
        }
        readUInt32() {
            let result = this.buffer.readUInt32BE(this._position);
            this._position += 4;
            return result;
        }
        readInt64() {
            let result = this.buffer.readBigInt64BE(this._position);
            this._position += 8;
            return result;
        }
        readDouble() {
            let result = this.buffer.readDoubleBE(this._position);
            this._position += 4;
            return result;
        }
        readBuffer() {
            const size = this.readUInt32();
            let result = this.buffer.subarray(this._position, this._position + size);
            this._position += size;
            return result;
        }
        readBoolean() {
            let v = this.readUInt8();
            return v !== 0;
        }
        readString(encoding) {
            const size = this.readUInt32();
            let result = this.buffer.toString(encoding ?? 'utf8', this._position, this._position + size);
            this._position += size;
            return result;
        }
    }
    const INT32_MAX_VALUE = 2147483647, INT32_MIN_VALUE = -2147483648;
    function _encode(data, result) {
        let t = typeof (data);
        if (t === "object" && result.mapping.has(data)) {
            let objId = result.mapping.get(data) ?? -1;
            result.writer.writeUInt8(DataTypes.Reference);
            result.writer.writeUInt16(objId);
            return;
        }
        switch (t) {
            case "object":
                if (data === null) {
                    result.writer.writeUInt8(DataTypes.Null);
                    return;
                }
                //添加对象引用
                let objId = result.id++;
                result.mapping.set(data, objId);
                //创建对象信息
                if (data instanceof CS.System.Object) {
                    result.writer.writeUInt8(DataTypes.Reference);
                    result.writer.writeUInt16(objId);
                    result.csharp.push([data, objId]);
                }
                else if (data instanceof ArrayBuffer || data instanceof Uint8Array || Buffer.isBuffer(data)) {
                    result.writer.writeUInt8(DataTypes.ArrayBuffer);
                    result.writer.writeUInt16(objId);
                    result.writer.writeBuffer(Buffer.isBuffer(data) ? data : Buffer.from(data));
                }
                else if (Array.isArray(data)) {
                    result.writer.writeUInt8(DataTypes.Array);
                    result.writer.writeUInt16(objId);
                    result.writer.writeUInt32(data.length);
                    for (let i = 0; i < data.length; i++) {
                        _encode(data[i], result);
                    }
                }
                else {
                    let keys = Object.keys(data);
                    result.writer.writeUInt8(DataTypes.Object);
                    result.writer.writeUInt16(objId);
                    result.writer.writeUInt32(keys.length);
                    keys.forEach(key => {
                        _encode(key, result);
                        _encode(data[key], result);
                    });
                }
                break;
            case "number":
                if (!Number.isInteger(data) || data < INT32_MIN_VALUE || data > INT32_MAX_VALUE) {
                    result.writer.writeUInt8(DataTypes.Number);
                    result.writer.writeDouble(/* Number.isNaN(data) ? 0 :  */ data);
                }
                else {
                    result.writer.writeUInt8(DataTypes.Integer);
                    result.writer.writeInt32(data);
                }
                break;
            case "bigint":
                result.writer.writeUInt8(DataTypes.Bigint);
                result.writer.writeInt64(data);
                break;
            case "string":
                result.writer.writeUInt8(DataTypes.String);
                result.writer.writeString(data);
                break;
            case "boolean":
                result.writer.writeUInt8(DataTypes.Boolean);
                result.writer.writeBoolean(data);
                break;
            default:
                result.writer.writeUInt8(DataTypes.Undefined);
                break;
        }
    }
    function _decode(reader, mapping) {
        let type = reader.readUInt8(), result;
        switch (type) {
            case DataTypes.Reference:
                {
                    let objId = reader.readUInt16();
                    result = mapping.get(objId);
                }
                break;
            case DataTypes.ArrayBuffer:
                {
                    let objId = reader.readUInt16();
                    result = reader.readBuffer();
                    mapping.set(objId, result); //add object reference
                }
                break;
            case DataTypes.Array:
                {
                    let array = [];
                    result = array;
                    let objId = reader.readUInt16();
                    let len = reader.readUInt32();
                    mapping.set(objId, array); //add object reference
                    while (len-- > 0) {
                        array.push(_decode(reader, mapping));
                    }
                }
                break;
            case DataTypes.Object:
                {
                    let obj = {};
                    result = obj;
                    let objId = reader.readUInt16();
                    let len = reader.readUInt32();
                    mapping.set(objId, obj); //add object reference
                    while (len-- > 0) {
                        let key = _decode(reader, mapping), value = _decode(reader, mapping);
                        obj[key] = value;
                    }
                }
                break;
            case DataTypes.Integer:
                result = reader.readInt32();
                break;
            case DataTypes.Number:
                result = reader.readDouble();
                break;
            case DataTypes.String:
                result = reader.readString();
                break;
            case DataTypes.Bigint:
                result = reader.readInt64();
                break;
            case DataTypes.Boolean:
                result = reader.readBoolean();
                break;
            case DataTypes.Null:
                result = null;
                break;
            case DataTypes.Undefined:
                result = undefined;
                break;
        }
        return result;
    }
    function encode(data) {
        let result = {
            id: 0,
            csharp: [],
            mapping: new WeakMap(),
            writer: new BufferWriter(),
        };
        _encode(data, result);
        let parameter = new CS.XOR.ThreadWorker.EventParameter(result.writer.toData());
        if (result.csharp.length > 0) {
            for (let [obj, objId] of result.csharp) {
                parameter.AddReference(objId, obj);
            }
        }
        return parameter;
    }
    utils.encode = encode;
    function decode(parameter) {
        let mapping = new Map();
        let objIds = parameter.GetReferenceKeys();
        if (objIds && objIds.Length > 0) {
            for (let i = 0; i < objIds.Length; i++) {
                let key = objIds.get_Item(i);
                mapping.set(key, parameter.GetReferenceValue(key));
            }
        }
        return _decode(new BufferReader(parameter.Data), mapping);
    }
    utils.decode = decode;
    /**是否受支持的数据类型
     * @param data
     * @returns
     */
    function isSupported(data) {
        return _getObjectLevel(data) !== ObjectLevel.Unsupport;
    }
    utils.isSupported = isSupported;
})(utils || (utils = {}));
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