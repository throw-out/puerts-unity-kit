using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Puerts;
using UnityEditor;
using UnityEngine;

namespace MiniLinkXml
{
    public static class GenerateLinkXml
    {
        [MenuItem("Tools/MiniLinkXml/Generate link.xml")]
        public static void GenerateXml()
        {
            Generate(true, false);
            //刷新资源页面
            AssetDatabase.Refresh();
        }
        [MenuItem("Tools/MiniLinkXml/Generate link.cs")]
        public static void GenerateScript()
        {
            Generate(false, true);
            //刷新资源页面
            AssetDatabase.Refresh();
        }
        [MenuItem("Tools/MiniLinkXml/Generate link.xml + link.cs")]
        public static void GenerateBoth()
        {
            Generate(true, true);
            //刷新资源页面
            AssetDatabase.Refresh();
        }

        static void Generate(bool writeXml, bool writeScript)
        {
            string filePath = EditorUtility.OpenFilePanelWithFilters("Select tsconfig.json", string.Empty, new string[]{
                "json", "json"
            });
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;
            if (!Path.GetFileName(filePath).Equals("tsconfig.json"))
            {
                bool _continue = EditorUtility.DisplayDialog(
                    "Tip",
                    "The file you selected is not tsconfig.json. Do you want to continue?",
                    "Continue",
                    "Cancel"
                );
                if (!_continue)
                    return;
            }

            //文件保存路径
            string saveAsXml = Path.Combine(Application.dataPath, "link.xml");
            string saveAsScript = Path.Combine(Application.dataPath, "link.cs");

            //检查link.xml文件是否已存在
            if (writeXml && File.Exists(saveAsXml))
            {
                bool _continue = EditorUtility.DisplayDialog(
                    "Tip",
                    "The file link.xml already exists. Do you want to overwrite it?",
                    "Overwrite",
                    "Cancel"
                );
                if (!_continue)
                    return;
            }

            ResolveResults results = ResolveReferencesTypes(filePath);
            IEnumerable<Type> baseTypes = results.underlyingTypes
                .SelectMany(tn => Utils.GetTypes(tn))
                .Concat(GetCustomTypes())
                .Concat(GetExtensionMethodsCallTypes(results.callMethods))
                .Where(t => t != null)
                .Distinct();

            //写入link.xml文件
            if (writeXml)
            {
                IEnumerable<Type> types = baseTypes
                    .Where(t => t != null && !IsExcludeType(t, true));

                Directory.CreateDirectory(Path.GetDirectoryName(saveAsXml));
                using (StreamWriter textWriter = new StreamWriter(saveAsXml, false, Encoding.UTF8))
                {
                    textWriter.Write(GenerateTemplateXml(types, GetCustomConfigure()));
                    textWriter.Flush();
                }
            }

            //写入link.cs文件
            if (writeScript)
            {
                IEnumerable<Type> types = baseTypes
                    .Concat(results.types.SelectMany(tn => Utils.GetTypes(tn)))
                    .Where(t => t != null && !IsExcludeType(t, false))
                    .Distinct();

                Directory.CreateDirectory(Path.GetDirectoryName(saveAsScript));
                using (StreamWriter textWriter = new StreamWriter(saveAsScript, false, Encoding.UTF8))
                {
                    textWriter.Write(GenerateTemplateScript(types));
                    textWriter.Flush();
                }
            }
        }

        static ResolveResults ResolveReferencesTypes(string filePath)
        {
            using (JsEnv env = new JsEnv(new ProxyAssertLoader()))
            {
                if (!(env.Backend is BackendNodeJS))
                    throw new Exception("environment not supported: must is nodejs plugins");

                Puerts.ThirdParty.CommonJS.InjectSupportForCJS(env);
                var resolve = env.Eval<Action<string, ResolveResults>>(@"
//加载工具类方法
require('puerts/xor-tools/link.xml');

(function(){
    //获取generate方法
    const generate = (function () {
        var _g = global || globalThis || this;
        if (!_g || !_g.generate || typeof (_g.generate) !== 'function')
            throw new Error(`invaild environment`);
        return _g.generate;
    })();

    function toStringArray(arr) {
        if(!arr || !Array.isArray(arr))
            return null;

        let results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.String), arr.length);
        for (let i = 0; i < arr.length; i++) {
            results.set_Item(i, arr[i]);
        }
        return results;
    }

    return function (tsconfigFile, results) {
        const { types, underlyingTypes, callMethods } = generate(tsconfigFile);
        if (!types && !underlyingTypes) {
            throw new Error(`generate failure!`);
        }

        results.types = toStringArray(types);
        results.underlyingTypes = toStringArray(underlyingTypes);
        if (callMethods) {
            for (let [typeName, methods] of callMethods) {
                results.AddCallMethods(typeName, toStringArray(methods));
            }
        }
    }
})();
");
                ResolveResults results = new ResolveResults();
                resolve(filePath, results);
                return results;
            }
        }
        static IEnumerable<Type> GetCustomTypes()
        {
            List<Type> results = new List<Type>();
            Utils.ForEachLinkConfigure((type, getValue) =>
            {
                if (!typeof(IEnumerable<Type>).IsAssignableFrom(type))
                    return;
                var cfg = getValue() as IEnumerable<Type>;
                if (cfg != null) results.AddRange(cfg);
            });
            return results;
        }
        static Dictionary<string, IEnumerable<string>> GetCustomConfigure()
        {
            Dictionary<string, HashSet<string>> results = new Dictionary<string, HashSet<string>>();
            void AddConfigure(IEnumerable<string> cfg)
            {
                if (cfg == null || cfg.Count() < 2)
                    return;
                string assemblyName = cfg.ElementAt(0);
                HashSet<string> types;
                if (!results.TryGetValue(assemblyName, out types))
                {
                    types = new HashSet<string>();
                    results.Add(assemblyName, types);
                }
                Array.ForEach(cfg.Skip(1).ToArray(), typeName => types.Add(typeName));
            }
            Utils.ForEachLinkConfigure((type, getValue) =>
            {
                if (typeof(List<List<string>>).IsAssignableFrom(type))
                {
                    var cfg = getValue() as List<List<string>>;
                    if (cfg != null) Array.ForEach(cfg.ToArray(), m => AddConfigure(m));
                }
                else if (typeof(List<IEnumerable<string>>).IsAssignableFrom(type))
                {
                    var cfg = getValue() as List<IEnumerable<string>>;
                    if (cfg != null) Array.ForEach(cfg.ToArray(), m => AddConfigure(m));
                }
            });
            return results.ToDictionary(o => o.Key, o => o.Value as IEnumerable<string>);
        }
        static IEnumerable<Type> GetExtensionMethodsCallTypes(Dictionary<string, string[]> callMethods)
        {
            if (callMethods == null)
                return new Type[0];
            Dictionary<Type, HashSet<string>> _callMethods = new Dictionary<Type, HashSet<string>>();
            foreach (var t in callMethods)
            {
                var type = Utils.GetType(t.Key);
                if (type == null)
                    continue;
                _callMethods.Add(type, new HashSet<string>(t.Value));
            }

            List<Type> results = new List<Type>();
            Utils.ForEachExtensionMethodDeclaration((clsType, methodName, thisArgType) =>
            {
                foreach (var t in _callMethods)
                {
                    if (!t.Value.Contains(methodName))
                        continue;
                    if (!thisArgType.IsAssignableFrom(t.Key))
                        continue;
                    results.Add(clsType);
                    break;
                }
            });
            return results;
        }

        static List<Func<Type, bool, bool>> excludeFuncs;
        static bool IsExcludeType(Type type, bool isLinkXml)
        {
            if (excludeFuncs == null)
            {
                excludeFuncs = new List<Func<Type, bool, bool>>();
                Utils.ForEachFilterFunction((methodInfo) =>
                {
                    if (methodInfo.ReturnType != typeof(bool))
                        return;
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Type))
                    {
                        excludeFuncs.Add((type, isLinkXml) => (bool)methodInfo.Invoke(null, new object[] { type }));
                    }
                    if (parameters.Length == 2 &&
                        parameters[0].ParameterType == typeof(Type) &&
                        parameters[1].ParameterType == typeof(bool))
                    {
                        excludeFuncs.Add((type, isLinkXml) => (bool)methodInfo.Invoke(null, new object[] { type, isLinkXml }));
                    }
                });
            }
            return excludeFuncs.Any(filter => !filter(type, isLinkXml));
        }

        static string GenerateTemplateXml(IEnumerable<Type> types, Dictionary<string, IEnumerable<string>> customConfigure = null)
        {
            if (types == null)
            {
                types = new Type[0];
            }

            var preserveAll = new HashSet<string>();
            var preserveTypes = types
                .GroupBy(t => t.Assembly.GetName().Name)
                .ToDictionary(group => group.Key, group => group.Cast<Type>().Select(t => t.FullName).ToList());
            if (customConfigure != null)
            {
                foreach (var assemblyName in customConfigure.Keys)
                {
                    if (customConfigure[assemblyName].Contains("*"))
                    {
                        preserveAll.Add(assemblyName);
                        if (preserveTypes.ContainsKey(assemblyName))
                        {
                            preserveTypes.Remove(assemblyName);
                        }
                    }
                    else
                    {
                        List<string> _types;
                        if (!preserveTypes.TryGetValue(assemblyName, out _types))
                        {
                            _types = new List<string>();
                            preserveTypes.Add(assemblyName, _types);
                        }
                        preserveTypes[assemblyName] = _types.Concat(customConfigure[assemblyName]).Distinct().ToList();
                    }
                }
            }
            return $@"
<!--
此配置由MiniLinkXml工具生成, 无需手动编辑.
This configuration is generated by the MiniLinkXml tool and does not require manual editing.
-->
<linker>
    {string.Join("", preserveAll.Select(assemblyName => $@"
    <assembly fullname=""{assemblyName}"" preserve=""all""/>")).Trim()}

    {string.Join("", preserveTypes.Keys.Select(assemblyName => $@"
    <assembly fullname=""{assemblyName}"">
        {string.Join("", preserveTypes[assemblyName].Select(typeName => $@"
        <type fullname=""{typeName}"" preserve=""all""/> ")).Trim()}
    </assembly>
"))}
</linker>
";
        }
        static string GenerateTemplateScript(IEnumerable<Type> types)
        {
            if (types == null)
            {
                types = new Type[0];
            }
            return $@"
using System;
using System.Collections.Generic;

/// <summary>
/// 此配置由MiniLinkXml工具生成, 无需手动编辑.
/// This configuration is generated by the MiniLinkXml tool and does not require manual editing.
/// </summary>
public class LinkXmlReferences
{{
    internal static List<Type> types;
    static void References()
    {{
        types = new List<Type>()
        {{
            {string.Join("", types.Select(type => $@"
            typeof({Utils.GetFullname(type)}), ")).Trim()}
        }};
    }}
}}
";
        }

        class ResolveResults
        {
            public string[] types;
            public string[] underlyingTypes;
            public Dictionary<string, string[]> callMethods;

            public void AddCallMethods(string typeName, string[] methods)
            {
                if (methods == null || methods.Length == 0)
                    return;

                if (callMethods == null)
                {
                    callMethods = new Dictionary<string, string[]>();
                }
                callMethods[typeName] = methods;
            }
        }
    }
}
