type ConstructorType<T> = Function & { prototype: T };
type NumberConstructor = typeof CS.System.Byte |
    typeof CS.System.SByte |
    typeof CS.System.Char |
    typeof CS.System.Int16 |
    typeof CS.System.UInt16 |
    typeof CS.System.Int32 |
    typeof CS.System.UInt32 |
    typeof CS.System.Int64 |
    typeof CS.System.UInt64;

type FieldOptions = NumberConstructor | Partial<{
    /**指定RawType(原始类型: System.Int16/System.Int32等类型都对应number) */
    type: NumberConstructor;
    /**指定数值范围: 仅可用于number类型 */
    range: [min: number, max: number];
    /**默认值: 只能是基础类型丶C#类型或其数组类型
     * 初始化:
     * PropertyAccess: ts类型必需是字段声明且赋初值, C#类型必需是可在子线程访问的类型
     * New:     ts类型不允许, C#类型必需是可在子线程访问的类型
     */
    value: any;
}>;

class TsComponentConstructor extends xor.TsBehaviour {
    constructor(component: CS.XOR.TsComponent) {
        super(
            component,
            component instanceof CS.XOR.TsComponent ? component : false
        );
    }
}
const RegisterFlag = Symbol("__guid__");
const RegisterTypes: { [guid: string]: Function } = {};

function guid(guid: string): ClassDecorator {
    return (target) => {
        target[RegisterFlag] = guid;
        RegisterTypes[guid] = target;
    };
}
function route(path: string): ClassDecorator {
    return (target) => {

    };
}
function field(options?: FieldOptions): PropertyDecorator {
    return (target, key) => {
    };
}
let dynamicTimestamp: number, dynamicIndex: number;
function dynamic(target: Function): string {
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

function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    _g.xor.TsComponent = TsComponentConstructor;
    _g.xor.guid = guid;
    _g.xor.route = route;
    _g.xor.field = field;
}
register();

/**接口声明 */
declare global {
    namespace xor {
        class TsComponent extends TsComponentConstructor { }
        /**定义组件guid(全局唯一性)
         * @param guid 
         * @example
         * ```
         * //⚠⚠⚠警告: 此语句由xor生成和管理, 且与class声明绑定, 用户不应该手动创建丶修改丶移动或删除
         * \@xor.guid('global uniqure identifier')
         * export class Example extends xor.TsComponent{
         *      //......
         * }
         * ```
         */
        function guid(guid: string): ClassDecorator;
        /**定义组件别名(后续可由此名称Get/Add TsComponent)
         * @param path 
         * @example
         * ```
         * //⚠⚠⚠警告: 此语句由xor生成和管理, 且与class声明绑定, 用户不应该手动创建丶修改丶移动或删除
         * \@xor.route('global unique arbitrary string')
         * export class Example extends xor.TsComponent{
         *      //......
         * }
         * ```
         */
        function route(path: string): ClassDecorator;
        /**定义序列化字段
         * @param options 
         */
        function field(options?: FieldOptions): PropertyDecorator;
    }
}

/**重写GetComponent事件, 用于获取 */
function overrideGetComponent() {
    function createGetComponent(original: Function) {
        return function (this: CS.UnityEngine.GameObject | CS.UnityEngine.Component) {
            let ctor: any = arguments[0];
            if (typeof (ctor) === "function") {
                if (ctor.prototype instanceof TsComponentConstructor) {
                    let guid = ctor[RegisterFlag] as string;
                    if (!guid)
                        return null;
                    return this.GetTsComponent(guid);
                }
                if (ctor.prototype instanceof CS.UnityEngine.Component) {
                    return original.call(this, puerts.$typeof(ctor));
                }
            }
            return original.apply(this, arguments);
        }
    }
    function createGetComponents(original: Function) {
        return function (this: CS.UnityEngine.GameObject | CS.UnityEngine.Component) {
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
        }
    }
    function createAddComponent(original: Function) {
        return function (this: CS.UnityEngine.GameObject) {
            let ctor: any = arguments[0];
            if (typeof (ctor) === "function") {
                if (ctor.prototype instanceof TsComponentConstructor) {
                    let guid = ctor[RegisterFlag] as string || dynamic(ctor),
                        obj = new ctor(this);
                    this.AddTsComponent(guid, obj);
                    return obj;
                }
                if (ctor.prototype instanceof CS.UnityEngine.Component) {
                    return original.call(this, puerts.$typeof(ctor));
                }
            }
            return original.apply(this, arguments);
        }
    }

    const Component = CS.UnityEngine.Component,
        GameObject = CS.UnityEngine.GameObject;

    Component.prototype.GetComponent = createGetComponent(Component.prototype.GetComponent);
    GameObject.prototype.GetComponent = createGetComponent(GameObject.prototype.GetComponent);
    Component.prototype.GetComponents = createGetComponents(Component.prototype.GetComponents);
    GameObject.prototype.GetComponents = createGetComponents(GameObject.prototype.GetComponents);
    GameObject.prototype.AddComponent = createAddComponent(GameObject.prototype.AddComponent);
}
overrideGetComponent();

/**重写GetComponent事件, 用于获取 */
declare module "csharp" {
    namespace UnityEngine {
        interface GameObject {
            GetComponent<T extends UnityEngine.Component>(ctor: ConstructorType<T>): T;
            GetComponent<T extends TsComponentConstructor>(ctor: ConstructorType<T>): T;
            GetComponents<T extends TsComponentConstructor = any>(onlyTsCompnent: true): T[];
            AddComponent<T extends TsComponentConstructor>(ctor: ConstructorType<T>): T;
        }
        interface Component {
            GetComponent<T extends UnityEngine.Component>(ctor: ConstructorType<T>): T;
            GetComponent<T extends TsComponentConstructor>(ctor: ConstructorType<T>): T;
            GetComponents<T extends TsComponentConstructor = any>(onlyTsCompnent: true): T[];
        }
    }
}

//export to csharp
export function create(component: CS.XOR.TsComponent, guid: string): TsComponentConstructor {
    let ctor = (guid ? RegisterTypes[guid] : null) as new (...args: any[]) => TsComponentConstructor;
    if (ctor && typeof (ctor) === "function") {
        return new ctor(component);
    }
    return null;
}
export function invokeMethod(obj: object, methodName: string, args: CS.System.Array$1<any>) {
    if (!obj || !(methodName in obj))
        return;
    let func: Function = obj[methodName];
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