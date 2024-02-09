## 介绍
> 一个单独的序列化类, 可用于保存数据丶挂载UnityEngine.Object对象等操作;

## 定义
> [`C#`]继承: [XOR.TsProperties](../../projects/Assets/XOR/Runtime/Src/Components/TsProperties.cs) → [UnityEngine.MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)  

<details>
<summary>接口详情</summary>

| 方法  | 描述  |
| ------------ | ------------ |
| `XOR.Serializables.ResultPair[] GetProperties()`   |  获取所有序列化成员 |
| `void SetProperty(string, object)`  | (EditorOnly)设置键值  |
| `void SetPropertyListener(Action<string, object>)` | (EditorOnly)设置键值更新回调 |
</details>

## 内置类型
> 请参照[TsComponent内置类型](./TsComponent.md#内置类型)

## 简单演示
> 示例场景:[projects/Assets/Samples/02_TsProperties](../../projects/Assets/Samples/02_TsProperties)  
> 示例typescript代码: [projects/TsProject/src/samples/02_TsProperties.ts](../../projects/TsProject/src/samples/02_TsProperties.ts)  

![image](https://user-images.githubusercontent.com/45587825/217222792-42495cf8-cec1-4ad2-92ea-6908d83f43af.png)
```typescript
export function init(target: CS.XOR.TsProperties) {
    let properties = target?.GetProperties();
    if (properties) {
        let stringBuilder: string[] = [];
        for (let i = 0; i < properties.Length; i++) {
            let property = properties.get_Item(i);
            stringBuilder.push(`${property.key}: ${property.value}`);
        }
        console.log(stringBuilder.join("\n"));
    } else {
        console.log("empty");
    }
}
```
> 运行后将输出以下语句:  
> key0: 12  
> key1: false  
> key2: null  