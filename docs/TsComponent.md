使用typescript的AST解析器, 分析ts脚本获取class声明及其成员信息, 然后传递到C# SerializedObject渲染使用.
> AST分析和SerializedObject渲染(TsComponentEditor)只在UnityEditor环境下使用

## 基础类型演示
![image](https://user-images.githubusercontent.com/45587825/216535611-dddbc03e-d9d8-4f92-9b75-edb6a435b9f6.png)
## 数组类型演示
![image](https://user-images.githubusercontent.com/45587825/216535825-af29587e-ded5-43ba-bfdb-08d8f7ce67da.png)
## RawType丶默认值丶Range演示
![image](https://user-images.githubusercontent.com/45587825/216536133-24f36803-9318-4786-8ad9-7ec63280a2b4.png)
## 枚举类型演示
 > 如果不定义value, 序列化类型为int时默认值为0, 序列化类型为string时默认值为null

![image](https://user-images.githubusercontent.com/45587825/216536402-4d09df95-cca5-43b5-97f2-be93965d1b04.png)

## 自定义扩展类型演示
TODO
