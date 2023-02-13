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
declare class TsComponentConstructor extends xor.TsBehaviour {
    constructor(component: CS.XOR.TsComponent);
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
         * @param options
         */
        function field(options?: FieldOptions): PropertyDecorator;
    }
}
/**重写GetComponent事件, 用于获取 */
declare module "csharp" {
    namespace UnityEngine {
        interface GameObject {
            GetComponent<T extends UnityEngine.Component>(ctor: ConstructorType<T>): T;
            GetComponent<T extends TsComponentConstructor>(ctor: ConstructorType<T>): T;
            GetComponents<T extends TsComponentConstructor = any>(onlyTsCompnent: true): T[];
        }
        interface Component {
            GetComponent<T extends UnityEngine.Component>(ctor: ConstructorType<T>): T;
            GetComponent<T extends TsComponentConstructor>(ctor: ConstructorType<T>): T;
            GetComponents<T extends TsComponentConstructor = any>(onlyTsCompnent: true): T[];
        }
    }
}
export declare function create(component: CS.XOR.TsComponent, guid: string): TsComponentConstructor;
export {};
