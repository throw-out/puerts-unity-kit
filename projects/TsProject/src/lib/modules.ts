/**
 * modules定义实现, 在程序启动时调用
 */
let namespaces = new Map<string, any>();
namespaces.set("csharp.System", CS.System);
namespaces.set("csharp.UnityEngine", CS.UnityEngine);
namespaces.set("csharp.UnityEngine.UI", CS.UnityEngine.UI);

(function () {
    namespaces.forEach((module, name) => {
        module.default = module;
        puer["registerBuildinModule"](name, module);
    });
})();
export { };