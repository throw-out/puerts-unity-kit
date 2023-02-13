class TsComponentConstructor extends xor.TsBehaviour {
    constructor(component) {
        super(component, component);
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
                if (RegisterFlag in ctor) {
                    return this.GetTsComponent(ctor[RegisterFlag]);
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
    const Component = CS.UnityEngine.Component, GameObject = CS.UnityEngine.GameObject;
    Component.prototype.GetComponent = createGetComponent(Component.prototype.GetComponent);
    GameObject.prototype.GetComponent = createGetComponent(GameObject.prototype.GetComponent);
    Component.prototype.GetComponents = createGetComponents(Component.prototype.GetComponents);
    GameObject.prototype.GetComponents = createGetComponents(GameObject.prototype.GetComponents);
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
//# sourceMappingURL=component.js.map