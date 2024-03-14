[![license](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE)
[![](https://img.shields.io/github/issues/throw-out/puerts-unity-kit.svg)](https://github.com/throw-out/puerts-unity-kit/issues)

**[中文简体] | [[English]](./README_EN.md)**

- **如果你对本项目感兴趣, 请点击`star`支持.**  
- **如果你有任何想法或疑问, 请提交[[PR]](https://github.com/throw-out/puerts-unity-kit/pulls)和[[ISSUES]](https://github.com/throw-out/puerts-unity-kit/issues)反馈.**  
- **请悉知: 当前项目中的`TsComponent`丶`ThreadWorker`模块尚未经广泛验证, 其功能或许存在缺陷, 如果你在使用中遇到任何问题请及时通过[[ISSUES]](https://github.com/throw-out/puerts-unity-kit/issues)反馈.**

## 介绍
> - 本项目是基于[puerts](https://github.com/Tencent/puerts)开发的Unity模板项目,  默认使用[OpenUPM(cn)](https://openupm.cn/)作为Unity包管理器源;  
> - 本项目使用typescript脚本开发, 运行时支持`commonjs`和`ESM`模块(匹配规则请查看[MixerLoader.Mode](./projects/Assets/XOR/Runtime/Src/Loader.cs));  
> - 本项目支持webgl构建(`ESM`), 使用webgl构建请务必先翻阅[源仓库](https://github.com/zombieyang/puerts_unity_webgl_demo)中[《如何从原有的PuerTS项目中迁移过来？》](https://github.com/zombieyang/puerts_unity_webgl_demo/wiki/%E5%A6%82%E4%BD%95%E4%BB%8E%E5%8E%9F%E6%9C%89%E7%9A%84PuerTS%E9%A1%B9%E7%9B%AE%E4%B8%AD%E8%BF%81%E7%A7%BB%E8%BF%87%E6%9D%A5%EF%BC%9F)一文;  
> - 集成常用配置或工具(可选), 如[ScriptPacker](./docs/zhcn/ScriptPacker.md)(`脚本打包`/`压缩`/`加密`/`验签`)丶[source-map-support](https://www.npmjs.com/package/source-map-support)丶[javascript-obfuscator](https://www.npmjs.com/package/javascript-obfuscator)等;
>
> 了解更多, 请查看[[文档页面]](./docs/zhcn).

> `[2023/02/13]`需注意webgl-support在OpenUMP中的最新版本`1.0.0-rc.1`不支持自动附加后缀名匹配, 需等待后续更新

主要功能:
- TsBehaviour:
  > 在ts脚本中使用Unity [MonoBehaviour生命周期](https://docs.unity3d.com/2021.3/Documentation/Manual/ExecutionOrder.html)方法;  
  > [[查看文档]](./docs/zhcn/TsBehaviour.md)

- TsProperties:
  > 一个单独的序列化类, 可用于保存数据丶挂载UnityEngine.Object对象等操作;  
  > [[查看文档]](./docs/zhcn/TsProperties.md)

- TsComponent:
  > 对TsBehaviour和TsProperties的结合实现, 可序列化ts脚本中的成员变量和使用Unity生命周期方法, 并实现对ts对象的生命周期管理;  
  > 允许UGUI事件绑定到ts脚本上([UGUI事件](./docs/zhcn/TsComponentBindUGUIEvents.md));  
  > [[查看文档]](./docs/zhcn/TsComponent.md)

- ThreadWorker:
  > 提供`跨线程`&`跨Puerts.JsEnv实例`交互(基于C#类型传递数据);  
  > [[查看文档]](./docs/zhcn/ThreadWorker.md)

## 环境
| 软件或包     |  版本           |
| ------------ | ------------ |
| unity   |  2019.2.x + |
| [puerts](https://github.com/Tencent/puerts/releases) |  1.4.0 + |
| [nodejs](https://nodejs.org/) | `unknown`|

| OpenUPM     |  版本           |
| ------------ | ------------ |
| [puerts](https://openupm.com/packages/com.tencent.puerts.core/) | 2.0.2 |
| [puerts-webgl-support](https://openupm.com/packages/com.tencent.puerts.webgl/) | 2.0.2 |
| [puerts-commonjs-support](https://openupm.com/packages/com.tencent.puerts.commonjs/) |  2.0.1 |

## 开始使用
1. 下载本模板项目;
2. 进入目录`projects/TsProject`, 执行`npm install` 或 `yarn`命令安装依赖;
3. 执行`tsc`命令编译typescript项目;
4. 完成.

## 附加工具
- [ConsoleRedirect:](./projects/Assets/Samples/Editor/ConsoleRedirect)
  > 实现从Unity Console单击/双击超链接直接跳转ts脚本
- [HotReload:](./projects/Assets/Samples/Editor/HotReload)
  > 纯C#实现的js脚本热更新工具(需要`v8 + inspect`支持), 用于运行时即时修改js逻辑快速调试.
- [MiniLinkXml:](./projects/Assets/Samples/Editor/MiniLinkXml)
  > 分析typescript工程, 获取所有使用的C#类型的(支持额外的自定义类型), 生成最小化`link.xml`配置文件.
- [ECMAScript:](./projects/Assets/Samples/Tools/ECMAScript)
  > C#命名空间生成工具, 可`快速导入`值和类型(例: `import { GameObject} from 'csharp.UnityEngine'`). 使工程同时兼容commonjs和esm模块从而无需额外修改代码.

## 规划
- 无
