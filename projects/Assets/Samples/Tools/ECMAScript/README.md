## 介绍
此功能旨在让puerts esm模块也能使用解构声明语法, 让commonjs和esm模块在typescript层语法一致, 如此我们就能自由转换工程为commonjs模块或esm模块而无需额外修改代码. 同时为csharp namespace创建额外的模块, 从而快速导入其中的类型.

## 菜单项
| - | - |
|-----|-----|
|PuerTS/Generate ECMAScript/ESM | 生成ESM模块运行时代码(仅限Binding列表) |
|PuerTS/Generate ECMAScript/ESM(AppDomain) | [`★推荐`]生成ESM模块运行时代码(完整程序集) |
|PuerTS/Generate ECMAScript/CommonJS | 生成commonjs模块运行时代码(仅限Binding列表) |
|PuerTS/Generate ECMAScript/CommonJS(AppDomain) | [`★推荐`]生成commonjs模块运行时代码(完整程序集) |

## 使用
- d.ts声明文件位置: Assets/Gen/Typing/csharp/namespaces.d.ts;  
- js运行时文件位置: Assets/Gen/Resources/puerts/modules/**;

> 在tsconfig.json文件中, 向选项`files`或`include`中添加d.ts声明文件;  
> 而js运行时代码会通过`Puerts.ILoader`实例读取, 需自行处理csharp/puerts模块的读取, 或可参考示例([ModuleLoader](./ModuleLoader.cs)).