using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XOR;

public class Starter : MonoBehaviour
{
    void Awake()
    {
        var app = XOR.Application.GetInstance();
        app.Loader.AddLoader(new Puerts.ECMAScriptLoader());

#if UNITY_EDITOR
        //添加Editor Loader
        var output = GetOutputPath();
        app.Loader.AddLoader(new FileLoader(output, Path.GetDirectoryName(output)));
#else
        //添加Runtime Loader
        //ScriptPacker打包后的数据就是一串普通的二进制数据, 可以以任意方式从其他地方获取.
        //此处示例将ScriptPacker打包后的数据写入Resources目录, Runtime也将从Resources读取. 
        //webpack等构建的脚本也可以使用ScriptPacker进行一样的打包操作, 但需注意使用ThreadWorker时, webpack应为不同线程的脚本分别配置入口脚本
        var loader = new CacheLoader();
        foreach (var path in new string[] { scriptPath, moduleScriptPath })
        {
            var asset = Resources.Load<TextAsset>(path);
            if (asset == null || asset.bytes == null || asset.bytes.Length == 0)
                continue;
            var scripts = ScriptPacker.Unpack(asset.bytes, new GZipEncrypt());  //使用GZip解压文件
            if (scripts == null)
            {
                UnityEngine.Debug.LogWarning($"{path} unpack failure.");
                continue;
            }
            foreach (var script in scripts)
            {
                loader.AddScript(script.Key, script.Value);
            }
        }
        app.Loader.AddLoader(loader);
#endif
        var modules = new string[]{
            "main",
        };
        foreach (var module in modules)
        {
            app.Load(module);
        }

        TsComponent.Register(app.Env);
    }

    static readonly string scriptPath = "scripts";
    static readonly string moduleScriptPath = "modulesScripts";
    static readonly string[] moduleNames = new string[0];

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/XOR-Samples/Publish")]
    static void Publish()
    {
        string output = GetOutputPath(), root = Path.GetDirectoryName(output);

        Action<string, byte[]> WriteBinaryFile = (name, data) =>
        {
            string path = Path.Combine(UnityEngine.Application.dataPath, "XOR-Publish/Resources", name);
            if (!path.EndsWith(".bytes")) path += ".bytes";
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllBytes(path, data);
        };
        //使用GZip压缩文件
        var scripts = ScriptPacker.Scan(output);
        WriteBinaryFile(scriptPath, ScriptPacker.Pack(scripts, new GZipEncrypt()));
        Debug.Log($"Publish Scripts: {scripts.Count}\n{string.Join("\n", scripts.Keys)}");
        if (moduleNames != null && moduleNames.Length > 0)
        {
            scripts = ScriptPacker.ScanModule(root, moduleNames);
            WriteBinaryFile(moduleScriptPath, ScriptPacker.Pack(scripts, new GZipEncrypt()));
            Debug.Log($"Publish Module Scripts: {scripts.Count}\n{string.Join("\n", scripts.Keys)}");
        }

        UnityEditor.AssetDatabase.Refresh();
    }
    [UnityEditor.MenuItem("Tools/XOR-Samples/Publish-WebGL")]
    static void PublishWebGL()
    {
        int index = UnityEditor.EditorUtility.DisplayDialogComplex("提示", "请选择脚本风格ESM模块(.mjs)和CommonJS(.cjs)", "取消", "ESM", "CommonJS");
        if (index == 0)
            return;
        string extName = index == 1 ? ".mjs" : ".cjs";
        string output = GetOutputPath(), root = Path.GetDirectoryName(output);
        Action<string, string> WriteTextFile = (name, data) =>
        {
            string path = Path.Combine(UnityEngine.Application.dataPath, "XOR-Publish/Resources", name);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(path, data);
        };

        var scripts = ScriptPacker.Scan(output);
        foreach (var script in scripts)
        {
            WriteTextFile(script.Key.Replace(Path.GetExtension(script.Key), extName), script.Value);
        }
        Debug.Log($"Publish Scripts: {scripts.Count}\n{string.Join("\n", scripts.Keys)}");
        if (moduleNames != null && moduleNames.Length > 0)
        {
            scripts = ScriptPacker.ScanModule(root, moduleNames);
            foreach (var script in scripts)
            {
                WriteTextFile(script.Key.Replace(Path.GetExtension(script.Key), extName), script.Value);
            }
            Debug.Log($"Publish Module Scripts: {scripts.Count}\n{string.Join("\n", scripts.Keys)}");
        }

        UnityEditor.AssetDatabase.Refresh();
    }
    [UnityEditor.MenuItem("Tools/XOR-Samples/Clear")]
    static void Clear()
    {
        string path = Path.Combine(UnityEngine.Application.dataPath, "XOR-Publish/Resources");
        if (!Directory.Exists(path))
            return;
        Directory.Delete(path, true);
        UnityEditor.AssetDatabase.Refresh();
    }
    static string GetOutputPath()
    {
        string configFilePath = Settings.Load().project;
        return Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, Path.GetDirectoryName(configFilePath), "output"));
    }
#endif
}
