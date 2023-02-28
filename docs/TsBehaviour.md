## 介绍
> 只需要继承[xor.TsBehaviour](../projects/TsEditorProject/src/xor/components/behaviour.ts)就可以使用Unity的[生命周期](https://docs.unity3d.com/2021.3/Documentation/Manual/ExecutionOrder.html)方法, 需要在调用构造函数时传参[UnityEngine.GameObject](https://docs.unity3d.com/ScriptReference/GameObject.html)对象;  
> 
> 如果你需要挂载ts脚本并序列化其成员, 请查看[[TsComponent]](./TsComponent.md)模块.

## 定义
> [`C#`]继承: [XOR.TsBehaviour](../projects/Assets/XOR/Runtime/Src/Components/TsBehaviour/TsBehaviour.cs) → [UnityEngine.MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)  

> [`ts`]继承: [xor.TsBehaviour](../projects/TsEditorProject/src/xor/components/behaviour.ts) → 无
<details>
<summary>接口详情</summary>

| 成员  | 描述  |
| ------------ | ------------ |
| `get transform(): Transform` | 获取UnityEngine.Transform组件 |
| `get gameObject(): GameObject` | 获取UnityEngine.GameObject组件 |
| `get enabled(): boolena` |  |
| `set enabled(val: boolena): void` | 设置XOR.TsBehaviour.enable, 影响Update丶FixedUpdate丶LateUpdate等方法 |
| `get isActiveAndEnabled(): boolena` |  |
| `get tag(): string` | 获取GameObject组件标签 |
| `set tag(val: string): void` | 设置GameObject组件标签 |
| `get name(): string` | 获取GameObject组件名称 |
| `set name(val: string): void` | 设置GameObject组件名称 |
| `get rectTransform(): RectTransform` |  |

| 方法  | 描述  |
| ------------ | ------------ |
| `StartCoroutine(Generator \| () => Generator): UnityEngine.Coroutine` | 开启一个协程(尽量使用Promise) |
| `StopCoroutine(UnityEngine.Coroutine): void` | 停止一个协程 |
| `StopAllCoroutines(): void` | 停止所有协程实例 |

| 装饰器  | 描述  |
| ------------ | ------------ |
| `@xor.TsBehaviour.standalone(): PropertyDecorator` | 以独立组件的方式调用(适用于Update丶LateUpdate和FixedUpdate方法, 默认以BatchProxy管理调用以满足更高性能要求) |
| `@xor.TsBehaviour.frameskip(number): PropertyDecorator` | 跨帧调用(全局共用/非单独的frameskip分区), 与standalone组件冲突 |
| `@xor.TsBehaviour.throttle(boolean): PropertyDecorator` | 适用于async/Promise方法, 在上一次调用完成后才会再次调用(Awake丶Update丶FixedUpdate...) |
</details>

## 简单演示
> 示例场景:[projects/Assets/Samples/03_TsBehaviour](../projects/Assets/Samples/03_TsBehaviour)  
> 示例typescript代码: [projects/TsProject/src/samples/03_Behaviour.ts](../projects/TsProject/src/samples/03_TsBehaviour.ts)  
``` typescript
import GameObject = CS.UnityEngine.GameObject;
import Time = CS.UnityEngine.Time;

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
