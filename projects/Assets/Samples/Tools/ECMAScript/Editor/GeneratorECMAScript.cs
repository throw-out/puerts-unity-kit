using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Puerts.Editor
{
    public static class GeneratorECMAScript
    {
        [MenuItem("Tools/PuerTS/Generate ECMAScript/ESM", false, 1)]
        public static void GenerateESM()
        {
            GenerateCode(true, null);
        }
        [MenuItem("Tools/PuerTS/Generate ECMAScript/ESM(Selector)", false, 1)]
        public static void GenerateESMAppDomain()
        {
            ModuleSelector.Get().Once((targets) =>
            {
                GenerateCode(true, targets);
            });
        }
        [MenuItem("Tools/PuerTS/Generate ECMAScript/CommonJS", false, 1)]
        public static void GenerateCommonjs()
        {
            GenerateCode(false, null);
        }
        [MenuItem("Tools/PuerTS/Generate ECMAScript/CommonJS(Selector)", false, 1)]
        public static void GenerateCommonjsAppDomain()
        {
            ModuleSelector.Get().Once((targets) =>
            {
                GenerateCode(false, targets);
            });
        }
        [MenuItem("Tools/PuerTS/Generate ECMAScript/Clear", false, 1)]
        public static void Clear()
        {
            string dirpath = GetRootPath(Configure.GetCodeOutputDirectory(), true);
            if (!Directory.Exists(dirpath))
                return;
            Directory.Delete(dirpath, true);
            AssetDatabase.Refresh();
        }

        static void GenerateCode(bool isESM, IEnumerable<string> targets)
        {
            Clear();

            var set = targets != null ? new HashSet<string>(targets) : null;

            var start = DateTime.Now;
            var saveTo = Configure.GetCodeOutputDirectory();
            GenerateCode(saveTo, isESM, set);
            GenerateDTS(saveTo, set);
            GenerateManifest(saveTo, isESM, set);
            Debug.Log("finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms");
            AssetDatabase.Refresh();
        }
        static void GenerateCode(string saveTo, bool isESM, HashSet<string> targets)
        {
            var configure = Configure.GetConfigureByTags(new List<string>() {
                    "Puerts.BindingAttribute",
                });
            var genTypes = configure["Puerts.BindingAttribute"].Select(kv => kv.Key)
                .Where(o => o is Type)
                .Cast<Type>()
                .Where(t => !t.IsGenericTypeDefinition);

            var genInfos = Puerts.Editor.Generator.DTS.TypingGenInfo.FromTypes(genTypes).NamespaceInfos
                .ToDictionary(
                    info => info.Name == null ? string.Empty : info.Name,
                    info => NamespaceGenInfo.From(info.Name, info.Types.Select(t => TypeGenInfo.From(t.Namespace, t.Name)).ToArray())
                );

            var namespaceInfos = genInfos.Values.ToList();
            while (namespaceInfos.Count > 0)
            {
                var info = namespaceInfos[0];
                namespaceInfos.RemoveAt(0);
                if (info.Name == info.FirstName || string.IsNullOrEmpty(info.Name))
                    continue;

                NamespaceGenInfo parent;
                if (!genInfos.TryGetValue(info.FirstName, out parent))
                {
                    parent = NamespaceGenInfo.From(info.FirstName, new TypeGenInfo[0]);
                    genInfos.Add(info.FirstName, parent);
                    namespaceInfos.Add(parent);
                }
                parent.AddChildNamespace(info.Name);
            }

            Directory.CreateDirectory(GetRootPath(saveTo, false));
            foreach (var info in genInfos)
            {
                if (!string.IsNullOrEmpty(info.Key) && targets != null && !targets.Contains(info.Key))
                    continue;

                string fileName = string.IsNullOrEmpty(info.Key) ? "csharp" : $"csharp.{info.Key}";
                string filePath = GetFilePath(saveTo, fileName, isESM);
                using (StreamWriter textWriter = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    textWriter.Write(isESM ? GenerateTemplateESMCode(info.Key, info.Value) : GenerateTemplateCommonjsCode(info.Key, info.Value));
                    textWriter.Flush();
                }
            }

            string filepath = GetFilePath(saveTo, "puerts", isESM);
            using (StreamWriter textWriter = new StreamWriter(filepath, false, Encoding.UTF8))
            {
                textWriter.Write(isESM ? GenerateTemplatePuertsESMCode() : GenerateTemplatePuertsCommonjsCode());
                textWriter.Flush();
            }
        }
        static void GenerateDTS(string saveTo, HashSet<string> targets)
        {
            var configure = Configure.GetConfigureByTags(new List<string>() {
                    "Puerts.BindingAttribute",
                });
            var genTypes = configure["Puerts.BindingAttribute"].Select(kv => kv.Key)
                .Where(o => o is Type)
                .Cast<Type>()
                .Where(t => !t.IsGenericTypeDefinition);

            var namespaces = Puerts.Editor.Generator.DTS.TypingGenInfo.FromTypes(genTypes).NamespaceInfos
                .Select(info => info.Name)
                .Distinct()
                .Where(name => !string.IsNullOrEmpty(name));
            if (targets != null)
            {
                namespaces = namespaces.Where(name => targets.Contains(name));
            }

            string filepath = GetDTSPath(saveTo);
            if (namespaces.Count() == 0)
            {
                if (File.Exists(filepath)) File.Delete(filepath);
                return;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            using (StreamWriter textWriter = new StreamWriter(filepath, false, Encoding.UTF8))
            {
                textWriter.Write(GenerateTemplateDTS(namespaces));
                textWriter.Flush();
            }
        }
        static void GenerateManifest(string saveTo, bool isESM, HashSet<string> targets)
        {
            var configure = Configure.GetConfigureByTags(new List<string>() {
                    "Puerts.BindingAttribute",
                });
            var genTypes = configure["Puerts.BindingAttribute"].Select(kv => kv.Key)
                .Where(o => o is Type)
                .Cast<Type>()
                .Where(t => !t.IsGenericTypeDefinition);

            var namespaces = Puerts.Editor.Generator.DTS.TypingGenInfo.FromTypes(genTypes).NamespaceInfos
                .Select(info => info.Name)
                .Distinct()
                .Where(name => !string.IsNullOrEmpty(name));
            if (targets != null)
            {
                namespaces = namespaces.Where(name => targets.Contains(name));
            }

            List<string> manifestList = new List<string>(namespaces.Select(name => $"csharp.{name}"));
            manifestList.Add("csharp");
            manifestList.Add("puerts");

            string filepath = GetManifestPath(saveTo);
            if (manifestList.Count == 0)
            {
                if (File.Exists(filepath)) File.Delete(filepath);
                return;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            using (StreamWriter textWriter = new StreamWriter(filepath, false, Encoding.UTF8))
            {
                textWriter.Write($"isESM:{isESM}\n{string.Join("\n", manifestList)}");
                textWriter.Flush();
            }
        }

        static readonly string ESMProxyFunctionCode = @"
function __proxy__(getter) {
    let target;
    function tryload() {
        if (!getter) return;
        target = getter();
        getter = null;
    };
    return new Proxy(tryload, {
        apply: function (_, thisArg, argArray) {
            tryload();
            target.apply(thisArg, argArray);
        },
        construct: function (_, argArray, newTarget) {
            tryload();
            return new target(...argArray);
        },
        get: function (_, property) {
            tryload();
            return target[property];
        },
        set: function (_, property, newValue) {
            tryload();
            target[property] = newValue;
            return true;
        },
        defineProperty: function (_, property, attributes) {
            tryload();
            Object.defineProperty(target, property, attributes);
            return true;
        },
        deleteProperty: function (_, property) {
            tryload();
            delete target[property];
            return true;
        },
        getOwnPropertyDescriptor: function (_, property) {
            tryload();
            return Object.getOwnPropertyDescriptor(target, property);
        },
        getPrototypeOf: function (_) {
            tryload();
            return Object.getPrototypeOf(target);
        },
        setPrototypeOf: function (_, newValue) {
            tryload();
            Object.setPrototypeOf(target, newValue);
            return true;
        },
        has: function (_, property) {
            tryload();
            return property in target;
        },
        isExtensible: function (_) {
            tryload();
            return Object.isExtensible(target);
        },
        ownKeys: function (_) {
            tryload();
            return Reflect.ownKeys(target)?.filter(key => Object.getOwnPropertyDescriptor(target, key)?.configurable);
        },
        preventExtensions: function (_) {
            tryload();
            Object.preventExtensions(target);
            return true;
        },

    });
}
";
        static string GenerateTemplateESMCode(string firstName, NamespaceGenInfo genInfo)
        {
            firstName = string.IsNullOrEmpty(firstName) ? "csharp" : $"csharp.{firstName}";
            var typeNames = genInfo.Members
                .Select(typeInfo => typeInfo.Name)
                .Distinct();
            var namespaceNames = genInfo.Namespaces
                .Where(name => !typeNames.Contains(name))
                .Distinct();
            return $@"
const csharp = (function () {{
    let _g = global || globalThis || this;
    return _g['CS'] || _g['csharp'] || require('csharp');
}})();

{ESMProxyFunctionCode}

export default {firstName};

//导出名称为Object的类可能与全局域中的Object冲突, 此处生成别名在末尾再一次性导出
{string.Join("", typeNames.Select(name => $@"
const ${name} = __proxy__(() => {firstName}.{name});"))}
{string.Join("", namespaceNames.Select(name => $@"
const ${name} = __proxy__(() => {firstName}.{name});"))}

export {{
{string.Join("", typeNames.Select(name => $@"
    ${name} as {name},"))}
{string.Join("", namespaceNames.Select(name => $@"
    ${name} as {name},"))}
}}

";
        }
        static string GenerateTemplateCommonjsCode(string firstName, NamespaceGenInfo genInfo)
        {
            firstName = string.IsNullOrEmpty(firstName) ? "csharp" : $"csharp.{firstName}";

            return $@"
const csharp = (function () {{
    let _g = global || globalThis || this;
    return _g['CS'] || _g['csharp'] || require('csharp');
}})();

module.exports = {firstName};
module.exports.default = {firstName};
";
        }
        static string GenerateTemplatePuertsESMCode()
        {
            string[] members;
            using (var env = new JsEnv())
            {
                members = env.Eval<string[]>(@"
(function () {
    const csharp = (function () {
        let _g = global || globalThis || this;
        return _g['CS'] || _g['csharp'] || require('csharp');
    })();
    const puerts = (function () {
        let _g = global || globalThis || this;
        return _g['puerts'] || require('puerts');
    })();

    const keys = Object.keys(puerts);
    const members = csharp.System.Array.CreateInstance(puerts.$typeof(csharp.System.String), keys.length);
    for (let i = 0; i < keys.length; i++) {
        members.set_Item(i, keys[i]);
    }
    return members;
})();");
            }

            return $@"
const puerts = (function () {{
    let _g = global || globalThis || this;
    return _g['puerts'] || require('puerts');
}})();

export default puerts;

{string.Join("", members.Select(name => $@"
export const {name} = puerts.{name};"))}
";
        }
        static string GenerateTemplatePuertsCommonjsCode()
        {
            return $@"
const puerts = (function () {{
    let _g = global || globalThis || this;
    return _g['puerts'] || require('puerts');
}})();

module.exports = puerts;
module.exports.default = puerts;
";
        }

        static string GenerateTemplateDTS(IEnumerable<string> namespaceNames)
        {
            return $@"
//===========================================================================================
//@ts-nocheck | ignore global error checking

//此功能适用于commonjs模块和esm模块;
//此功能旨在让puerts esm模块也能使用解构声明语法, 让commonjs和esm模块在typescript层语法一致, 如此我们就能自由转换工程为commonjs模块或esm模块而无需额外修改代码. 
//同时为csharp namespace创建额外的模块, 从而快速导入其中的类型.
//例:
//import {{ Array }} from 'csharp.System';
//import {{ File }} from 'csharp.System.IO';
//
//此处仅声明接口, 运行时js代码通过GeneratorECMAScript工具生成, 生成文件位置'Assets/Gen/Resources/puerts/modules';
//js运行时代码会通过`Puerts.ILoader`实例读取, 需自行处理csharp/puerts模块的读取, 详情请查看GeneratorECMAScript工具`README.md`:
//===========================================================================================

{string.Join("", namespaceNames.Select(name => $@"
declare module ""csharp.{name}"" {{
    import * as csharp from ""csharp"";
    export = csharp.{name};
}}
"))}
";
        }


        static string GetFilePath(string saveTo, string filename, bool isESM)
        {
            string extensionName = isESM ? "mjs" : "cjs";
#if !UNITY_2018_1_OR_NEWER
            extensionName += ".txt";
#endif
            return $"{GetRootPath(saveTo, false)}/{filename}.{extensionName}";
        }
        static string GetRootPath(string saveTo, bool isRoot = false)
        {
            return isRoot ? $"{saveTo}/Resources" : $"{saveTo}/Resources/puerts/modules";
        }
        static string GetDTSPath(string saveTo)
        {
            return $"{saveTo}/Typing/csharp/namespaces.d.ts";
        }
        static string GetManifestPath(string saveTo)
        {
            return $"{GetRootPath(saveTo, false)}/manifest.txt";
        }

        class NamespaceGenInfo
        {
            public string FirstName { get; private set; }
            public string Name { get; private set; }
            public TypeGenInfo[] Members { get; private set; }
            public List<string> Namespaces { get; private set; }
            public void AddChildNamespace(string @namespace)
            {
                this.Namespaces.Add(@namespace);
            }
            public static NamespaceGenInfo From(string fullName, TypeGenInfo[] members)
            {
                int lastIndex = fullName != null ? fullName.LastIndexOf(".") : -1;
                return new NamespaceGenInfo()
                {
                    FirstName = lastIndex >= 0 ? fullName.Substring(0, lastIndex) : string.Empty,
                    Name = lastIndex >= 0 ? fullName.Substring(lastIndex + 1) : fullName,
                    Members = members,
                    Namespaces = new List<string>()
                };
            }
        }
        class TypeGenInfo
        {
            public string FirstName { get; private set; }
            public string Name { get; private set; }
            public static TypeGenInfo From(string firstName, string name)
            {
                return new TypeGenInfo()
                {
                    FirstName = firstName == null ? string.Empty : firstName,
                    Name = name,
                };
            }
            public static TypeGenInfo From(string fullName)
            {
                int lastIndex = fullName.LastIndexOf(".");
                string firstName = lastIndex >= 0 ? fullName.Substring(0, lastIndex) : string.Empty;
                string name = lastIndex >= 0 ? fullName.Substring(lastIndex + 1) : fullName;
                return From(firstName, name);
            }
            public static TypeGenInfo From(Type type)
            {
                if (string.IsNullOrEmpty(type.FullName))
                    return null;
                string fullName;
                if (type.IsGenericType)
                {
                    var parts = type.FullName.Replace('+', '.').Split('`');
                    fullName = $"{parts[0]}${parts[1].Split('[')[0]}";
                }
                else
                {
                    fullName = type.FullName.Replace('+', '.');
                }
                return From(fullName);
            }
        }
    }
}
