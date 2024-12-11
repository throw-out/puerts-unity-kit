import { UnityEngine } from "csharp";
import { ITest, TestBase } from "./_base";
import GameObject = CS.UnityEngine.GameObject;

class OnTriggerEnter extends TestBase {
    private data: UnityEngine.Collider
    protected OnTriggerEnter(data: UnityEngine.Collider): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = this.gameObject.AddComponent(puerts.$typeof(UnityEngine.BoxCollider)) as UnityEngine.Collider
        //使用反射模拟C#调用
        this.invokeComponent(this.OnTriggerEnter.name, this.data)
    }
}
class OnTriggerStay extends TestBase {
    private data: UnityEngine.Collider
    protected OnTriggerStay(data: UnityEngine.Collider): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = this.gameObject.AddComponent(puerts.$typeof(UnityEngine.BoxCollider)) as UnityEngine.Collider
        //使用反射模拟C#调用
        this.invokeComponent(this.OnTriggerStay.name, this.data)
    }
}
class OnTriggerExit extends TestBase {
    private data: UnityEngine.Collider
    protected OnTriggerExit(data: UnityEngine.Collider): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        this.data = this.gameObject.AddComponent(puerts.$typeof(UnityEngine.BoxCollider)) as UnityEngine.Collider
        //使用反射模拟C#调用
        this.invokeComponent(this.OnTriggerExit.name, this.data)
    }
}

export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnTriggerEnter,
        OnTriggerStay,
        OnTriggerExit,
    ]
}