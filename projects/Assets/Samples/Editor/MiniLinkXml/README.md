## 介绍
> 分析typescript项目, 查找所有使用的C#类型, 然后生成link.xml配置文件来防止代码裁剪.

- 支持分析扩展方法调用
- 支持自定义C#类型
- 支持自定义link.xml内容(支持程序集 preserve="all")


## 菜单项
| 菜单 | 描述 |
|-----|-----|
|Tools/MiniLinkXml/Generate link.xml | 生成link.xml配置文件 |
|Tools/MiniLinkXml/Generate link.cs | 生成link.cs脚本引用文件 |
|Tools/MiniLinkXml/Generate link.xml +  link.cs | 同时生成两者配置 |
