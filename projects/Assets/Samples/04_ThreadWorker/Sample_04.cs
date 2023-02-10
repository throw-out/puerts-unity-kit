using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_04 : MonoBehaviour
{
    void Start()
    {
        var app = XOR.Application.Instance;
        if (app == null || app.IsDestroyed)
        {
            Debug.LogWarning($"{nameof(XOR.Application)} not running");
            return;
        }
        string module = "samples/04_ThreadWorker/main";
        var func = app.Loader.IsESM(module) ?
            app.Env.ExecuteModule<Action<Puerts.ILoader>>(module, "init") :
            app.Env.Eval<Action<Puerts.ILoader>>($"var m = require('{module}'); m.init;");
        func(app.Loader);
    }
}
