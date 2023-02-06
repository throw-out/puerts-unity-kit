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
        var func = app.Env.Eval<Action<XOR.TsProperties>>("var m = require('./samples/02_TsProperties'); m.init;");
        func(m_Target);
    }
}
