import { UnityEngine } from "csharp";
import { ITest, TestBase } from "./_base";
import GameObject = CS.UnityEngine.GameObject;

class OnTriggerEnter2D extends TestBase {
    private data: UnityEngine.Collider2D
    protected OnTriggerEnter2D(data: UnityEngine.Collider2D): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = this.gameObject.AddComponent(puerts.$typeof(UnityEngine.BoxCollider2D)) as UnityEngine.Collider2D
        //使用反射模拟C#调用
        this.invokeComponent(this.OnTriggerEnter2D.name, this.data)
    }
}
class OnTriggerStay2D extends TestBase {
    private data: UnityEngine.Collider2D
    protected OnTriggerStay2D(data: UnityEngine.Collider2D): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = this.gameObject.AddComponent(puerts.$typeof(UnityEngine.BoxCollider2D)) as UnityEngine.Collider2D
        //使用反射模拟C#调用
        this.invokeComponent(this.OnTriggerStay2D.name, this.data)
    }
}
class OnTriggerExit2D extends TestBase {
    private data: UnityEngine.Collider2D
    protected OnTriggerExit2D(data: UnityEngine.Collider2D): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = this.gameObject.AddComponent(puerts.$typeof(UnityEngine.BoxCollider2D)) as UnityEngine.Collider2D
        //使用反射模拟C#调用
        this.invokeComponent(this.OnTriggerExit2D.name, this.data)
    }
}

export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnTriggerEnter2D,
        OnTriggerStay2D,
        OnTriggerExit2D,
    ]
}