import { UnityEngine } from "csharp";
import { ITest, TestBase } from "./_base";
import GameObject = CS.UnityEngine.GameObject;

class OnBeginDrag extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnBeginDrag(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnBeginDrag.name, this.data)
    }
}
class OnDrag extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnDrag(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnDrag.name, this.data)
    }
}
class OnEndDrag extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnEndDrag(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnEndDrag.name, this.data)
    }
}
class OnPointerClick extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnPointerClick(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnPointerClick.name, this.data)
    }
}
class OnPointerDown extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnPointerDown(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnPointerDown.name, this.data)
    }
}
class OnPointerEnter extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnPointerEnter(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnPointerEnter.name, this.data)
    }
}
class OnPointerExit extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnPointerExit(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnPointerExit.name, this.data)
    }
}
class OnPointerUp extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnPointerUp(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnPointerUp.name, this.data)
    }
}
class OnDrop extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnDrop(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnDrop.name, this.data)
    }
}
class OnScroll extends TestBase {
    private data: UnityEngine.EventSystems.PointerEventData
    protected OnScroll(data: UnityEngine.EventSystems.PointerEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnScroll.name, this.data)
    }
}

export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnBeginDrag,
        OnDrag,
        OnEndDrag,
        OnPointerClick,
        OnPointerDown,
        OnPointerEnter,
        OnPointerExit,
        OnPointerUp,
        OnDrop,
        OnScroll,
    ]
}