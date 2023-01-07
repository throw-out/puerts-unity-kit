/**
 * modules定义实现, 在程序启动时调用
 */
import * as csharp from "csharp";

let namespaces = new Map<string, any>();
namespaces.set("csharp.System", csharp.System);
namespaces.set("csharp.UnityEngine", csharp.UnityEngine);
namespaces.set("csharp.UnityEngine.UI", csharp.UnityEngine.UI);

(function () {
    let puerts = (this ?? globalThis)["puerts"];
    namespaces.forEach((module, name) => {
        module.default = module;
        puerts.registerBuildinModule(name, module);
    });
})();