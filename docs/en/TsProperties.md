*The current page is translated by a machine, information may be missing or inaccurate. If you encounter any problems or have questions, please submit an ISSUE.*

## Introduction
> A standalone serialization class that can be used for saving data and mounting UnityEngine.Object objects.

## Definition
> [`C#`] Inheritance: [XOR.TsProperties](../../projects/Assets/XOR/Runtime/Src/Components/TsProperties.cs) → [UnityEngine.MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)

<details>
<summary>Interface Details</summary>

| Methods  | Description  |
| ------------ | ------------ |
| `XOR.Serializables.ResultPair[] GetProperties()`   |  Get all serialized members |
| `void SetProperty(string, object)`  | (EditorOnly) Set key-value  |
| `void SetPropertyListener(Action<string, object>)` | (EditorOnly) Set key-value update callback |
</details>

## Built-in Types
> Please refer to [TsComponent Built-in Types](./TsComponent.md#built-in-types)

## Simple Demonstration
> Example Scene: [projects/Assets/Samples/02_TsProperties](../../projects/Assets/Samples/02_TsProperties)  
> Example TypeScript Code: [projects/TsProject/src/samples/02_TsProperties.ts](../../projects/TsProject/src/samples/02_TsProperties.ts)  

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

> After running, the following statements will be output:
>
> key0: 12  
> key1: false  
> key2: null  