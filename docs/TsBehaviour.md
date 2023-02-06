> - xor d.ts声明目录: [projects/Assets/XOR/Typing](../projects/Assets/XOR/Typing)
> - ts示例目录: [projects/TsProject/src/samples](../projects/TsProject/src/samples)

## 使用需知
- 只需要继承[xor.TsBehaviour](../projects/TsEditorProject/src/xor/components/behaviour.ts)就可以使用Unity的[生命周期](https://docs.unity3d.com/2021.3/Documentation/Manual/ExecutionOrder.html)方法 

## 定义
[XOR.TsBehaviour](../projects/Assets/XOR/Runtime/Src/Components/TsBehaviour/TsBehaviour.cs)

## 简单演示

``` typescript
import { GameObject, Time } from "csharp.UnityEngine";

//一个简单的TsBehaviour示例:
class Sample01 extends xor.TsBehaviour {
    private _time: number = 0;

    protected Awake(): void {
        console.log(`TsBehaviour ${Sample01.name}: Awake`);
    }
    //Update将由xor.TsBehaviour统一调用, 你可以使用`standalone`装饰器告知xor.TsBehaviour使用单独的Update调用组件
    //@xor.TsBehaviour.standalone()
    protected Update(deltaTime?: number): void {
        this._time += (deltaTime ?? Time.time);
        if (this._time > 1) {
            this._time -= 1;
            console.log(`TsBehaviour ${Sample01.name}: OnEnable`);
        }
    }
    protected OnEnable(): void {
        console.log(`TsBehaviour ${Sample01.name}: OnEnable`);
    }
    protected OnDisable(): void {
        console.log(`TsBehaviour ${Sample01.name}: OnDisable`);
    }
}
```
