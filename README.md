**请悉知: 当前项目处于预览版, 其功能并不完善, 其中部分代码可能会在未来版本中进行修改或删除.**

## 介绍
------------
> 本项目是基于[puerts](https://github.com/Tencent/puerts)开发的Unity模板项目,  默认使用[OpenUPM(cn)](https://openupm.cn/)作为Unity包管理器源;
> 本项目使用typescript脚本开发, 使用commonjs作为主要运行时;

主要功能:
- TsBehaviour: 在ts脚本中使用Unity [MonoBehaviour生命周期](https://docs.unity3d.com/2021.3/Documentation/Manual/ExecutionOrder.html)方法
- TsProperties: 一个单独的序列化类, 可用于保存数据丶挂载UnityEngine.Object对象等操作
- [TsComponent](./docs/TsComponent.md): 对TsBehaviour和TsProperties的结合实现, 可序列化ts脚本中的成员变量, 并实现ts对象的生命周期管理
- ThreadWorker: 提供跨线程/跨Puerts.JsEnv实例交互

> 想了解更多, 请查看[文档页面](./docs)

## 环境
------------
| 软件或包     |  版本           |
| ------------ | ------------ |
| unity   |  2019.2.x+ |
| [puerts](https://github.com/Tencent/puerts/releases) |  1.4.0+ |
| [nodejs](https://nodejs.org/) | `unknown`|

## 使用
1. 下载本模板项目;
2. 进入目录`projects/TsEditorProject`和`projects/TsProject`安装依赖, 使用 `npm` or `yarn`; 
3. 分别编译以上目录中的typescript脚本;
4. 完成.

## 规划
------------
- ES模块支持
