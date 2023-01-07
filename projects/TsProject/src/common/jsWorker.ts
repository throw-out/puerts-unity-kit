//@ts-nocheck
import * as CS from "csharp";
import { $generic } from "puerts";
let List = $generic(CS.System.Collections.Generic.List$1, CS.System.Object);

const CLOSE_EVENT = "__e_close",
    REMOTE_EVENT = "__e_remote";

/**
 * 仅交换纯数据
 * 1. 对象原型链无法传递
 * 2. 方法以字符串方式传递, eval还原. 闭包引用无法传递
 */
class JsWorkerCC {
    public get isAlive() { return this._worker.IsAlive; }
    public get isMainThread() { return this._isMainThread; };

    private _isMainThread: boolean;
    private _worker: CS.JsWorker;
    private _events: Map<string, ((data?: any) => any)[]>;

    constructor(loader: CS.Puerts.ILoader | CS.JsWorker) {
        let worker: CS.JsWorker = undefined;
        if (loader instanceof CS.JsWorker) {
            worker = loader;
            this._isMainThread = false;
        } else {
            worker = CS.JsWorker.Create(loader);
            this._isMainThread = true;
        }
        this._worker = worker;
        this._worker.VerifySafety(this._isMainThread);
        this._events = new Map();
        this.working();
    }

    private working() {
        let getValue = (data: CS.JsWorker.Package) => {
            if (data !== undefined && data !== null && data !== void 0) {
                return this.unpackage(data);
            }
            return undefined;
        };
        let onmessage = (name: string, data: CS.JsWorker.Package): CS.JsWorker.Package => {
            let result = undefined;
            let arr = this._events.get(name);
            if (arr) {
                let o = getValue(data);
                for (let cb of arr) {
                    result = cb(o);
                }
            }
            if (result !== undefined && result !== null && result !== void 0)
                return this.package(result);
            return undefined;
        };
        if (this._isMainThread) {
            this._worker.onMessageOfMain = (name, data) => {
                switch (name) {
                    case CLOSE_EVENT:
                        {
                            let v = getValue(data);
                            let closing = true;
                            let arr = this._events.get(CLOSE_EVENT);
                            if (arr)
                                arr.forEach(cb => {
                                    if ((cb as (data?: any) => boolean)(v) === false)
                                        closing = false;
                                });
                            if (closing)
                                this.dispose();
                            return this.package(closing);
                        }
                    case REMOTE_EVENT:
                        {
                            let value = CS;
                            (getValue(data) as string).split(".").forEach(name => {
                                if (value && name) { value = value[name]; }
                            });
                            let t = typeof (value);
                            if (t !== "undefined" && t !== "object" && t !== "function") {
                                return this.package(value);
                            }
                            return undefined;
                        }
                        break;
                    default:
                        {
                            return onmessage(name, data);
                        }
                }
            };
        }
        else {
            this._worker.onMessageOfChild = onmessage;
            //创建remote, 如何在子线程内访问Unity Api
            let _this = this;
            function createProxy(namespace: string) {
                return new Proxy(Object.create(null), {
                    get: function (cache, name) {
                        if (!(name in cache) && typeof (name) === "string") {
                            let fullName = namespace ? (namespace + '.' + name) : name;
                            //同步调用Unity Api
                            if (fullName.startsWith("UnityEngine") && fullName !== "UnityEngine.Debug") {
                                let cls = _this.postSync(REMOTE_EVENT, fullName);
                                if (cls) {
                                    cache[name] = cls;
                                }
                                else {
                                    cache[name] = createProxy(fullName);
                                }
                            } else {
                                let value = CS;
                                fullName.split(".").forEach(name => {
                                    if (value && name) { value = value[name]; }
                                });
                                cache[name] = value;
                            }
                        }
                        return cache[name];
                    }
                });
            }
            let puerts = require("puerts");
            puerts.registerBuildinModule('csharp', createProxy(undefined));
        }
    }
    private package(data: any, refs?: WeakMap<object, number>, refId?: number): CS.JsWorker.Package {
        refId = refId ?? 1;
        refs = refs ?? new WeakMap();

        let result = new CS.JsWorker.Package();
        let type = typeof (data);
        if ((type === "object" || type === "function") && refs.has(data)) {
            result.type = CS.JsWorker.Type.RefObject;
            result.value = refs.get(data);
        }
        else {
            switch (type) {
                case "object":
                    {
                        //添加引用
                        let id = refId++;
                        result.id = id;
                        refs.set(data, id);
                        //创建C#对象
                        if (data instanceof CS.System.Object) {
                            result.type = CS.JsWorker.Type.Value;
                            result.value = data;
                        }
                        else if (data instanceof ArrayBuffer) {
                            result.type = CS.JsWorker.Type.ArrayBuffer;
                            result.value = CS.JsWorker.Package.ToBytes(data);
                        }
                        else if (Array.isArray(data)) {
                            let list = new List() as CS.System.Collections.Generic.List$1<any>;
                            for (let i = 0; i < data.length; i++) {
                                let item = this.package(data[i], refs, refId);
                                item.info = i;
                                list.Add(item);
                            }
                            result.type = CS.JsWorker.Type.Array;
                            result.value = list;
                        }
                        else {
                            let list = new List() as CS.System.Collections.Generic.List$1<any>;
                            Object.keys(data).forEach(key => {
                                let item = this.package(data[key], refs, refId);
                                item.info = key;
                                list.Add(item);
                            });
                            result.type = CS.JsWorker.Type.Object;
                            result.value = list;
                        }
                    }
                    break;
                case "function":
                    {
                        //添加引用
                        let id = refId++;
                        result.id = id;
                        refs.set(data, id);
                        //创建C#对象
                        result.type = CS.JsWorker.Type.Function;
                        result.value = data.toString();
                    }
                    break;
                case "string":
                case "number":
                case "bigint":
                case "boolean":
                    result.type = CS.JsWorker.Type.Value;
                    result.value = data;
                    break;
                default:
                    result.type = CS.JsWorker.Type.Unknown;
                    break;
            }
        }
        return result;
    }
    private unpackage(data: CS.JsWorker.Package, refs?: Map<number, Object>): any {
        refs = refs ?? new Map();
        let result = undefined, id = data.id, value = data.value;
        switch (data.type) {
            case CS.JsWorker.Type.Object:
                {
                    result = {};
                    if (id > 0) refs.set(id, result); //Add ref object
                    let arr = value as CS.System.Collections.Generic.List$1<CS.JsWorker.Package>;
                    for (let i = 0; i < arr.Count; i++) {
                        let item = arr.get_Item(i);
                        result[item.info] = this.unpackage(item, refs);
                    }
                }
                break;
            case CS.JsWorker.Type.Array:
                {
                    result = [];
                    if (id > 0) refs.set(id, result); //Add ref object
                    let arr = value as CS.System.Collections.Generic.List$1<CS.JsWorker.Package>;
                    for (let i = 0; i < arr.Count; i++) {
                        let item = arr.get_Item(i);
                        result[item.info] = this.unpackage(item, refs);
                    }
                }
                break;
            case CS.JsWorker.Type.ArrayBuffer:
                result = CS.JsWorker.Package.ToArrayBuffer(value);
                if (id > 0) refs.set(id, result); //Add ref object
                break;
            case CS.JsWorker.Type.Function:
                result = eval(value);
                if (id > 0) refs.set(id, result); //Add ref object
                break;
            case CS.JsWorker.Type.RefObject:
                if (refs.has(value))
                    result = refs.get(value);
                else
                    result = "Error: ref id " + value + " not found";
                break;
            case CS.JsWorker.Type.Unknown:
            default:
                result = value;
                if (id > 0) refs.set(id, result); //Add ref object
                break;
        }
        return result;
    }
    public start(filepath: string) {
        if (globalWorker && globalWorker["worker"] == this._worker)
            throw new Error("Thread cannot called start");

        this._worker.Startup(filepath);
    }
    public dispose() {
        let globalWorker = (function () { return this ?? globalThis; })()["globalWorker"];
        if (globalWorker && globalWorker["worker"] == this._worker)
            this.post(CLOSE_EVENT);
        else {
            if (!this._worker.Equals(undefined) && this._worker.gameObject && !this._worker.gameObject.Equals(null)) {
                CS.UnityEngine.Object.Destroy(this._worker.gameObject);
            }
            this._events.clear();
            this._worker.Dispose();
        }
    }
    public post(eventName: string, data?: any) {
        let o: CS.JsWorker.Package;
        if (data !== undefined && data !== null && data !== void 0) {
            o = this.package(data);
        }
        if (this._isMainThread)
            this._worker.CallChild(eventName, o);
        else
            this._worker.CallMain(eventName, o);
    }
    /**
     * 同步发送事件, 并且获取返回值
     */
    public postSync<T>(eventName: string, data?: any): T {
        let o: CS.JsWorker.Package;
        if (data !== undefined && data !== null && data !== void 0) {
            o = this.package(data);
        }

        let result = undefined
        if (this._isMainThread)
            result = this._worker.Sync.CallChild(eventName, o);
        else
            result = this._worker.Sync.CallMain(eventName, o);
        //Result
        if (result !== undefined && result !== null && result !== void 0) {
            result = this.unpackage(result);
        }
        return result;
    }
    /**执行一段代码, 由主线程(外部)调用
     * @param chunk 
     * @param chunkName 
     */
    public eval(chunk: string, chunkName?: string) {
        if (globalWorker && globalWorker["worker"] == this._worker)
            throw new Error("Thread cannot called eval");

        this._worker.Eval(chunk, chunkName);
    }

    /**
     * 监听JsWorker实例close消息, cb返回false将阻止JsWorker实例销毁
     */
    //public on(eventName: "close", cb: (state?: any) => boolean): void;
    /**
     * 监听事件信息
     */
    public on(eventName: string, cb: (data?: any) => any) {
        if (eventName && cb) {
            let arr = this._events.get(eventName);
            if (!arr) {
                arr = [];
                this._events.set(eventName, arr);
            }
            arr.push(cb);
        }
    }
    /**
     * 移除一条监听
     */
    public remove(eventName: string, cb: (data?: any) => void) {
        let arr = this._events.get(eventName);
        if (arr) {
            let index = arr.indexOf(cb);
            if (index >= 0)
                this._events.set(eventName, [...arr.slice(0, index), ...arr.slice(index + 1)]);
        }
    }
    /**移除所有监听
     * @param eventName 移除特定事件监听
     */
    public removeAll(eventName?: string) {
        if (eventName)
            this._events.delete(eventName);
        else
            this._events.clear();
    }
}
(function () {
    let global = (this ?? globalThis);
    global["JsWorker"] = JsWorkerCC;
    global["globalWorker"] = undefined;
})();

/**
 * 接口声明
 */
declare global {
    /**
     * 声明为全局对象
     */
    class JsWorker extends JsWorkerCC {

    }
    class JsWorker_ {
        public get isAlive(): boolean;
        public get isMainThread(): boolean;
        public constructor(loader: CS.Puerts.ILoader);
        /**
         * 开始执行脚本(实例生命周期内仅调用一次) 
         */
        public start(filepath: string): void;
        /**
         * 关闭JsWorker实例, 不可在内部关闭实例
         */
        public dispose(): void;
        /**
         * 发送一条消息(异步)
         */
        public post(eventName: string, data?: any): void;
        /**
         * 同步发送消息并获取返回值
         */
        public postSync<T>(eventName: string, data?: any): T;
        /**
         * 执行一段代码, 由外部程序调用
         */
        public eval(chunk: string, chunkName?: string): void;
        /**
         * 监听事件信息
         */
        public on(eventName: string, cb: (data?: any) => void): void;
        /**
         * 监听并劫持JsWorker实例close消息
         */
        public on(eventName: "close", cb: (state?: any) => boolean): void;
        /**
         * 移除一条监听
         */
        public remove(eventName: string, cb: (data?: any) => void): void;
        /**
         * 移除所有监听
         */
        public removeAll(eventName: string): void;
        /**
         * 移除所有监听
         */
        public removeAll(): void;
    }
    /**
     * 只能在JsWorker内部访问, 与主线程交互的对象
     */
    const globalWorker: JsWorkerCC;
}