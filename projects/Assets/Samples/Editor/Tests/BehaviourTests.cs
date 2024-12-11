using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using XOR;

public static class BehaviourTests
{
    private static bool EnterPlayingTests
    {
        get
        {
            return EditorPrefs.GetBool("BehaviourTests.Timestamp", false);
        }
        set
        {
            EditorPrefs.SetBool("BehaviourTests.Timestamp", value);
        }
    }

    [MenuItem("Tools/XOR-Samples/Run BehaviourTests", false, int.MaxValue)]
    static void RunTests()
    {
        var output = GetTestsProjectOutput();
        if (!Directory.Exists(output) || !File.Exists(Path.Combine(output, "tests/main.js")) && !File.Exists(Path.Combine(output, "tests/main.mjs")))
        {
            var root = Path.GetDirectoryName(output);
            EditorUtility.DisplayDialog("提示", $"请先编译Tests项目, 请在以下目录中执行\"tsc -p tsconfig.json --module ES6\":\n{root}", "确定");
            EditorUtility.RevealInFinder(root);
            return;
        }

        if (EditorApplication.isPlaying)
        {
            RunBehaviourTests();
        }
        else
        {
            EnterPlayingTests = true;
            EditorApplication.isPlaying = true;
        }
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredPlayMode)
            return;
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        if (EnterPlayingTests)
        {
            EnterPlayingTests = false;
            RunBehaviourTests();
        }
    }

    static void RunBehaviourTests()
    {
        void Release()
        {
            TsComponent.Unregister();
            TsBehaviour.DisposeAll();
            TsComponent.DisposeAll();

            //XOR.Application.ReleaseInstance();
            XOR.Behaviour.Invoker.Default = null;
        }
        Release();
        XOR.Application.ReleaseInstance();

        //构建XOR.Application实例
        var app = XOR.Application.GetInstance();
        app.Loader.AddLoader(new Puerts.ECMAScriptLoader());
        //添加Editor Loader
        var output = GetTestsProjectOutput();
        app.Loader.AddLoader(new FileLoader(output, Path.GetDirectoryName(output)));

        TsComponent.Register(app.Env);
        try
        {
            var run = app.Load<Action<Action>>("tests/main", "run");
            run(Release);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Release();
        }

    }
    static string GetTestsProjectOutput()
    {
        return Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "TsEditorProject/output");
    }
}
