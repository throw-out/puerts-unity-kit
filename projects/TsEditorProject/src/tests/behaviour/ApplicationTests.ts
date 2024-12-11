import GameObject = CS.UnityEngine.GameObject;
import { ITest, TestBase } from "./_base";

class OnApplicationQuit extends TestBase {
    protected OnApplicationQuit(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnApplicationQuit.name)
    }
}
class OnApplicationFocus extends TestBase {
    private p1: boolean
    private p2: boolean
    protected OnApplicationFocus(focus: boolean): void {
        if (focus)
            this.p1 = true
        else
            this.p2 = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnApplicationFocus.name, true)
        this.invokeComponent(this.OnApplicationFocus.name, false)

        this.result = this.p1 && this.p2
    }
}
class OnApplicationPause extends TestBase {
    private p1: boolean
    private p2: boolean
    protected OnApplicationPause(pause: boolean): void {
        if (pause)
            this.p1 = true
        else
            this.p2 = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnApplicationPause.name, true)
        this.invokeComponent(this.OnApplicationPause.name, false)

        this.result = this.p1 && this.p2
    }
}


export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnApplicationQuit,
        OnApplicationFocus,
        OnApplicationPause,
    ]
}