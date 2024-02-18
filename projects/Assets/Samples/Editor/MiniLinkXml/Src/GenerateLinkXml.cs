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
        [MenuItem("Tools/XOR-Tools/Generate Mini link.xml")]
        public static void GenerateXml()
        {
            string filePath = EditorUtility.OpenFilePanelWithFilters("Select tsconfig.json", string.Empty, new string[]{
                "json", "json"
            });
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            string[] typeNames = ResolveReferencesTypes(filePath);
            var assemblies = typeNames
                .Select(tn => GetType(tn))
                .Where(t => t != null)
                .Distinct()
                .GroupBy(t => t.Assembly.GetName().Name)
                .ToDictionary(group => group.Key, group => group.Cast<Type>().Select(t => t.FullName).ToArray());

            //保存文件
            string saveTo = Path.Combine(Application.dataPath, "link.xml");
            if (File.Exists(saveTo))
            {
                bool ok = EditorUtility.DisplayDialog(
                    "Tip",
                    "The link.xml file already exists, overwrite it?",
                    "Confirm",
                    "Cancel"
                );
                if (!ok)
                    return;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(saveTo));
            using (StreamWriter textWriter = new StreamWriter(saveTo, false, Encoding.UTF8))
            {
                textWriter.Write(GenerateTemplateXML(assemblies));
                textWriter.Flush();
            }

            //刷新资源页面
            AssetDatabase.Refresh();
        }

        static string[] ResolveReferencesTypes(string filePath)
        {
            using (JsEnv env = new JsEnv())
            {
                if (!(env.Backend is BackendNodeJS))
                    throw new Exception("environment not supported: must is nodejs plugins");

                Puerts.ThirdParty.CommonJS.InjectSupportForCJS(env);
                var load = env.Eval<Func<string, string[]>>(@"
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

    return function (tsconfigFile) {
        const { types, underlyingTypes } = generate(tsconfigFile);
        if (!types && !underlyingTypes) {
            throw new Error(`generate failure!`);
        }

        let results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.String), types.length);
        for (let i = 0; i < types.length; i++) {
            results.set_Item(i, types[i]);
        }
        return results;
    }
})();
");

                return load(filePath);
            }
        }
        static Type GetType(string typeName)
        {
            Type result = Type.GetType(typeName, false);
            if (result == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    result = assembly.GetType(typeName, false);
                    if (result != null)
                        break;
                }
            }
            return result;
        }
        static string GenerateTemplateXML(Dictionary<string, string[]> assemblies)
        {
            if (assemblies == null || assemblies.Count == 0)
            {
                return $@"<linker></linker>";
            }

            return $@"
<linker>
    {string.Join("", assemblies.Keys.Select(assemblyName => $@"
    <assembly fullname=""{assemblyName}"">
        {string.Join("", assemblies[assemblyName].Select(typeName => $@"
        <type fullname=""{typeName}"" preserve=""all""/>
        "))}
    </assembly>
"))}
</linker>
";
        }
    }
}
