import GameObject = CS.UnityEngine.GameObject;
import { ITest, TestBase } from "./_base";

class OnPreCull extends TestBase {
    protected OnPreCull(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnPreCull.name)
    }
}
class OnWillRenderObject extends TestBase {
    protected OnWillRenderObject(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnWillRenderObject.name)
    }
}
class OnBecameVisible extends TestBase {
    protected OnBecameVisible(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnBecameVisible.name)
    }
}
class OnBecameInvisible extends TestBase {
    protected OnBecameInvisible(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnBecameInvisible.name)
    }
}
class OnPreRender extends TestBase {
    protected OnPreRender(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnPreRender.name)
    }
}
class OnRenderObject extends TestBase {
    protected OnRenderObject(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnRenderObject.name)
    }
}
class OnPostRender extends TestBase {
    protected OnPostRender(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnPostRender.name)
    }
}

export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnPreCull,
        OnWillRenderObject,
        OnBecameVisible,
        OnBecameInvisible,
        OnPreRender,
        OnRenderObject,
        OnPostRender,
    ]
}