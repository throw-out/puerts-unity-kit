
//===========================================================================================
//@ts-nocheck | ignore global error checking

//此功能适用于commonjs模块和esm模块;
//此功能旨在让puerts esm模块也能使用解构声明语法, 让commonjs和esm模块在typescript层语法一致, 如此我们就能自由转换工程为commonjs模块或esm模块而无需额外修改代码. 
//同时为csharp namespace创建额外的模块, 从而快速导入其中的类型.
//例:
//import { Array } from 'csharp.System';
//import { File } from 'csharp.System.IO';
//
//此处仅声明接口, 运行时js代码通过GeneratorECMAScript工具生成, 生成文件位置'Assets/Gen/Resources/puerts/modules';
//js运行时代码会通过`Puerts.ILoader`实例读取, 需自行处理csharp/puerts模块的读取, 详情请查看GeneratorECMAScript工具`README.md`:
//===========================================================================================


declare module "csharp.System" {
    import * as csharp from "csharp";
    export = csharp.System;
}

declare module "csharp.UnityEngine" {
    import * as csharp from "csharp";
    export = csharp.UnityEngine;
}

declare module "csharp.UnityEngine.UI" {
    import * as csharp from "csharp";
    export = csharp.UnityEngine.UI;
}

