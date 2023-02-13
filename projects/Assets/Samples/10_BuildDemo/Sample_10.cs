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
        var func = app.Load<Action<GameObject>>("samples/10_BuildDemo", "init");
        func(m_Target);
    }
}
