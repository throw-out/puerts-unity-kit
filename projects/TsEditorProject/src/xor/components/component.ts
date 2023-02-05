import * as csharp from "csharp";
import { $typeof } from "puerts";

type ConstructorType<T> = Function & { prototype: T };
type NumberConstructor = typeof csharp.System.Byte |
    typeof csharp.System.SByte |
    typeof csharp.System.Char |
    typeof csharp.System.Int16 |
    typeof csharp.System.UInt16 |
    typeof csharp.System.Int32 |
    typeof csharp.System.UInt32 |
    typeof csharp.System.Int64 |
    typeof csharp.System.UInt64;

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
    constructor(component: csharp.XOR.TsComponent) {
        super(component, component);
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
function getConstructor(guid: string): Function {
    return RegisterTypes[guid];
}
function getGuid(type: Function): string {
    return type[RegisterFlag];
}

function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    _g.xor.TsComponent = TsComponentConstructor;
    _g.xor.guid = guid;
    _g.xor.route = route;
    _g.xor.field = field;
    _g.xor.getConstructor = getConstructor;
    _g.xor.getGuid = getGuid;
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
        return function (this: csharp.UnityEngine.GameObject | csharp.UnityEngine.Component) {
            let ctor: any = arguments[0];
            if (typeof (ctor) === "function") {
                if (RegisterFlag in ctor) {
                    return this.GetTsComponent(ctor[RegisterFlag] as string)?.JSObject;
                }
                if (ctor.prototype instanceof csharp.UnityEngine.Component) {
                    return original.call(this, $typeof(ctor));
                }
            }
            return original.apply(this, arguments);
        }
    }

    const Component = csharp.UnityEngine.Component,
        GameObject = csharp.UnityEngine.GameObject;

    Component.prototype.GetComponent = createGetComponent(Component.prototype.GetComponent);
    GameObject.prototype.GetComponent = createGetComponent(GameObject.prototype.GetComponent);
}
overrideGetComponent();

/**重写GetComponent事件, 用于获取 */
declare module "csharp" {
    namespace UnityEngine {
        interface GameObject {
            GetComponent<T extends UnityEngine.Component>(ctor: ConstructorType<T>): T;
            GetComponent<T extends TsComponentConstructor>(ctor: ConstructorType<T>): T;
            //GetTsComponents<T extends TsComponentConstructor=any>(): T[];
        }
        interface Component {
            GetComponent<T extends UnityEngine.Component>(ctor: ConstructorType<T>): T;
            GetComponent<T extends TsComponentConstructor>(ctor: ConstructorType<T>): T;
            //GetTsComponents<T extends TsComponentConstructor=any>(): T[];
        }
    }
}