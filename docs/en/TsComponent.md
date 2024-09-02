*The current page is translated by a machine, information may be missing or inaccurate. If you encounter any problems or have questions, please submit an ISSUE.*

## Introduction
> This is a component designed to function similarly to [UnityEngine.MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html). It can be used to attach and serialize TypeScript script member declarations, automatically building a JSObject object and managing its lifecycle ([TsComponentLifecycle](../../projects/Assets/XOR/Runtime/Src/Components/Lifecycle/TsComponentLifecycle.cs)).
>
> It relies on TypeScript's AST analysis functionality to analyze TypeScript scripts, retrieve class declarations, and their member information, and then pass it to C# EditorGUI for use.

## Usage Guidelines
- TypeScript types must inherit from [xor.TsComponent](../../projects/TsEditorProject/src/xor/components/component.ts), be exported, and not be abstract to be used by TsComponent.
- TypeScript type members must use the declare modifier or be decorated with [xor.field](../../projects/TsEditorProject/src/xor/components/component.ts#115) to be serialized.
- If an enum type does not specify a value, its default value is 0 (`System.Int32`) or null (`System.String`).
- TypeScript analysis service and SerializedObject rendering are only used in the Unity Editor environment.
- TypeScript analysis service runs in a separate thread ([ThreadWorker](./ThreadWorker.md)), please follow its [known issues](./ThreadWorker.md#known-issues) for setup, otherwise it may cause crashes.
- When specifying a value, the expression must be accessible in the separate thread; for example, `UnityEngine.Vector2.right` is acceptable, but `UnityEngine.Application.dataPath` is not.

## Definitions
> [`C#`] Inheritance: [XOR.TsComponent](../../projects/Assets/XOR/Runtime/Src/Components/TsComponent.cs) → [XOR.TsBehaviour](./TsBehaviour.md)

<details>
<summary>Interface Details</summary>

| Member  | Description  |
| ------------ | ------------ |
| `Puerts.JSObject JSObject{ get; }`   |  JSObject created by it  |

| Methods  | Description  |
| ------------ | ------------ |
| `static void Register(Puerts.JsEnv)`   |  Register the Puerts.JsEnv instance used by TsComponent  |
| `static void Unregister()`   |  Remove the registered Puerts.JsEnv instance  |
| `static void GC()`   |  Recycle XOR.TsComponent objects that are not properly released (e.g., when using Object.DestroyImmediate, OnDestroy may not be called properly) |
| `static void PrintStatus()`   |  Print the status of all instances (execute GC first) |
| `XOR.Serializables.ResultPair[] GetProperties()`   |  Get all serialized members |
| `void SetProperty(string, object)`  | (EditorOnly) Set key-value  |
| `void SetPropertyListener(Action<string, object>)` | (EditorOnly) Set key-value update callback |
</details>

> [`ts`] Inheritance: [xor.TsComponent](../../projects/TsEditorProject/src/xor/components/component.ts) → [xor.TsBehaviour](./TsBehaviour.md)

<details>
<summary>Interface Details</summary>

| Decorator  | Description  |
| ------------ | ------------ |
| `@xor.guid(string): ClassDecorator`   |  Define the component GUID (⚠⚠⚠ This statement should be generated and managed by xor, bound to the class declaration, and users should not manually create or modify it) |
| `@xor.route(string): ClassDecorator`   |  Define the component route (unique value), which can be used later to get the component instance (more in line with human reading and memory habits compared to GUID) |
| `@xor.field({...}): PropertyDecorator`   |  Define serialization field details, such as RawType, default value, Range (only for numbers) |

</details>

## Built-in Types
<details>
<summary>Details</summary>

| Type   | Basic | Array|
| ------- | --- | --- |
| string  | √   | √   |
| number  | √   | √   |
| boolean | √   | √   |
| bigint  | √   | √   |
| UnityEngine.Vector2  | √   | √   |
| UnityEngine.Vector3  | √   | √   |
| UnityEngine.Object and its subtypes  | √   | √   |

> For other types, please refer to [Custom Types](./TsComponent.md#custom-extension-type-demo)
</details>

## Basic Type Demo
> Example Scene: [projects/Assets/Samples/01_TsComponent](../../projects/Assets/Samples/01_TsComponent)  
> Example TypeScript Code: [projects/TsProject/src/samples/01_TsComponent.ts](../../projects/TsProject/src/samples/01_TsComponent.ts)  

![image](https://user-images.githubusercontent.com/45587825/216535611-dddbc03e-d9d8-4f92-9b75-edb6a435b9f6.png)

## Array Type Demo
![image](https://user-images.githubusercontent.com/45587825/216535825-af29587e-ded5-43ba-bfdb-08d8f7ce67da.png)

## RawType, Default Value, and Range Demo
![image](https://user-images.githubusercontent.com/45587825/216536133-24f36803-9318-4786-8ad9-7ec63280a2b4.png)

## Enum Type Demo
> If no value is specified for an enum type, its default value is 0 (`System.Int32`) or null (`System.String`)

![image](https://user-images.githubusercontent.com/45587825/216808157-d8eaeee8-bcf9-410f-895f-c20ecf04901d.png)

## Custom Extension Type Demo
> Check the code in [Samples](../../projects/Assets/Samples/01_TsComponent/CustomTypes) for [TsComponent.partial](../../projects/Assets/Samples/01_TsComponent/CustomTypes/Runtime/TsComponent.partial.cs) and [SerializablesEditor](../../projects/Assets/Samples/01_TsComponent/CustomTypes/Editor/SerializablesEditor.cs)

![image](https://user-images.githubusercontent.com/45587825/216751394-12e34267-cee4-40ed-9269-8efa5e10320a.png)

## Two-Way Binding Demo
> Modifying this._value in Sample06.OnGUI will be synchronized to the Inspector, and vice versa, modifying the _value field in the Inspector will be synchronized to Sample06._value.

![image](https://user-images.githubusercontent.com/45587825/216810151-6f5d8d6d-c51e-49b3-a976-92d167202d82.png)

## TypeScript Reference Type Binding
> // _sample01, _sample03, and _sample04 here are Proxy objects and cannot be directly compared using '==='.
> // Call xxx.valueOf() to get the original JS object.
> ```typescript
> let isSample01 = this._sample04?.valueOf() === this._sample01?.valueOf();
> let isSample03 = this._sample04?.valueOf() === this._sample03?.valueOf();
> console.log(`this._sample04: is sample01 = ${isSample01},  is sample03 = ${isSample03}`);
> ```
![image](https://github.com/throw-out/puerts-unity-kit/assets/45587825/2f6366cf-d506-4c4b-a00f-205adbc5690f)

## Runtime Methods (AddComponent/GetComponent)
![image](https://github.com/throw-out/puerts-unity-kit/assets/45587825/0fa391aa-642b-4cb0-a8a1-a313a2075ab6)
Output prints as follows:
![image](https://github.com/throw-out/puerts-unity-kit/assets/45587825/8dfb795b-1d60-47fe-93c2-773552eecd2c)

## UGUI Event Binding
Please refer to [UGUI Page](./TsComponentBindUGUIEvents.md)
