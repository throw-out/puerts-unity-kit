import GameObject = CS.UnityEngine.GameObject;
import { ITest, TestBase } from "./_base";

class OnDrawGizmos extends TestBase {
    protected OnDrawGizmos(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnDrawGizmos.name)
    }
}
class OnDrawGizmosSelected extends TestBase {
    protected OnDrawGizmosSelected(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnDrawGizmosSelected.name)
    }
}
class OnSceneGUI extends TestBase {
    protected OnSceneGUI(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnSceneGUI.name)
    }
}
class Reset extends TestBase {
    protected Reset(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.Reset.name)
    }
}
class OnValidate extends TestBase {
    protected OnValidate(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnValidate.name)
    }
}

export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnDrawGizmos,
        OnDrawGizmosSelected,
        OnSceneGUI,
        Reset,
        OnValidate
    ]
}