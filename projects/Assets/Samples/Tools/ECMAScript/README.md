## 介绍
此功能旨在让puerts esm模块也能使用解构声明语法, 让commonjs和esm模块在typescript层语法一致, 如此我们就能自由转换工程为commonjs模块或esm模块而无需额外修改代码. 同时为csharp namespace创建额外的模块, 从而快速导入其中的类型.

## 注意事项
 **WebGL暂时不支持**

> [2023/03/20]: 使用ESM模块时, puerts需[commit 8dc1af4](https://github.com/Tencent/puerts/commit/8dc1af4e55431dedb3d226139ace69e588e480fa)之后的版本


## 菜单项
| 菜单 | 描述 |
|-----|-----|
|PuerTS/Generate ECMAScript/ESM | 生成ESM模块运行时代码(仅限Binding列表) |
|PuerTS/Generate ECMAScript/ESM(Selector) | [`★推荐`]生成ESM模块运行时代码(手动指定Namespace) |
|PuerTS/Generate ECMAScript/CommonJS | 生成commonjs模块运行时代码(仅限Binding列表) |
|PuerTS/Generate ECMAScript/CommonJS(Selector) | [`★推荐`]生成commonjs模块运行时代码(手动指定Namespace) |

## 使用
- d.ts声明文件位置: Assets/Gen/Typing/csharp/namespaces.d.ts;  
- js运行时文件位置: Assets/Gen/Resources/puerts/modules/**[`.cjs` , `.mjs`];
- js运行时文件manifest: Assets/Gen/Resources/puerts/modules/manifest.txt;

> 在tsconfig.json文件中, 向选项`files`或`include`中添加d.ts声明文件;  
> 而js运行时代码会通过`Puerts.ILoader`实例读取, 需自行处理csharp/puerts模块的读取, 或可参考示例([ECMAScriptLoader](./ECMAScriptLoader.cs)).

```csharp
//串行ILoader调用:
//当filepath处于ECMAScriptLoader.manifest清单中时, 将执行ECMAScriptLoader逻辑, 否则将执行customLoader逻辑
var customLoader = new CustomLoader();
var loader = new Puerts.ECMAScriptLoader(customLoader); 

var env = new Puerts.JsEnv(loader);
```
