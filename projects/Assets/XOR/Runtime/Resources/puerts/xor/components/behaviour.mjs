var Transform = CS.UnityEngine.Transform;
var RectTransform = CS.UnityEngine.RectTransform;
var Application = CS.UnityEngine.Application;
var Time = CS.UnityEngine.Time;
var CSObject = CS.System.Object;
const { File, Path } = CS.System.IO;
const isEditor = Application.isEditor;
/**
 * 详情参阅: https://docs.unity3d.com/cn/current/ScriptReference/MonoBehaviour.html
 */
class IBehaviour {
}
class IGizmos {
}
class IOnPointerHandler {
}
class IOnDragHandler {
}
class IOnCollision {
}
class IOnCollision2D {
}
class IOnTrigger {
}
class IOnTrigger2D {
}
class IOnMouse {
}
/**
 * 沿用C# MonoBehaviour习惯, 将OnEnable丶Update丶OnEnable等方法绑定到C#对象上, Unity将在生命周期内调用
 *
 * 注: 为避免多次跨语言调用, Update丶FixedUpdate丶LateUpdate方法将由BatchProxy统一管理(并非绑定到各自的GameObject上)
 * @see standalone 如果需要绑定独立的组件, 在对应方法上添加此标注
 */
class BehaviourConstructor {
    //--------------------------------------------------------
    //协程
    StartCoroutine(routine, ...args) {
        //传入了js Generator方法, 转为C#迭代器对象
        var iterator = inner.cs_generator(routine, ...args);
        return this.component.StartCoroutine(iterator);
    }
    StopCoroutine(routine) {
        this.component.StopCoroutine(routine);
    }
    StopAllCoroutines() {
        this.component.StopAllCoroutines();
    }
    /**添加Unity Message listener
     * @param eventName
     * @param fn
     */
    addListener(eventName, fn) {
        //create message proxy
        if (!this.__listenerProxy__ || this.__listenerProxy__.Equals(null)) {
            this.__listenerProxy__ = (this.gameObject.GetComponent(puerts.$typeof(CS.XOR.TsMessages)) ??
                this.gameObject.AddComponent(puerts.$typeof(CS.XOR.TsMessages)));
            this.__listenerProxy__.emptyCallback = () => this._invokeListeners('');
            this.__listenerProxy__.callback = (name, args) => this._invokeListeners(name, args);
        }
        //add listeners
        if (!this.__listeners__) {
            this.__listeners__ = new Map();
        }
        if (eventName === undefined || eventName === null || eventName === void 0)
            eventName = '';
        let functions = this.__listeners__.get(eventName);
        if (!functions) {
            functions = [];
            this.__listeners__.set(eventName, functions);
        }
        functions.push(fn);
    }
    /**移除Unity Message listener
     * @param eventName
     * @param fn
     */
    removeListener(eventName, fn) {
        if (!this.__listeners__)
            return;
        if (eventName === undefined || eventName === null || eventName === void 0)
            eventName = '';
        let functions = this.__listeners__.get(eventName);
        if (!functions)
            return;
        functions = functions.filter(f => f !== fn);
        if (functions.length > 0) {
            this.__listeners__.set(eventName, functions);
        }
        else {
            this.__listeners__.delete(eventName);
            if (this.__listeners__.size === 0)
                this.clearListeners();
        }
    }
    /**移除所有Unity Message listener
     * @param eventName
     */
    removeAllListeners(eventName) {
        if (!this.__listeners__)
            return;
        if (eventName === undefined || eventName === null || eventName === void 0)
            eventName = '';
        this.__listeners__.delete(eventName);
        if (this.__listeners__.size === 0)
            this.clearListeners();
    }
    /**清空Unity Message listener */
    clearListeners() {
        if (!this.__listeners__)
            return;
        this.__listeners__ = null;
        this.__listenerProxy__.callback = null;
        this.__listenerProxy__.emptyCallback = null;
    }
    _invokeListeners(eventName, args) {
        if (!this.__listeners__) {
            console.warn(`invail invoke: ${eventName}`);
            return;
        }
        let functions = this.__listeners__.get(eventName);
        if (!functions)
            return;
        if (args instanceof CS.System.Array) {
            let _args = new Array();
            for (let i = 0; i < args.Length; i++) {
                _args.push(args.get_Item(i));
            }
            args = _args;
        }
        functions.forEach(fn => fn.apply(undefined, args));
    }
    //protected
    disponse() {
        if (this.__componentID__) {
            GlobalManager.unregister(this.__componentID__);
        }
        if (this.__updateElement__) {
            UpdateManager.unregister(this.__updateElement__);
        }
    }
    //绑定生命周期方法
    bindLifecycle() {
        const proto = Object.getPrototypeOf(this);
        const isGlobalInvoker = !!CS.XOR.Behaviour.Invoker.Default;
        if (isGlobalInvoker) {
            this.__componentID__ = this.component.GetObjectID();
            GlobalManager.register(this.__componentID__, this);
        }
        //注册Mono事件
        let methodFlags = 0;
        let specificFlags = CS.XOR.Behaviour.Args.Mono.Awake |
            CS.XOR.Behaviour.Args.Mono.Start |
            CS.XOR.Behaviour.Args.Mono.OnDestroy |
            CS.XOR.Behaviour.Args.Mono.OnEnable |
            CS.XOR.Behaviour.Args.Mono.OnDisable;
        let updateFlags = CS.XOR.Behaviour.Args.Mono.Update |
            CS.XOR.Behaviour.Args.Mono.FixedUpdate |
            CS.XOR.Behaviour.Args.Mono.LateUpdate;
        for (let funcname in CS.XOR.Behaviour.Args.Mono) {
            let v = CS.XOR.Behaviour.Args.Mono[funcname];
            if (typeof (v) != "number")
                continue;
            let hasFunc = typeof (this[funcname]) == "function";
            if ((specificFlags & v) > 0) {
                if (hasFunc)
                    continue;
                specificFlags ^= v;
            }
            else if ((updateFlags & v) > 0) {
                if (!hasFunc) {
                    updateFlags ^= v;
                    continue;
                }
                if (metadata.isDefine(proto, funcname, utils.standalone)) {
                    updateFlags ^= v;
                    methodFlags |= v;
                }
            }
            else {
                if (!hasFunc)
                    continue;
                methodFlags |= v;
            }
        }
        if (specificFlags > 0) { //注册Awake丶Start丶OnDestroy事件
            this.component.CreateMono(specificFlags, isGlobalInvoker ? undefined : (method) => {
                let funcname = CS.XOR.Behaviour.Args.Mono[method];
                switch (method) {
                    case CS.XOR.Behaviour.Args.Mono.Awake:
                    case CS.XOR.Behaviour.Args.Mono.Start:
                    case CS.XOR.Behaviour.Args.Mono.OnDestroy:
                        inner.invoke(this, funcname, true);
                        break;
                    default:
                        inner.invoke(this, funcname, false);
                        break;
                }
            });
        }
        if (methodFlags > 0) { //注册剩余Mono事件
            this.component.CreateMono(methodFlags, isGlobalInvoker ? undefined : (method) => this[CS.XOR.Behaviour.Args.Mono[method]]());
        }
        if (updateFlags > 0) {
            let element = new UpdateManager.Element(updateFlags, this);
            this.__updateElement__ = element;
            UpdateManager.register(element);
            if (isGlobalInvoker) {
                element.enabled = !this.component.IsDestroyed && this.component.IsEnable;
            }
            else {
                //生命周期管理
                this.component.CreateMono(CS.XOR.Behaviour.Args.Mono.OnEnable | CS.XOR.Behaviour.Args.Mono.OnDisable | CS.XOR.Behaviour.Args.Mono.OnDestroy, (method) => {
                    switch (method) {
                        case CS.XOR.Behaviour.Args.Mono.OnEnable:
                            element.enabled = true;
                            break;
                        case CS.XOR.Behaviour.Args.Mono.OnDisable:
                            element.enabled = false;
                            break;
                        case CS.XOR.Behaviour.Args.Mono.OnDestroy:
                            element.enabled = false;
                            UpdateManager.unregister(element);
                            break;
                    }
                });
            }
        }
        //注册Gizmos事件
        if (isEditor) {
            methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.Gizmos);
            if (methodFlags > 0) {
                this.component.CreateGizmos(methodFlags, isGlobalInvoker ? undefined : (method) => this[CS.XOR.Behaviour.Args.Gizmos[method]]());
            }
        }
        //注册Mouse事件
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.Mouse);
        if (methodFlags > 0) {
            this.component.CreateMouse(methodFlags, isGlobalInvoker ? undefined : (method) => this[CS.XOR.Behaviour.Args.Mouse[method]]());
        }
        //注册MonoBoolean事件
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.MonoBoolean);
        if (methodFlags > 0) {
            this.component.CreateMonoBoolean(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.MonoBoolean[method]](data));
        }
        //注册EventSystems事件
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.EventSystems);
        if (methodFlags > 0) {
            this.component.CreateEventSystems(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.EventSystems[method]](data));
        }
        //注册Physics事件
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.PhysicsCollider);
        if (methodFlags > 0) {
            this.component.CreatePhysicsCollider(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.PhysicsCollider[method]](data));
        }
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.PhysicsCollider2D);
        if (methodFlags > 0) {
            this.component.CreatePhysicsCollider2D(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.PhysicsCollider2D[method]](data));
        }
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.PhysicsCollision);
        if (methodFlags > 0) {
            this.component.CreatePhysicsCollision(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.PhysicsCollision[method]](data));
        }
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.PhysicsCollision2D);
        if (methodFlags > 0) {
            this.component.CreatePhysicsCollision2D(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.PhysicsCollision2D[method]](data));
        }
    }
    bindListeners() {
        const proto = Object.getPrototypeOf(this);
        for (let funcname of metadata.getKeys(proto)) {
            let eventName = metadata.getDefineData(proto, funcname, utils.listener);
            if (!eventName)
                continue;
            let waitAsyncComplete = metadata.getDefineData(proto, funcname, utils.throttle, false);
            let func = inner.bind(this, funcname, waitAsyncComplete);
            if (!func)
                return undefined;
            this.addListener(eventName, func);
        }
    }
    //绑定脚本内容
    bindModuleInEditor() {
        if (!isEditor || !this.gameObject || this.gameObject.Equals(null))
            return;
        //堆栈信息
        let stack = new Error().stack
            .replace(/\r\n/g, "\n")
            .split('\n')
            .slice(2)
            .join("\n");
        //class名称
        let className = Object.getPrototypeOf(this).constructor.name;
        let moduleName, modulePath, moduleLine, moduleColumn;
        //匹配new构造函数
        //let regex = /at [a-zA-z0-9#$._ ]+ \(([a-zA-Z0-9:/\\._ ]+(.js|.ts))\:([0-9]+)\:([0-9]+)\)/g;
        let regex = /at [a-zA-z0-9#$._ ]+ \(([^\n\r\*\"\|\<\>]+(.js|.cjs|.mjs|.ts|.mts))\:([0-9]+)\:([0-9]+)\)/g;
        let match;
        while (match = regex.exec(stack)) {
            let isConstructor = match[0].includes("at new "); //是否调用构造对象函数
            if (isConstructor) {
                let path = match[1].replace(/\\/g, "/");
                let line = match[3], column = match[4];
                if (path.endsWith(".js") || path.endsWith(".cjs") || path.endsWith(".mjs")) {
                    //class在声明变量时赋初值, 构造函数中sourceMap解析会失败: 故此处尝试读取js.map文件手动解析
                    try {
                        let mapPath = path + ".map", tsPath;
                        let sourceMap = File.Exists(mapPath) ? JSON.parse(File.ReadAllText(mapPath)) : null;
                        if (sourceMap && Array.isArray(sourceMap.sources) && sourceMap.sources.length == 1) {
                            tsPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), sourceMap.sources[0]));
                        }
                        if (File.Exists(tsPath)) {
                            path = tsPath;
                            line = column = "0";
                            //尝试寻常class信息
                            let lines = File.ReadAllLines(tsPath);
                            for (let i = 0; i < lines.Length; i++) {
                                let content = lines.get_Item(i);
                                if (content.indexOf(`class ${className}`) >= 0 || content.indexOf(`function ${className}`) >= 0) {
                                    line = (i + 1).toString();
                                    break;
                                }
                            }
                        }
                    }
                    catch (e) {
                        console.warn(e);
                    }
                }
                modulePath = path;
                moduleName = path.substring(path.lastIndexOf("/") + 1);
                moduleLine = parseInt(line ?? "0");
                moduleColumn = parseInt(column ?? "0");
            }
            else if (modulePath) {
                break;
            }
        }
        let module = new CS.XOR.ModuleInfo();
        module.className = className;
        if (modulePath) {
            module.moduleName = moduleName;
            module.modulePath = modulePath;
            module.line = moduleLine;
            module.column = moduleColumn;
            module.stack = stack;
        }
        else {
            console.warn(`Unresolved Module: ${className}\n${stack}`);
        }
        this.component["Module"] = module;
    }
    get enabled() { return this.component.enabled; }
    set enabled(value) { this.component.enabled = value; }
    get isActiveAndEnabled() { return this.component.isActiveAndEnabled; }
    get tag() { return this.gameObject.tag; }
    set tag(value) { this.gameObject.tag = value; }
    get name() { return this.gameObject.name; }
    set name(value) { this.gameObject.name = value; }
    get rectTransform() {
        if (!this.transform)
            return undefined;
        if (!("__rectTransform__" in this)) {
            this["__rectTransform__"] = this.transform instanceof RectTransform ? this.transform : null;
        }
        return this["__rectTransform__"];
    }
}
class TsBehaviourConstructor extends BehaviourConstructor {
    //--------------------------------------------------------
    get transform() {
        return this.__transform__;
    }
    get gameObject() {
        return this.__gameObject__;
    }
    get component() {
        if (!this.__component__ || this.__component__.Equals(null)) {
            this.__component__ = this.__gameObject__.GetComponent(puerts.$typeof(CS.XOR.TsBehaviour));
            if (!this.__component__ || this.__component__.Equals(null)) {
                this.__component__ = this.__gameObject__.AddComponent(puerts.$typeof(CS.XOR.TsBehaviour));
            }
        }
        return this.__component__;
    }
    constructor() {
        super();
        let object = arguments[0], accessor, args, before, after;
        let p2 = arguments[1];
        switch (typeof (p2)) {
            case "object":
                if (p2 === null) { }
                else if (p2 instanceof CSObject || Array.isArray(p2)) {
                    accessor = p2;
                }
                else {
                    accessor = p2.accessor;
                    args = p2.args;
                    before = p2.before;
                    after = p2.after;
                }
                break;
            case "boolean":
                accessor = p2;
                break;
        }
        let gameObject;
        if (object instanceof CS.XOR.TsBehaviour) {
            gameObject = object.gameObject;
            this.__component__ = object;
        }
        else if (object instanceof Transform) {
            gameObject = object.gameObject;
        }
        else {
            gameObject = object;
        }
        this.__gameObject__ = gameObject;
        this.__transform__ = gameObject.transform;
        //call before callback
        if (before)
            inner.invoke(this, before, true);
        //bind properties
        if (accessor === undefined || accessor === true) {
            utils.bindAccessor(this, object.GetComponents(puerts.$typeof(CS.XOR.TsProperties)), true);
        }
        else if (accessor) {
            utils.bindAccessor(this, accessor, true);
        }
        //call constructor
        let onctor = this["onConstructor"];
        if (onctor && typeof (onctor) === "function") {
            if (args && args.length > 0) {
                inner.invoke(this, onctor, true, ...args);
            }
            else {
                inner.invoke(this, onctor, true);
            }
        }
        //bind methods
        this.bindLifecycle();
        this.bindListeners();
        this.bindModuleInEditor();
        //call after callback
        if (after)
            inner.invoke(this, after, true);
    }
    disponse() {
        this.__gameObject__ = undefined;
        this.__transform__ = undefined;
        this.__component__ = undefined;
    }
    /**注册全局生命周期回调, 每个TsBehaviour实例不再单独创建多个生命周期回调绑定 */
    static registerGlobalInvoker() {
        GlobalManager.init();
    }
}
/**全局对象管理 */
class GlobalManager {
    static register(objectID, obj) {
        this.objects.set(objectID, obj);
    }
    static unregister(objectID) {
        this.objects.delete(objectID);
    }
    static init() {
        let invoker = new CS.XOR.Behaviour.Invoker();
        invoker.mono = (objectID, method) => {
            switch (method) {
                case CS.XOR.Behaviour.Args.Mono.Awake:
                case CS.XOR.Behaviour.Args.Mono.Start:
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], true);
                    break;
                case CS.XOR.Behaviour.Args.Mono.OnDestroy:
                    this.unregister(objectID);
                    this.setUpdateElement(objectID, false);
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], true);
                    break;
                case CS.XOR.Behaviour.Args.Mono.OnEnable:
                    this.setUpdateElement(objectID, true);
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], false);
                    break;
                case CS.XOR.Behaviour.Args.Mono.OnDisable:
                    this.setUpdateElement(objectID, false);
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], false);
                    break;
                default:
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], false);
                    break;
            }
        };
        invoker.monoBoolean = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.MonoBoolean[method], false, data);
        invoker.gizmos = (objectID, method) => this.invoke(objectID, CS.XOR.Behaviour.Args.Gizmos[method], false);
        invoker.mouse = (objectID, method) => this.invoke(objectID, CS.XOR.Behaviour.Args.Mouse[method], false);
        invoker.eventSystems = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.EventSystems[method], false, data);
        invoker.collision = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.PhysicsCollider[method], false, data);
        invoker.collision2D = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.PhysicsCollider2D[method], false, data);
        invoker.collision = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.PhysicsCollision[method], false, data);
        invoker.collision2D = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.PhysicsCollision2D[method], false, data);
        invoker.destroy = (objectID) => this.unregister(objectID);
        CS.XOR.Behaviour.Invoker.Default = invoker;
    }
    static invoke(objectID, funcname, catchExecption, ...args) {
        let obj = this.objects.get(objectID);
        if (!obj)
            return;
        inner.invoke(obj, funcname, catchExecption, ...args);
    }
    static setUpdateElement(objectID, enabled) {
        let obj = this.objects.get(objectID);
        if (!obj || !obj["__updateElement__"])
            return;
        obj["__updateElement__"].enabled = enabled;
    }
}
GlobalManager.objects = new Map();
/**Update批量调用管理 */
class UpdateManager {
    static register(element) {
        if (element.isUpdate) {
            this.update.add(element, element.updateSkipFrame);
        }
        if (element.isLateUpdate) {
            this.lateUpdate.add(element, element.lateUpdateSkipFrame);
        }
        if (element.isFixedUpdate) {
            this.fixedUpdate.add(element, element.fixedUpdateSkipFrame);
        }
        if (!this._init) {
            this._init = true;
            this.init();
        }
    }
    static unregister(element) {
        if (element.isUpdate) {
            this.update.remove(element, element.updateSkipFrame);
        }
        if (element.isLateUpdate) {
            this.lateUpdate.remove(element, element.lateUpdateSkipFrame);
        }
        if (element.isFixedUpdate) {
            this.fixedUpdate.remove(element, element.fixedUpdateSkipFrame);
        }
    }
    static init() {
        let go = new CS.UnityEngine.GameObject(`[${UpdateManager.name}]`);
        go.transform.SetParent(CS.XOR.Application.GetInstance().transform);
        go.AddComponent(puerts.$typeof(CS.XOR.Behaviour.Default.UpdateBehaviour)).Callback = () => {
            this.update.tick(Time.deltaTime);
        };
        go.AddComponent(puerts.$typeof(CS.XOR.Behaviour.Default.LateUpdateBehaviour)).Callback = () => {
            this.lateUpdate.tick(Time.deltaTime);
        };
        go.AddComponent(puerts.$typeof(CS.XOR.Behaviour.Default.FixedUpdateBehaviour)).Callback = () => {
            this.fixedUpdate.tick(Time.fixedDeltaTime);
        };
    }
}
UpdateManager.Elements = class {
    constructor(lifecycle) {
        this.every = [];
        this.frame = new Map();
        this.lifecycle = lifecycle;
    }
    add(element, skip) {
        if (skip <= 0) {
            this.every.push(element);
            return;
        }
        let state = this.frame.get(skip);
        if (!state) {
            state = { tick: 0, dt: 0, elements: [] };
            this.frame.set(skip, state);
        }
        state.elements.push(element);
    }
    remove(element, skip) {
        const units = skip > 0 ? this.frame.get(skip)?.elements : this.every;
        if (!units || units.length <= 0)
            return;
        const index = units.indexOf(element);
        if (index >= 0) {
            units.splice(index, 1);
        }
    }
    tick(dt) {
        //每帧调用
        if (this.every.length > 0) {
            for (let element of this.every) {
                element.invoke(this.lifecycle, dt);
            }
        }
        //跨帧调用
        for (const [t, state] of this.frame) {
            state.dt += dt;
            if ((--state.tick) > 0)
                continue;
            if (state.elements.length > 0) {
                for (let element of state.elements) {
                    element.invoke(this.lifecycle, state.dt);
                }
            }
            state.tick = t;
            state.dt = 0;
        }
    }
};
UpdateManager.update = new UpdateManager.Elements(CS.XOR.Behaviour.Args.Mono.Update);
UpdateManager.lateUpdate = new UpdateManager.Elements(CS.XOR.Behaviour.Args.Mono.LateUpdate);
UpdateManager.fixedUpdate = new UpdateManager.Elements(CS.XOR.Behaviour.Args.Mono.FixedUpdate);
(function (UpdateManager) {
    class Element {
        constructor(methods, target) {
            this.target = target;
            this.enabled = false;
            this.isUpdate = (methods & CS.XOR.Behaviour.Args.Mono.Update) > 0;
            this.isLateUpdate = (methods & CS.XOR.Behaviour.Args.Mono.LateUpdate) > 0;
            this.isFixedUpdate = (methods & CS.XOR.Behaviour.Args.Mono.FixedUpdate) > 0;
            const proto = Object.getPrototypeOf(this);
            this.updateSkipFrame = this.isUpdate ? metadata.getDefineData(proto, CS.XOR.Behaviour.Args.Mono[CS.XOR.Behaviour.Args.Mono.Update], utils.frameskip, 0) : 0;
            this.lateUpdateSkipFrame = this.isLateUpdate ? metadata.getDefineData(proto, CS.XOR.Behaviour.Args.Mono[CS.XOR.Behaviour.Args.Mono.LateUpdate], utils.frameskip, 0) : 0;
            this.fixedUpdateSkipFrame = this.isFixedUpdate ? metadata.getDefineData(proto, CS.XOR.Behaviour.Args.Mono[CS.XOR.Behaviour.Args.Mono.FixedUpdate], utils.frameskip, 0) : 0;
        }
        invoke(lifecycle, dt) {
            if (!this.enabled)
                return;
            inner.invoke(this.target, CS.XOR.Behaviour.Args.Mono[lifecycle], false, dt);
        }
    }
    UpdateManager.Element = Element;
})(UpdateManager || (UpdateManager = {}));
var inner;
(function (inner) {
    /**将对象与方法绑定
     * @param thisArg
     * @param funcname
     * @param waitAsyncComplete
     * @returns
     */
    function bind(thisArg, funcname, waitAsyncComplete) {
        const func = typeof (funcname) === "string" ? thisArg[funcname] : funcname;
        if (func !== undefined && typeof (func) === "function") {
            //return (...args: any[]) => func.call(thisArg, ...srcArgs, ...args);
            if (waitAsyncComplete) {
                let executing = false;
                return function (...args) {
                    if (executing)
                        return;
                    let result = func.call(thisArg, ...args);
                    if (result instanceof Promise) {
                        executing = true; //wait async function finish
                        result.finally(() => executing = false);
                    }
                    return result;
                };
            }
            return function (...args) {
                return func.call(thisArg, ...args);
            };
        }
        return undefined;
    }
    inner.bind = bind;
    /**调用方法
     * @param thisArg
     * @param funcname
     * @param catchException 是否捕获异常
     * @param args
     */
    function invoke(thisArg, funcname, catchException, ...args) {
        const func = typeof (funcname) === "string" ? thisArg[funcname] : funcname;
        if (!catchException) {
            return func.apply(thisArg, args);
        }
        //带try catch捕获异常的调用
        try {
            return func.apply(thisArg, args);
        }
        catch (e) {
            console.error(e.message + "\n" + e.stack);
        }
        return undefined;
    }
    inner.invoke = invoke;
    /**创建C#迭代器
     * @param func
     * @param args
     * @returns
     */
    function cs_generator(func, ...args) {
        let generator = undefined;
        if (typeof (func) === "function") {
            generator = func(...args);
            if (generator === null || generator === undefined || generator === void 0)
                throw new Error("Function '" + func?.name + "' no return Generator");
        }
        else {
            generator = func;
        }
        return CS.XOR.IEnumeratorUtil.Generator(function () {
            let tick;
            try {
                let next = generator.next();
                tick = new CS.XOR.IEnumeratorUtil.Tick(next.value, next.done);
            }
            catch (e) {
                tick = new CS.XOR.IEnumeratorUtil.Tick(null, true);
                console.error(e.message + "\n" + e.stack);
            }
            return tick;
        });
    }
    inner.cs_generator = cs_generator;
    /**导出方法枚举映射值
     * @param obj
     * @param types
     * @returns
     */
    function getFunctionFlags(obj, types) {
        let results = 0;
        for (let funcname in types) {
            let v = types[funcname];
            if (typeof (v) != "number")
                continue;
            if (typeof (obj[funcname]) != "function")
                continue;
            results |= v;
        }
        return results;
    }
    inner.getFunctionFlags = getFunctionFlags;
})(inner || (inner = {}));
var metadata;
(function (metadata) {
    const MATEDATA_INFO = Symbol("__MATEDATA_INFO__");
    function define(proto, key, attribute, data) {
        let matedatas = proto[MATEDATA_INFO];
        if (!matedatas) {
            matedatas = proto[MATEDATA_INFO] = {};
        }
        let attributes = matedatas[key];
        if (!attributes) {
            attributes = matedatas[key] = [];
        }
        attributes.push({ attribute, data });
    }
    metadata.define = define;
    function getKeys(proto) {
        let matedatas = proto[MATEDATA_INFO];
        return matedatas ? Object.keys(matedatas) : [];
    }
    metadata.getKeys = getKeys;
    function isDefine(proto, key, attribute) {
        let matedatas = proto[MATEDATA_INFO];
        if (!matedatas) {
            return false;
        }
        let attributes = matedatas[key];
        if (!attributes) {
            return false;
        }
        return !!attributes.find(define => define.attribute === attribute);
    }
    metadata.isDefine = isDefine;
    function getDefineData(proto, key, attribute, defaultValue) {
        let matedatas = proto[MATEDATA_INFO];
        if (!matedatas) {
            return defaultValue;
        }
        let attributes = matedatas[key];
        if (!attributes) {
            return defaultValue;
        }
        return attributes.find(define => define.attribute === attribute)?.data ?? defaultValue;
    }
    metadata.getDefineData = getDefineData;
})(metadata || (metadata = {}));
var utils;
(function (utils) {
    function toCSharpArray(array, checkMemberType = true) {
        if (!array || array.length === 0)
            return null;
        let firstIndex = array.findIndex(m => m !== undefined && m !== null && m !== void 0) ?? -1;
        if (firstIndex < 0)
            return null;
        let first = array[firstIndex];
        let results, type = typeof first, memberType;
        switch (type) {
            case "bigint":
                results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.Int64), array.length);
                break;
            case "number":
                results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.Double), array.length);
                break;
            case "string":
                results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.String), array.length);
                break;
            case "boolean":
                results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.Boolean), array.length);
                break;
            case "object":
                if (first instanceof CS.System.Object) {
                    results = CS.System.Array.CreateInstance(first.GetType(), array.length);
                }
                break;
        }
        if (results) {
            for (let i = 0; i < array.length; i++) {
                let value = array[i];
                if (checkMemberType) {
                    if (!memberType && typeof (value) !== type) {
                        continue;
                    }
                    if (memberType && (typeof (value) !== "object" ||
                        !(value instanceof CS.System.Object) ||
                        !memberType.IsAssignableFrom(value.GetType()))) {
                        continue;
                    }
                }
                results.SetValue(value, i);
            }
        }
        return results;
    }
    function toArray() {
        let array = arguments[0];
        if (!array)
            return null;
        let results = new Array();
        for (let i = 0; i < array.Length; i++) {
            results.push(array.GetValue(i));
        }
        return results;
    }
    const WatchFlag = Symbol("--watch--");
    const WatchFunctions = ["pop", "push", "reverse", "shift", "sort", "splice", "unshift"];
    function watch(obj, change) {
        if (!obj || !Array.isArray(obj))
            return obj;
        let functions = {};
        Object.defineProperty(obj, WatchFlag, {
            value: change,
            configurable: true,
            enumerable: false,
            writable: false,
        });
        return new Proxy(obj, {
            get: function (target, property) {
                if (WatchFlag in target && WatchFunctions.includes(property)) {
                    if (!(property in functions)) {
                        functions[property] = new Proxy(Array.prototype[property], {
                            apply: function (target, thisArg, argArray) {
                                let result = target.apply(thisArg, argArray);
                                if (WatchFlag in thisArg) {
                                    thisArg[WatchFlag]();
                                }
                                return result;
                            }
                        });
                    }
                    return functions[property];
                }
                return target[property];
            },
            set: function (target, property, newValue) {
                target[property] = newValue;
                if (WatchFlag in target) {
                    target[WatchFlag]();
                }
                return true;
            }
        });
    }
    function unwatch(obj) {
        if (!obj || !Array.isArray(obj))
            return;
        delete obj[WatchFlag];
    }
    const OriginFlag = Symbol("--origin--"), ProxyFlag = Symbol("--proxy--");
    function convertToJsObejctProxy(component) {
        if (!component || !(component instanceof CS.XOR.TsComponent))
            return component;
        if (!component.Guid)
            return undefined;
        let proxy = component[ProxyFlag];
        if (proxy === undefined || proxy === null || proxy === void 0) {
            let target;
            function getter() {
                if (!component)
                    return;
                if (component.Equals(null)) {
                    component = null;
                    return;
                }
                if (!CS.XOR.TsComponent.IsRegistered())
                    throw new Error("XOR.TsComponet.Register is required.");
                if (component.IsPending)
                    CS.XOR.TsComponent.Resolve(component, true);
                target = component.JSObject;
                component = null;
            }
            ;
            proxy = new Proxy({}, {
                apply: function (_, thisArg, argArray) {
                    getter();
                    target.apply(thisArg, argArray);
                },
                construct: function (_, argArray, newTarget) {
                    getter();
                    return new target(...argArray);
                },
                get: function (_, property) {
                    getter();
                    if (property === OriginFlag && target)
                        return target;
                    return target[property];
                },
                set: function (_, property, newValue) {
                    getter();
                    target[property] = newValue;
                    return true;
                },
                defineProperty: function (_, property, attributes) {
                    getter();
                    Object.defineProperty(target, property, attributes);
                    return true;
                },
                deleteProperty: function (_, property) {
                    getter();
                    delete target[property];
                    return true;
                },
                getOwnPropertyDescriptor: function (_, property) {
                    getter();
                    return Object.getOwnPropertyDescriptor(target, property);
                },
                getPrototypeOf: function (_) {
                    getter();
                    return Object.getPrototypeOf(target);
                },
                setPrototypeOf: function (_, newValue) {
                    getter();
                    Object.setPrototypeOf(target, newValue);
                    return true;
                },
                has: function (_, property) {
                    getter();
                    return property in target;
                },
                isExtensible: function (_) {
                    getter();
                    return Object.isExtensible(target);
                },
                ownKeys: function (_) {
                    getter();
                    return Reflect.ownKeys(target)?.filter(key => Object.getOwnPropertyDescriptor(target, key)?.configurable);
                },
                preventExtensions: function (_) {
                    getter();
                    Object.preventExtensions(target);
                    return true;
                },
            });
            Object.defineProperty(component, ProxyFlag, {
                get: () => proxy,
                enumerable: false,
                configurable: true,
            });
        }
        return proxy;
    }
    function createJsObjectProxies(properties) {
        let c2jsKeys = [];
        for (let key of Object.keys(properties)) {
            let val = properties[key];
            if (val && val instanceof CS.XOR.TsComponent) {
                properties[key] = convertToJsObejctProxy(val);
                c2jsKeys.push(key);
            }
            else if (val && Array.isArray(val)) {
                for (let i = 0; i < val.length; i++) {
                    if (val[i] && val[i] instanceof CS.XOR.TsComponent) {
                        val[i] = convertToJsObejctProxy(val[i]);
                    }
                }
            }
        }
        return c2jsKeys;
    }
    function getAccessorProperties(accessor) {
        let results = {};
        let properties = accessor.GetProperties();
        if (properties && properties.Length > 0) {
            for (let i = 0; i < properties.Length; i++) {
                let { key, value } = properties.get_Item(i);
                if (value && value instanceof CS.System.Array) {
                    value = toArray(value);
                }
                results[key] = value;
            }
        }
        return results;
    }
    utils.getAccessorProperties = getAccessorProperties;
    function bindAccessor() {
        let object = arguments[0], accessor = arguments[1], bind, c2js;
        if (!accessor)
            return;
        switch (typeof (arguments[2])) {
            case "boolean":
                bind = arguments[2];
                break;
            case "object":
                bind = arguments[2]?.bind;
                c2js = arguments[2]?.convertToJsObejct;
                break;
        }
        let list = accessor instanceof CS.System.Array ? toArray(accessor) : Array.isArray(accessor) ? accessor : [accessor];
        for (let accessor of list) {
            if (!accessor || accessor.Equals(null))
                continue;
            let properties = getAccessorProperties(accessor), keys = Object.keys(properties);
            if (keys.length === 0)
                continue;
            let c2jsKeys = c2js ? createJsObjectProxies(properties) : null;
            if (isEditor && bind) {
                let setValue = (key, newValue) => {
                    unwatch(properties[key]);
                    properties[key] = watch(newValue, () => {
                        accessor.SetProperty(key, Array.isArray(newValue) ? toCSharpArray(newValue) : newValue);
                    });
                };
                accessor.SetPropertyListener((key, newValue) => {
                    if (newValue && newValue instanceof CS.System.Array) {
                        newValue = toArray(newValue);
                    }
                    setValue(key, newValue);
                });
                for (let key of keys) {
                    if (key in object) {
                        console.warn(`Object ${object}(${object["name"]}) already exists prop '${key}' ---> ${object[key]}`);
                    }
                    if (!c2jsKeys || !c2jsKeys.includes(key)) {
                        setValue(key, properties[key]);
                    }
                    Object.defineProperty(object, key, {
                        get: () => properties[key],
                        set: (newValue) => {
                            setValue(key, newValue);
                            accessor.SetProperty(key, Array.isArray(newValue) ? toCSharpArray(newValue) : newValue);
                        },
                        configurable: true,
                        enumerable: true,
                    });
                }
            }
            else {
                Object.assign(object, properties);
            }
        }
    }
    utils.bindAccessor = bindAccessor;
    function getAccessorPropertyOrigin(val) {
        if (!val || typeof (val) !== "object")
            return val;
        return val[OriginFlag] ?? val;
    }
    utils.getAccessorPropertyOrigin = getAccessorPropertyOrigin;
    function standalone() {
        return (target, key) => {
            let proto = target.constructor.prototype;
            if (!(proto instanceof TsBehaviourConstructor)) {
                console.warn(`${target.constructor.name}: invaild decorator ${standalone.name}`);
                return;
            }
            metadata.define(proto, key, standalone);
        };
    }
    utils.standalone = standalone;
    function frameskip(value) {
        return (target, key) => {
            let proto = target.constructor.prototype;
            if (!(proto instanceof TsBehaviourConstructor)) {
                console.warn(`${target.constructor.name}: invaild decorator ${frameskip.name}`);
                return;
            }
            if (!Number.isInteger(value) || value <= 1) {
                console.warn(`${target.constructor.name}: invaild decorator parameter ${value} for ${frameskip.name}`);
                return;
            }
            metadata.define(proto, key, frameskip, value);
        };
    }
    utils.frameskip = frameskip;
    function throttle(enable) {
        return (target, key) => {
            let proto = target.constructor.prototype;
            if (!(proto instanceof TsBehaviourConstructor)) {
                console.warn(`${target.constructor.name}: invaild decorator ${throttle.name}`);
                return;
            }
            metadata.define(proto, key, throttle, !!enable);
        };
    }
    utils.throttle = throttle;
    function listener(eventName) {
        return (target, key) => {
            let proto = target.constructor.prototype;
            if (!(proto instanceof TsBehaviourConstructor)) {
                console.warn(`${target.constructor.name}: invaild decorator ${listener.name}`);
                return;
            }
            metadata.define(proto, key, listener, eventName ?? key);
        };
    }
    utils.listener = listener;
})(utils || (utils = {}));
function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    Object.assign(_g.xor, utils);
    Object.assign(TsBehaviourConstructor, utils);
    _g.xor.Behaviour = BehaviourConstructor;
    _g.xor.TsBehaviour = TsBehaviourConstructor;
}
register();
export {};
//# sourceMappingURL=behaviour.js.map