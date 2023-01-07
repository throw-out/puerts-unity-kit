/**
 * 此处仅声明接口, 运行时实现在lib/modules.ts中
 */
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