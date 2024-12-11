import GameObject = CS.UnityEngine.GameObject;
import BindingFlags = CS.System.Reflection.BindingFlags;

export interface ITest {
    /**运行示例的名称 */
    get title(): string;
    /**运行结果 */
    get result(): boolean;
    run(): void | Promise<void>
    destroy(): void
}

export abstract class TestBase extends xor.TsBehaviour implements ITest {
    get title(): string {
        return this.name;
    }
    public result: boolean;
    public run(): void | Promise<void> {
    }
    public destroy() {
        GameObject.Destroy(this.gameObject);
    }
    /**异步等待事件
     * @param time 毫秒数
     */
    protected async waitTime(time: number) {
        await new Promise<void>(resolve => setTimeout(resolve, time));
    }
    /**使用反射模拟调用
     * @param funcname 方法名
     * @param args 方法参数
     */
    protected invokeComponent(funcname: string, ...args: any[]): boolean {
        if (!funcname)
            return false;
        let components = this.gameObject.GetComponents(puerts.$typeof(CS.XOR.Behaviour.Behaviour));
        if (!components || components.Length == 0)
            return false;

        let cacheArgs: CS.System.Array$1<any>
        function getArgs(): CS.System.Array$1<any> {
            if (cacheArgs) {
                return cacheArgs;
            }
            if (!args || args.length == 0)
                return null;
            cacheArgs = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.Object), args.length) as CS.System.Array$1<any>;
            for (let i = 0; i < args.length; i++) {
                cacheArgs.set_Item(i, args[i])
            }
            return cacheArgs;
        }

        for (let i = 0; i < components.Length; i++) {
            let component = components.get_Item(i)
            if (!component || component.Equals(null))
                continue;
            let methodInfo = component.GetType().GetMethod(funcname, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic /**| BindingFlags.Static */);
            if (!methodInfo)
                continue;
            Object.setPrototypeOf(methodInfo, CS.System.Reflection.MethodBase.prototype)
            methodInfo.Invoke(component, getArgs())
        }
        return false;
    }
}

export function name(name: string): ClassDecorator {
    return (target) => {
        Object.defineProperty(target, "name", {
            get: () => name
        })
    }
}