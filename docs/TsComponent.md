## 介绍
> 实现于类似于[UnityEngine.MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)功能的组件, 可用于挂载并序列化ts脚本成员声明, 将自动构建JSObject对象并实现其生命周期管理([TsComponentLifecycle](../projects/Assets/XOR/Runtime/Src/Components/Lifecycle/TsComponentLifecycle.cs)).
>
> 依赖typescript的AST分析功能, 分析ts脚本获取class声明及其成员信息, 然后传递到C# EditorGUI使用.

## 使用需知
- ts类型必需继承自[xor.TsComponent](../projects/TsEditorProject/src/xor/components/component.ts)丶export且不是abstract才可以被TsComponent使用;
- ts类型成员必需使用declare修饰符或被[xor.field](../projects/TsEditorProject/src/xor/components/component.ts#87)修饰才能被序列;
- 枚举类型如不指定value, 其默认值为0(`System.Int32`)或null(`System.String`);
- AST分析服务和SerializedObject渲染只在Unity Editor环境下使用;
- AST分析服务运行在子线程中([ThreadWorker](./ThreadWorker.md)), 请按照其[已知缺陷](./ThreadWorker.md#已知缺陷)进行设置, 否则可能导致crash;
- AST分析服务运行在子线程中, 指定value时的表达式必需要能在子线程中访问: `例如UnityEngine.Vector2.right是可以的, 而UnityEngine.Application.dataPath不可以`.

## 定义
> [`C#`]继承: [XOR.TsComponent](../projects/Assets/XOR/Runtime/Src/Components/TsComponent.cs) →  [XOR.TsBehaviour](./TsBehaviour.md)  

<details>
<summary>接口详情</summary>

| 成员  | 描述  |
| ------------ | ------------ |
| `Puerts.JSObject JSObject{ get; }`   |  其创建的[Puerts.JSObject](./???)对象 |

| 方法  | 描述  |
| ------------ | ------------ |
| `static void Register(Puerts.JsEnv)`   |  注册TsCompoent使用的Puerts.JsEnv实例 |
| `static void Unregister()`   |  移除已注册的Puerts.JsEnv实例 |
| `static void GC()`   |  回收未正常释放的XOR.TsComponent对象(`例如使用Object.DestroyImmediate时, OnDestroy不会被正常调用`) |
| `static void PrintStatus()`   |  打印所有实例状态(先执行一次GC) |
| `XOR.Serializables.ResultPair[] GetProperties()`   |  获取所有序列化成员 |
| `void SetProperty(string, object)`  | (EditorOnly)设置键值  |
| `void SetPropertyListener(Action<string, object>)` | (EditorOnly)设置键值更新回调 |
</details>

> [`ts`]继承: [xor.TsComponent](../projects/TsEditorProject/src/xor/components/component.ts) →  [xor.TsBehaviour](./TsBehaviour.md)  

<details>
<summary>接口详情</summary>

| 装饰器  | 描述  |
| ------------ | ------------ |
| `@xor.guid(string): ClassDecorator`   |  定义组件guid(⚠⚠⚠此语句应由xor生成和管理, 与class声明绑定, 用户不应该手动创建丶修改) |
| `@xor.route(string): ClassDecorator`   |  定义组件路由(唯一值), 后续可使用此值获取j组件实例(相比较guid更符合人类阅读和记忆的习惯) |
| `@xor.field({...}): PropertyDecorator`   |  定义序列化字段详情, 可设置RawType丶默认值丶Range(仅限number) |

</details>


## 内置类型
<details>
<summary>查看详情</summary>

|  类型   | 基础 | 数组|
| ------- | --- | --- |
| string  | √   | √   |
| number  | √   | √   |
| boolean | √   | √   |
| bigint  | √   | √   |
| UnityEngine.Vector2  | √   | √   |
| UnityEngine.Vector3  | √   | √   |
| UnityEngine.Object及其子类型  | √   | √   |

> 其他类型请参照[自定义类型](./TsComponent.md#自定义扩展类型演示)
</details>

## 基础类型演示
> 示例场景:[projects/Assets/Samples/01_TsComponent](../projects/Assets/Samples/01_TsComponent)  
> 示例typescript代码: [projects/TsProject/src/samples/01_TsComponent.ts](../projects/TsProject/src/samples/01_TsComponent.ts)  

![image](https://user-images.githubusercontent.com/45587825/216535611-dddbc03e-d9d8-4f92-9b75-edb6a435b9f6.png)

## 数组类型演示
![image](https://user-images.githubusercontent.com/45587825/216535825-af29587e-ded5-43ba-bfdb-08d8f7ce67da.png)

## RawType丶默认值丶Range演示
![image](https://user-images.githubusercontent.com/45587825/216536133-24f36803-9318-4786-8ad9-7ec63280a2b4.png)

## 枚举类型演示
 > 枚举类型如不指定value, 其默认值为0(`System.Int32`)或null(`System.String`)

![image](https://user-images.githubusercontent.com/45587825/216808157-d8eaeee8-bcf9-410f-895f-c20ecf04901d.png)

## 自定义扩展类型演示
> 相关代码请查看[示例](../projects/Assets/Samples/01_TsComponent/CustomTypes)中的[TsComponent](../projects/Assets/Samples/01_TsComponent/CustomTypes/Runtime/TsComponent.cs)和[SerializablesEditor](../projects/Assets/Samples/01_TsComponent/CustomTypes/Editor/SerializablesEditor.cs)

![image](https://user-images.githubusercontent.com/45587825/216751394-12e34267-cee4-40ed-9269-8efa5e10320a.png)

## 双向绑定演示
> 在Sample06.OnGUI中修改this._value将会同步到Inspector, 反之在Inspector中修改_value字段亦会同步到Sample06._value上

![image](https://user-images.githubusercontent.com/45587825/216810151-6f5d8d6d-c51e-49b3-a976-92d167202d82.png)

## UGUI事件绑定
 请查看[UGUI页面](./TsComponentBindUGUIEvents.md)
