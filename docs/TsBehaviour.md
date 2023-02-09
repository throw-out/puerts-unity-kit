## 介绍
> 只需要继承[xor.TsBehaviour](../projects/TsEditorProject/src/xor/components/behaviour.ts)就可以使用Unity的[生命周期](https://docs.unity3d.com/2021.3/Documentation/Manual/ExecutionOrder.html)方法, 需要在调用构造函数时传参[UnityEngine.GameObject](https://docs.unity3d.com/ScriptReference/GameObject.html)对象;

## 定义
> 继承: [XOR.TsBehaviour](../projects/Assets/XOR/Runtime/Src/Components/TsBehaviour/TsBehaviour.cs) → [UnityEngine.MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)

## 成员
<details>
<summary>查看详情</summary>

| 名称  | 描述  |
| ------------ | ------------ |
</details>

## 方法
<details>
<summary>查看详情</summary>

| 名称  | 描述  |
| ------------ | ------------ |
</details>

## 简单演示
> - 示例场景:[projects/Assets/Samples/03_TsBehaviour](../projects/Assets/Samples/03_TsBehaviour)
> - 示例typescript代码: [projects/TsProject/src/samples/03_Behaviour.ts](../projects/TsProject/src/samples/03_TsBehaviour.ts)
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
            console.log(`TsBehaviour ${Sample01.name}: Update`);
        }
    }
    protected OnEnable(): void {
        console.log(`TsBehaviour ${Sample01.name}: OnEnable`);
    }
    protected OnDisable(): void {
        console.log(`TsBehaviour ${Sample01.name}: OnDisable`);
    }
}

export function init() {
    let gameObject = new GameObject(Sample01.name);
    return new Sample01(gameObject);
}
```
