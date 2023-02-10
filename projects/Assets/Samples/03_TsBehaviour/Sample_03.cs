using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_03 : MonoBehaviour
{
    void Start()
    {
        var app = XOR.Application.Instance;
        if (app == null || app.IsDestroyed)
        {
            Debug.LogWarning($"{nameof(XOR.Application)} not running");
            return;
        }
        string module = "samples/03_TsBehaviour";
        var func = app.Loader.IsESM(module) ?
            app.Env.ExecuteModule<Action>(module, "init") :
            app.Env.Eval<Action>($"var m = require('{module}'); m.init;");
        func();
    }
}
