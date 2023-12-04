using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_07 : MonoBehaviour
{
    void Start()
    {
        var app = XOR.Application.Instance;
        if (app == null || app.IsDestroyed)
        {
            Debug.LogWarning($"{nameof(XOR.Application)} not running");
            return;
        }
        app.Load("samples/07_ECMAScriptTool");
    }

}
