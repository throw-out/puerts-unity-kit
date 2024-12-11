import { UnityEngine } from "csharp";
import { ITest, TestBase } from "./_base";
import GameObject = CS.UnityEngine.GameObject;

class OnCollisionEnter extends TestBase {
    private data: UnityEngine.Collision
    protected OnCollisionEnter(data: UnityEngine.Collision): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = new UnityEngine.Collision()
        //使用反射模拟C#调用
        this.invokeComponent(this.OnCollisionEnter.name, this.data)
    }
}
class OnCollisionStay extends TestBase {
    private data: UnityEngine.Collision
    protected OnCollisionStay(data: UnityEngine.Collision): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = new UnityEngine.Collision()
        //使用反射模拟C#调用
        this.invokeComponent(this.OnCollisionStay.name, this.data)
    }
}
class OnCollisionExit extends TestBase {
    private data: UnityEngine.Collision
    protected OnCollisionExit(data: UnityEngine.Collision): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = new UnityEngine.Collision()
        //使用反射模拟C#调用
        this.invokeComponent(this.OnCollisionExit.name, this.data)
    }
}

export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnCollisionEnter,
        OnCollisionStay,
        OnCollisionExit,
    ]
}