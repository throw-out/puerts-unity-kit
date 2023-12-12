import XOR = CS.XOR;
import Transform = CS.UnityEngine.Transform;
import GameObject = CS.UnityEngine.GameObject;
type ConstructorType<T> = Function & {
    prototype: T;
};
type NumberConstructor = typeof CS.System.Byte | typeof CS.System.SByte | typeof CS.System.Char | typeof CS.System.Int16 | typeof CS.System.UInt16 | typeof CS.System.Int32 | typeof CS.System.UInt32 | typeof CS.System.Int64 | typeof CS.System.UInt64;
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
declare class TsComponentConstructor extends xor.Behaviour {
    private __transform__;
    private __gameObject__;
    private __component__;
    get transform(): Transform;
    get gameObject(): GameObject;
    protected get component(): XOR.TsBehaviour;
    constructor(object: GameObject | CS.XOR.TsComponent);
    protected disponse(): void;
    private bindAll;
    /**xor.TsComponent作为序列化类型时, bindAccessor绑定的是Proxy对象, 在访问它时才会获取实际的js对象.
     * 如果直接使用"==="比较同一个序列化对象(xor.TsComponent), 它将返回false. 此方法提供访问原始js对象.
     * @returns
     */
    valueOf(): this;
}
/**接口声明 */
declare global {
    namespace xor {
        class TsComponent extends TsComponentConstructor {
        }
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
         * @example
         * ```
         * ```
         * @param options
         */
        function field(options?: FieldOptions): PropertyDecorator;
    }
}
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
export declare function create(component: CS.XOR.TsComponent, guid: string, created?: CS.System.Action$2<CS.XOR.TsComponent, object>): TsComponentConstructor;
export declare function invokeMethod(obj: object, methodName: string, args: CS.System.Array$1<any>): void;
export {};
