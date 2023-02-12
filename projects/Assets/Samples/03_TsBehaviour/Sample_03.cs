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
        var func = app.Load<Action>("samples/03_TsBehaviour", "init");
        func();
    }
}
