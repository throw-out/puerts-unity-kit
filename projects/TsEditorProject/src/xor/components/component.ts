import * as csharp from "csharp";
import { $typeof } from "puerts";

class TsComponentImpl extends xor.TsBehaviour {

}
namespace TsComponentImpl {

    /**定义组件guid(全局唯一性)
     * @param guid 
     * @returns 
     */
    export function ComponentId(guid: number | string): ClassDecorator {
        return (target) => {

        };
    }
    /**定义组件别名(后续可由此名称Get/Add TsComponent)
     * @param uniqueName 
     * @returns 
     */
    export function ComponentAlias(uniqueName: string): ClassDecorator {
        return (target) => {

        };
    }
}
function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    _g.xor.TsComponent = TsComponentImpl;
}
register();

/**接口声明 */
declare global {
    namespace xor {
        class TsComponent extends TsComponentImpl { }
    }
}