[![license](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE)
[![](https://img.shields.io/github/issues/throw-out/puerts-unity-kit.svg)](https://github.com/throw-out/puerts-unity-kit/issues)

**[[中文简体]](./README.md) | [English]**

*The current page is translated by a machine, information may be missing or inaccurate. If you encounter any problems or have questions, please submit an ISSUE.*

- **If you are interested in this project, please click `star` to support.**
- **If you have any ideas or questions, please submit feedback through [[PR]](https://github.com/throw-out/puerts-unity-kit/pulls) and [[ISSUES]](https://github.com/throw-out/puerts-unity-kit/issues).**
- **Note: The `TsComponent` and `ThreadWorker` modules in the current project have not been widely verified, and their functionality may have defects. If you encounter any issues during use, please provide feedback through [[ISSUES]](https://github.com/throw-out/puerts-unity-kit/issues).**

## Introduction
> - This project is a Unity template project developed based on [puerts](https://github.com/Tencent/puerts), using [OpenUPM](https://openupm.com/) as the default Unity package manager source.
> - This project is developed using TypeScript scripts, with runtime support for both `commonjs` and `ESM` modules (check [MixerLoader.Mode](./projects/Assets/XOR/Runtime/Src/Loader.cs) for matching rules).
> - This project supports webgl build (`ESM`). If using webgl build, be sure to first read the article ["How to Migrate from the Original PuerTS Project?"](https://github.com/zombieyang/puerts_unity_webgl_demo/wiki/%E5%A6%82%E4%BD%95%E4%BB%8E%E5%8E%9F%E6%9C%89%E7%9A%84PuerTS%E9%A1%B9%E7%9B%AE%E4%B8%AD%E8%BF%81%E7%A7%BB%E8%BF%87%E6%9D%A5%EF%BC%9F) in the [source repository](https://github.com/zombieyang/puerts_unity_webgl_demo).
> - Integrates common configurations or tools (optional), such as [ScriptPacker](./docs/en/ScriptPacker.md) (`script packaging`/`compression`/`encryption`/`verification`) and [source-map-support](https://www.npmjs.com/package/source-map-support), [javascript-obfuscator](https://www.npmjs.com/package/javascript-obfuscator), etc.
>
> For more information, please refer to the [[Documentation]](./docs/en).

> `[2023/02/13]` Please note that the latest version `1.0.0-rc.1` of webgl-support in OpenUMP does not support automatic appending of suffixes, waiting for subsequent updates.

Main features:
- TsBehaviour:
  > Use Unity [MonoBehaviour lifecycle methods](https://docs.unity3d.com/2021.3/Documentation/Manual/ExecutionOrder.html) in TypeScript scripts.
  >
  > [[Documentation]](./docs/en/TsBehaviour.md)

- TsProperties:
  > A standalone serialization class used for saving data and attaching UnityEngine.Object objects.
  >
  > [[Documentation]](./docs/en/TsProperties.md)

- TsComponent:
  > Implementation that combines TsBehaviour and TsProperties, serializing member variables in ts scripts and using Unity lifecycle methods. It manages the lifecycle of ts objects.
  > Allows UGUI events to be bound to ts scripts ([UGUI events](./docs/en/TsComponentBindUGUIEvents.md)).
  >
  > [[Documentation]](./docs/en/TsComponent.md)

- ThreadWorker:
  > Provides `cross-thread` & `cross-Puerts.JsEnv instance` interaction (data transmission based on C# types).
  >
  > [[Documentation]](./docs/en/ThreadWorker.md)

## Environment
| Software or Package | Version |
| ------------ | ------------ |
| Unity   |  2019.2.x + |
| [puerts](https://github.com/Tencent/puerts/releases) |  1.4.0 + |
| [nodejs](https://nodejs.org/) | `unknown`|

| OpenUPM     |  Version           |
| ------------ | ------------ |
| [puerts](https://openupm.com/packages/com.tencent.puerts.core/) | 2.0.2 |
| [puerts-webgl-support](https://openupm.com/packages/com.tencent.puerts.webgl/) | 2.0.2 |
| [puerts-commonjs-support](https://openupm.com/packages/com.tencent.puerts.commonjs/) |  2.0.1 |

## Getting Started
1. Download this template project.
2. Go to the `projects/TsProject` directory and execute the `npm install` or `yarn` command to install dependencies.
3. Execute the `tsc` command to compile the TypeScript project.
4. Done.

## Tools
- [ConsoleRedirect:](./projects/Assets/Samples/Editor/ConsoleRedirect)
  > Implement direct jumping to ts scripts by clicking/double-clicking hyperlinks in the Unity Console.
- [HotReload:](./projects/Assets/Samples/Editor/HotReload)
  > C#-only js script hot update tool (requires `v8 + inspect` support), used for runtime modification of js logic for quick debugging.
- [MiniLinkXml:](./projects/Assets/Samples/Editor/MiniLinkXml)
  > Analyze the typescript project, obtain all used C# types (support additional custom types), and generate a minimal `link.xml` configuration file.
- [ECMAScript:](./projects/Assets/Samples/Tools/ECMAScript)
  > C# namespace generation tool, can `quick import` values and types (e.g., `import { GameObject} from 'csharp.UnityEngine'`). Make the project compatible with both commonjs and esm modules without additional code modifications.


## Roadmap
- None
