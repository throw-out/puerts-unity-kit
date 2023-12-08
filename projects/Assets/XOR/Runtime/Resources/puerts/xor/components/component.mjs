class TsComponentConstructor extends xor.TsBehaviour {
    constructor(component) {
        super(component, component instanceof CS.XOR.TsComponent ? component : false);
    }
}
const RegisterFlag = Symbol("__guid__");
const RegisterTypes = {};
function guid(guid) {
    return (target) => {
        target[RegisterFlag] = guid;
        RegisterTypes[guid] = target;
    };
}
function route(path) {
    return (target) => {
    };
}
function field(options) {
    return (target, key) => {
    };
}
function dynamic(target) {
    let guid = `dynamic-${Date.now()}`;
    target[RegisterFlag] = guid;
    RegisterTypes[guid] = target;
    return guid;
}
function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    _g.xor.TsComponent = TsComponentConstructor;
    _g.xor.guid = guid;
    _g.xor.route = route;
    _g.xor.field = field;
}
register();
/**重写GetComponent事件, 用于获取 */
function overrideGetComponent() {
    function createGetComponent(original) {
        return function () {
            let ctor = arguments[0];
            if (typeof (ctor) === "function") {
                if (ctor.prototype instanceof TsComponentConstructor) {
                    let guid = ctor[RegisterFlag];
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
                    let guid = ctor[RegisterFlag] || dynamic(ctor), obj = new ctor(this);
                    this.AddTsComponent(guid, obj);
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
overrideGetComponent();
//export to csharp
export function create(component, guid) {
    let ctor = (guid ? RegisterTypes[guid] : null);
    if (ctor && typeof (ctor) === "function") {
        return new ctor(component);
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