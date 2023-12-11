class TsComponentConstructor extends xor.Behaviour {
    //--------------------------------------------------------
    get transform() {
        return this.__transform__;
    }
    get gameObject() {
        return this.__gameObject__;
    }
    get component() {
        return this.__component__;
    }
    constructor(object) {
        super();
        let gameObject;
        if (object instanceof CS.XOR.TsBehaviour) {
            gameObject = object.gameObject;
            this.__component__ = object;
        }
        else {
            gameObject = object;
        }
        this.__gameObject__ = gameObject;
        this.__transform__ = gameObject.transform;
    }
    disponse() {
        this.__gameObject__ = undefined;
        this.__transform__ = undefined;
        this.__component__ = undefined;
    }
    bindAll() {
        if (this.__component__) {
            xor.bindAccessor(this, this.__component__, {
                bind: true,
                convertToJsObejct: true,
            });
        }
        //call constructor
        let onctor = this["onConstructor"];
        if (onctor && typeof (onctor) === "function") {
            try {
                onctor.apply(this);
            }
            catch (e) {
                console.error(e.message + "\n" + e.stack);
            }
        }
        //bind methods
        this.bindProxies();
        this.bindUpdateProxies();
        this.bindListeners();
        this.bindModuleInEditor();
    }
}
var utils;
(function (utils) {
    const RegisterFlag = Symbol("__guid__");
    const RegisterTypes = {};
    function guid(guid) {
        return (target) => {
            target[RegisterFlag] = guid;
            RegisterTypes[guid] = target;
        };
    }
    utils.guid = guid;
    function route(path) {
        return (target) => {
        };
    }
    utils.route = route;
    function field(options) {
        return (target, key) => {
        };
    }
    utils.field = field;
    let dynamicTimestamp, dynamicIndex;
    function dynamic(target) {
        let ts = Date.now();
        if (dynamicTimestamp !== ts) {
            dynamicTimestamp = ts;
            dynamicIndex = 0;
        }
        let guid = `dynamic-${dynamicTimestamp}-${dynamicIndex}`;
        target[RegisterFlag] = guid;
        RegisterTypes[guid] = target;
        return guid;
    }
    utils.dynamic = dynamic;
    function getGuid(ctor) {
        return ctor[RegisterFlag];
    }
    utils.getGuid = getGuid;
    function getConstructor(guid) {
        return RegisterTypes[guid];
    }
    utils.getConstructor = getConstructor;
})(utils || (utils = {}));
function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    _g.xor.TsComponent = TsComponentConstructor;
    _g.xor.guid = utils.guid;
    _g.xor.route = utils.route;
    _g.xor.field = utils.field;
}
register();
/**重写GetComponent/AddComponent事件 */
function overrideMethods() {
    function createGetComponent(original) {
        return function () {
            let ctor = arguments[0];
            if (typeof (ctor) === "function") {
                if (ctor.prototype instanceof TsComponentConstructor) {
                    let guid = utils.getGuid(ctor);
                    if (!guid)
                        return null;
                    return this.GetTsComponent(guid);
                }
                if (ctor.prototype instanceof CS.UnityEngine.Component) {
                    return original.call(this, puerts.$typeof(ctor));
                }
            }
            return original.apply(this, arguments);
        };
    }
    function createGetComponents(original) {
        return function () {
            if (arguments[0] && typeof (arguments[0]) === "boolean") {
                let components = this.GetTsComponents();
                if (components) {
                    let results = [];
                    for (let i = 0; i < components.Length; i++) {
                        results.push(components.get_Item(i));
                    }
                    return results;
                }
                return null;
            }
            return original.apply(this, arguments);
        };
    }
    function createAddComponent(original) {
        return function () {
            let ctor = arguments[0];
            if (typeof (ctor) === "function") {
                if (ctor.prototype instanceof TsComponentConstructor) {
                    let guid = utils.getGuid(ctor) || utils.dynamic(ctor), obj = new ctor(this);
                    let component = this.AddTsComponent(guid, obj);
                    obj["__component__"] = component;
                    obj["bindAll"]();
                    return obj;
                }
                if (ctor.prototype instanceof CS.UnityEngine.Component) {
                    return original.call(this, puerts.$typeof(ctor));
                }
            }
            return original.apply(this, arguments);
        };
    }
    const Component = CS.UnityEngine.Component, GameObject = CS.UnityEngine.GameObject;
    Component.prototype.GetComponent = createGetComponent(Component.prototype.GetComponent);
    GameObject.prototype.GetComponent = createGetComponent(GameObject.prototype.GetComponent);
    Component.prototype.GetComponents = createGetComponents(Component.prototype.GetComponents);
    GameObject.prototype.GetComponents = createGetComponents(GameObject.prototype.GetComponents);
    GameObject.prototype.AddComponent = createAddComponent(GameObject.prototype.AddComponent);
}
overrideMethods();
//export to csharp
export function create(component, guid, created) {
    let ctor = guid ? utils.getConstructor(guid) : null;
    if (ctor && typeof (ctor) === "function") {
        let obj = new ctor(component);
        if (created) {
            created.Invoke(component, obj);
        }
        obj["bindAll"]();
        return obj;
    }
    return null;
}
export function invokeMethod(obj, methodName, args) {
    if (!obj || !(methodName in obj))
        return;
    let func = obj[methodName];
    if (typeof (func) !== "function")
        return;
    let _args = [];
    if (args) {
        for (let i = 0; i < args.Length; i++) {
            _args.push(args.get_Item(i));
        }
    }
    func.apply(obj, _args);
}
//# sourceMappingURL=component.js.map