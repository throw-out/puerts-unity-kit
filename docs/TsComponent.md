使用typescript的AST解析器, 分析ts脚本获取class声明及其成员信息, 然后传递到C# SerializedObject渲染使用.
- ts类型必需继承自[xor.TsComponent](../projects/TsEditorProject/src/xor/components/component.ts#L59)丶export且不是abstract才会被序列化:
- ts类型成员必需使用declare修饰符或被[xor.field](../projects/TsEditorProject/src/xor/components/component.ts#87)修饰才能被序列化

## 使用需知
- AST分析和SerializedObject渲染只在UnityEditor环境下使用
- 指定value时如超出range设定时, 只会在SerializedObject渲染时才会重新设定为range区间
- 枚举类型如不指定value, 其默认值为0(`System.Int32`)或null(`System.String`)
- AST解析器运行在子线程中, 指定value时的表达式必需要能在子线程中访问: `例如UnityEngine.Vector2.right是可以的, 而UnityEngine.Application.dataPath是不可以`

## 内置类型
|  类型   | 基础 | 数组|
| ------- | --- | --- |
| string  | √   | √   |
| number  | √   | √   |
| boolean | √   | √   |
| bigint  | √   | √   |
| UnityEngine.Vector2  | √   | √   |
| UnityEngine.Vector3  | √   | √   |
| UnityEngine.Object及其子类型  | √   | √   |

> 其他类型请参照[自定义类型](../projects/Assets/Samples/01_TsComponent/CustomTypes)

## 基础类型演示
![image](https://user-images.githubusercontent.com/45587825/216535611-dddbc03e-d9d8-4f92-9b75-edb6a435b9f6.png)
## 数组类型演示
![image](https://user-images.githubusercontent.com/45587825/216535825-af29587e-ded5-43ba-bfdb-08d8f7ce67da.png)
## RawType丶默认值丶Range演示
![image](https://user-images.githubusercontent.com/45587825/216536133-24f36803-9318-4786-8ad9-7ec63280a2b4.png)
## 枚举类型演示
 > 枚举类型如不指定value, 其默认值为0(`System.Int32`)或null(`System.String`)

![image](https://user-images.githubusercontent.com/45587825/216536402-4d09df95-cca5-43b5-97f2-be93965d1b04.png)

## 自定义扩展类型演示
> 查看[示例](../projects/Assets/Samples/01_TsComponent/CustomTypes)中的[TsComponent](../projects/Assets/Samples/01_TsComponent/CustomTypes/Runtime/TsComponent.cs)和[SerializablesEditor](../projects/Assets/Samples/01_TsComponent/CustomTypes/Editor/SerializablesEditor.cs)

![image](https://user-images.githubusercontent.com/45587825/216751394-12e34267-cee4-40ed-9269-8efa5e10320a.png)
