using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_10 : MonoBehaviour
{
    public GameObject m_Target;
    void Start()
    {
        var app = XOR.Application.Instance;
        if (app == null || app.IsDestroyed)
        {
            Debug.LogWarning($"{nameof(XOR.Application)} not running");
            return;
        }
        string module = "samples/10_BuildDemo";
        var func = app.Loader.IsESM(module) ?
            app.Env.ExecuteModule<Action<GameObject>>(module, "init") :
            app.Env.Eval<Action<GameObject>>($"var m = require('{module}'); m.init;");
        func(m_Target);
    }
}
