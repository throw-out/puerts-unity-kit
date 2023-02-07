> - xor d.ts声明目录: [projects/Assets/XOR/Typing](../projects/Assets/XOR/Typing)
> - ts示例目录: [projects/TsProject/src/samples](../projects/TsProject/src/samples)

## 定义
[XOR.TsProperties](../projects/Assets/XOR/Runtime/Src/Components/TsProperties.cs)

## 方法
| 名称  | 描述  |
| ------------ | ------------ |
| GetProperties   |  获取所有序列化成员 |
| SetProperty  | (EditorOnly)设置键值  |
| SetPropertyListener | (EditorOnly)设置键值更新回调 |


## 简单演示
> - 示例场景:[projects/Assets/Samples/02_TsProperties](../projects/Assets/Samples/02_TsProperties)  
> - 示例typescript代码: [projects/TsProject/src/samples/02_TsProperties.ts](../projects/TsProject/src/samples/02_TsProperties.ts)

![image](https://user-images.githubusercontent.com/45587825/217222792-42495cf8-cec1-4ad2-92ea-6908d83f43af.png)
```typescript
import * as csharp from "csharp";

export function init(target: csharp.XOR.TsProperties) {
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