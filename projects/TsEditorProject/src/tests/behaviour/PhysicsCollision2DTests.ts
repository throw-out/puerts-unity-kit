import { UnityEngine } from "csharp";
import { ITest, TestBase } from "./_base";
import GameObject = CS.UnityEngine.GameObject;

class OnCollisionEnter2D extends TestBase {
    private data: UnityEngine.Collision2D
    protected OnCollisionEnter2D(data: UnityEngine.Collision2D): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = new UnityEngine.Collision2D()
        //使用反射模拟C#调用
        this.invokeComponent(this.OnCollisionEnter2D.name, this.data)
    }
}
class OnCollisionStay2D extends TestBase {
    private data: UnityEngine.Collision2D
    protected OnCollisionStay2D(data: UnityEngine.Collision2D): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = new UnityEngine.Collision2D()
        //使用反射模拟C#调用
        this.invokeComponent(this.OnCollisionStay2D.name, this.data)
    }
}
class OnCollisionExit2D extends TestBase {
    private data: UnityEngine.Collision2D
    protected OnCollisionExit2D(data: UnityEngine.Collision2D): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = new UnityEngine.Collision2D()
        //使用反射模拟C#调用
        this.invokeComponent(this.OnCollisionExit2D.name, this.data)
    }
}

export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnCollisionEnter2D,
        OnCollisionStay2D,
        OnCollisionExit2D,
    ]
}