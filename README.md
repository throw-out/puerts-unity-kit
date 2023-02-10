[![license](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE)
[![](https://img.shields.io/github/issues/throw-out/puerts-unity-kit.svg)](https://github.com/throw-out/puerts-unity-kit/issues)

- **请悉知: 当前项目处于预览期, 其功能并不完善, 其中部分代码可能会在未来版本中进行修改或删除.**
- **如果你有好想法或任何问题, 请提交[[PR]](https://github.com/throw-out/puerts-unity-kit/pulls)和[[ISSUES]](https://github.com/throw-out/puerts-unity-kit/issues).**

## 介绍
> 本项目是基于[puerts](https://github.com/Tencent/puerts)开发的Unity模板项目,  默认使用[OpenUPM(cn)](https://openupm.cn/)作为Unity包管理器源;  
> 本项目使用typescript脚本开发, 支持`commonjs`和`ESM`模块(`ESM`模块通过正则表达式匹配, 无需额外设置);  
> 集成常用配置或工具(可选), 如[ScriptPacker](./docs/ScriptPacker.md)(`脚本打包`/`压缩`/`加密`/`验签`)丶[source-map-support](https://www.npmjs.com/package/source-map-support)丶[javascript-obfuscator](https://www.npmjs.com/package/javascript-obfuscator)等;
>
> 了解更多, 请查看[[文档页面]](./docs).

主要功能:
- TsBehaviour:
  > 在ts脚本中使用Unity [MonoBehaviour生命周期](https://docs.unity3d.com/2021.3/Documentation/Manual/ExecutionOrder.html)方法;  
  > [[查看文档]](./docs/TsBehaviour.md)

- TsProperties:
  > 一个单独的序列化类, 可用于保存数据丶挂载UnityEngine.Object对象等操作;  
  > [[查看文档]](./docs/TsProperties.md)

- TsComponent:
  > 对TsBehaviour和TsProperties的结合实现, 可序列化ts脚本中的成员变量, 并实现ts对象的生命周期管理;  
  > [[查看文档]](./docs/TsComponent.md)

- ThreadWorker:
  > 提供`跨线程`&`跨Puerts.JsEnv实例`交互(基于C#类型传递数据);  
  > [[查看文档]](./docs/ThreadWorker.md)

## 环境
| 软件或包     |  版本           |
| ------------ | ------------ |
| unity   |  2019.2.x + |
| [puerts](https://github.com/Tencent/puerts/releases) |  1.4.0 + |
| [nodejs](https://nodejs.org/) | `unknown`|

## 开始使用
1. 下载本模板项目;
2. 进入目录`projects/TsEditorProject`和`projects/TsProject`安装依赖, 使用`npm install` or `yarn install`命令; 
3. 使用`tsc`命令分别编译以上目录中的typescript脚本;
4. 完成.

## 规划
- webgl/小游戏支持
