using System;
using System.Collections;
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
        [MenuItem("PuerTS/Generate ECMAScript/ESM", false, 1)]
        public static void GenerateESM()
        {
            GenerateCode(true, false);
        }
        [MenuItem("PuerTS/Generate ECMAScript/ESM(AppDomain)", false, 1)]
        public static void GenerateESMAppDomain()
        {
            GenerateCode(true, true);
        }
        [MenuItem("PuerTS/Generate ECMAScript/CommonJS", false, 1)]
        public static void GenerateCommonjs()
        {
            GenerateCode(false, false);
        }
        [MenuItem("PuerTS/Generate ECMAScript/CommonJS(AppDomain)", false, 1)]
        public static void GenerateCommonjsAppDomain()
        {
            GenerateCode(false, true);
        }
        [MenuItem("PuerTS/Generate ECMAScript/Clear", false, 1)]
        public static void Clear()
        {
            string dirpath = GetDirectoryPath(Configure.GetCodeOutputDirectory(), true);
            if (!Directory.Exists(dirpath))
                return;
            Directory.Delete(dirpath, true);
            AssetDatabase.Refresh();
        }

        static void GenerateCode(bool isESM, bool isDomain)
        {
            Clear();

            var start = DateTime.Now;
            var saveTo = Configure.GetCodeOutputDirectory();
            GenerateCode(saveTo, isESM, isDomain);
            GenerateDTS(saveTo);
            Debug.Log("finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms");
            AssetDatabase.Refresh();
        }
        static void GenerateCode(string saveTo, bool isESM, bool isAppDomain)
        {

            IEnumerable<Type> genTypes;
            if (isAppDomain)
            {
                genTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                            from type in assembly.GetExportedTypes()
                            select type);
            }
            else
            {
                var configure = Configure.GetConfigureByTags(new List<string>() {
                    "Puerts.BindingAttribute",
                });
                genTypes = configure["Puerts.BindingAttribute"].Select(kv => kv.Key)
                    .Where(o => o is Type)
                    .Cast<Type>()
                    .Where(t => !t.IsGenericTypeDefinition);
            }
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

            Directory.CreateDirectory(GetDirectoryPath(saveTo, false));
            foreach (var info in genInfos)
            {
                string filepath = GetFilePath(
                    saveTo,
                    string.IsNullOrEmpty(info.Key) ? "csharp" : $"csharp.{info.Key}",
                    isESM
                );
                using (StreamWriter textWriter = new StreamWriter(filepath, false, Encoding.UTF8))
                {
                    textWriter.Write(isESM ? GenerateTemplateESMCode(info.Key, info.Value) : GenerateTemplateCommonjsCode(info.Key, info.Value));
                    textWriter.Flush();
                }
            }
            if (isESM)
            {
                string filepath = GetFilePath(saveTo, "puerts", isESM);
                using (StreamWriter textWriter = new StreamWriter(filepath, false, Encoding.UTF8))
                {
                    textWriter.Write(GenerateTemplatePuertsCode());
                    textWriter.Flush();
                }
            }
        }
        static void GenerateDTS(string saveTo)
        {
            var configure = Configure.GetConfigureByTags(new List<string>() {
                    "Puerts.BindingAttribute",
                });
            var genTypes = configure["Puerts.BindingAttribute"].Select(kv => kv.Key)
                .Where(o => o is Type)
                .Cast<Type>()
                .Where(t => !t.IsGenericTypeDefinition);

            var namespaces = Puerts.Editor.Generator.DTS.TypingGenInfo.FromTypes(genTypes).NamespaceInfos
                .Select(info => info.Name);

            string filepath = GetDTSPath(saveTo);
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            using (StreamWriter textWriter = new StreamWriter(filepath, false, Encoding.UTF8))
            {
                textWriter.Write(GenerateTemplateDTS(namespaces));
                textWriter.Flush();
            }
        }

        static string ProxyFunctions = @"
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
    let _g = this || global || globalThis;
    return _g['CS'] || _g['csharp'] || require('csharp');
}})();

{ProxyFunctions}

export default {firstName};

{string.Join("", typeNames.Select(name => $@"
export const {name} = __proxy__(() => {firstName}.{name});"))}

{string.Join("", namespaceNames.Select(name => $@"
export const {name} = __proxy__(() => {firstName}.{name});"))}
";
        }
        static string GenerateTemplateCommonjsCode(string firstName, NamespaceGenInfo genInfo)
        {
            firstName = string.IsNullOrEmpty(firstName) ? "csharp" : $"csharp.{firstName}";

            return $@"
const csharp = (function () {{
    let _g = this || global || globalThis;
    return _g['CS'] || _g['csharp'] || require('csharp');
}})();

module.exports = {firstName};
module.exports.default = {firstName};
";
        }
        static string GenerateTemplatePuertsCode()
        {
            string[] members;
            using (var env = new JsEnv())
            {
                members = env.Eval<string[]>(@"
(function () {
    const csharp = (function () {
        let _g = this || global || globalThis;
        return _g['CS'] || _g['csharp'] || require('csharp');
    })();
    const puerts = (function () {
        let _g = this || global || globalThis;
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
    let _g = this || global || globalThis;
    return _g['puerts'] || require('puerts');
}})();

export default puerts;

{string.Join("", members.Select(name => $@"
export const {name} = puerts.{name};"))}
";
        }
        static string GenerateTemplateDTS(IEnumerable<string> namespaceNames)
        {
            namespaceNames = namespaceNames.Distinct().Where(name => !string.IsNullOrEmpty(name));
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
            string extensionName =
#if UNITY_2018_1_OR_NEWER
                isESM ? "mjs" : "cjs";
#else
                ".txt";
#endif
            return $"{GetDirectoryPath(saveTo, false)}/{filename}.{extensionName}";
        }
        static string GetDirectoryPath(string saveTo, bool isRoot = false)
        {
            return isRoot ? $"{saveTo}/Resources" : $"{saveTo}/Resources/puerts/modules";
        }
        static string GetDTSPath(string saveTo)
        {
            return $"{saveTo}/Typing/csharp/namespaces.d.ts";
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
