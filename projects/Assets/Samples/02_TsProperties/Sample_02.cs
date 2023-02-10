using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_02 : MonoBehaviour
{
    public XOR.TsProperties m_Target;
    void Start()
    {
        var app = XOR.Application.Instance;
        if (app == null || app.IsDestroyed)
        {
            Debug.LogWarning($"{nameof(XOR.Application)} not running");
            return;
        }
        string module = "samples/02_TsProperties";
        var func = app.Loader.IsESM(module) ?
            app.Env.ExecuteModule<Action<XOR.TsProperties>>(module, "init") :
            app.Env.Eval<Action<XOR.TsProperties>>($"var m = require('{module}'); m.init;");
        func(m_Target);
    }
}
