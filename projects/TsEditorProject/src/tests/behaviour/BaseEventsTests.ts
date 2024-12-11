import { UnityEngine } from "csharp";
import GameObject = CS.UnityEngine.GameObject;
import { ITest, TestBase } from "./_base";

class OnSelect extends TestBase {
    private data: UnityEngine.EventSystems.BaseEventData
    protected OnSelect(data: UnityEngine.EventSystems.BaseEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnSelect.name, this.data)
    }
}
class OnDeselect extends TestBase {
    private data: UnityEngine.EventSystems.BaseEventData
    protected OnDeselect(data: UnityEngine.EventSystems.BaseEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnDeselect.name, this.data)
    }
}
class OnSubmit extends TestBase {
    private data: UnityEngine.EventSystems.BaseEventData
    protected OnSubmit(data: UnityEngine.EventSystems.BaseEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnSubmit.name, this.data)
    }
}
class OnCancel extends TestBase {
    private data: UnityEngine.EventSystems.BaseEventData
    protected OnCancel(data: UnityEngine.EventSystems.BaseEventData): void {
        this.result = !!data && data === this.data
    }
    public async run() {
        await super.run()

        //@ts-ignore
        this.data = new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current);
        //使用反射模拟C#调用
        this.invokeComponent(this.OnCancel.name, this.data)
    }
}

export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        OnSelect,
        OnDeselect,
        OnSubmit,
        OnCancel,
    ]
}