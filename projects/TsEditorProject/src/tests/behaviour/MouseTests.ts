import GameObject = CS.UnityEngine.GameObject;
import { ITest, TestBase } from "./_base";

class OnMouseDown extends TestBase {
    protected OnMouseDown(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnMouseDown.name)
    }
}
class OnMouseDrag extends TestBase {
    protected OnMouseDrag(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnMouseDrag.name)
    }
}
class OnMouseEnter extends TestBase {
    protected OnMouseEnter(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnMouseEnter.name)
    }
}
class OnMouseExit extends TestBase {
    protected OnMouseExit(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnMouseExit.name)
    }
}
class OnMouseOver extends TestBase {
    protected OnMouseOver(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnMouseOver.name)
    }
}
class OnMouseUpAsButton extends TestBase {
    protected OnMouseUpAsButton(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnMouseUpAsButton.name)
    }
}
class OnMouseUp extends TestBase {
    protected OnMouseUp(): void {
        this.result = true
    }
    public async run() {
        await super.run()

        //使用反射模拟C#调用
        this.invokeComponent(this.OnMouseUp.name)
    }
}


export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnMouseDown,
        OnMouseDrag,
        OnMouseEnter,
        OnMouseExit,
        OnMouseOver,
        OnMouseUpAsButton,
        OnMouseUp,
    ]
}