import * as csharp from "csharp";
type NumberConstructor = typeof csharp.System.Byte | typeof csharp.System.SByte | typeof csharp.System.Char | typeof csharp.System.Int16 | typeof csharp.System.UInt16 | typeof csharp.System.Int32 | typeof csharp.System.UInt32 | typeof csharp.System.Int64 | typeof csharp.System.UInt64;
type FieldOptions = NumberConstructor | Partial<{
    /**指定RawType(原始类型: System.Int16/System.Int32等类型都对应number) */
    type: NumberConstructor;
    /**指定数值范围 */
    range: [min: number, max: number];
    /**默认值 */
    value: any;
}>;
declare class TsComponentConstructor extends xor.TsBehaviour {
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
         * //⚠⚠⚠警告: 此语句由xor自动生成并与class/enum声明绑定, 用户不应该手动创建丶修改丶移动或删除
         * \@xor.guid('global uniqure identifier')
         * export class Example extends xor.TsComponent{
         *      //......
         * }
         * ```
         */
        function guid(guid: number | string): ClassDecorator;
        /**定义组件别名(后续可由此名称Get/Add TsComponent)
         * @param path
         * @example
         * ```
         * //⚠⚠⚠警告: 此语句由xor自动生成并与class/enum声明绑定, 用户不应该手动创建丶修改丶移动或删除
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
export {};
