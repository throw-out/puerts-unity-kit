using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XOR;

public class TsComponentRegister : MonoBehaviour
{
    void Awake()
    {
        var app = XOR.Application.GetInstance();
#if UNITY_EDITOR
        string projectRoot = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "TsProject");
        string outputRoot = Path.Combine(projectRoot, "output");
        app.Loader.AddLoader(new FileLoader(outputRoot, projectRoot));
#else
        //添加Runtime Loader
#endif
        app.Env.Eval("require('./main')");
        app.Env.Eval("require('./samples/01_TsComponent')");

        TsComponent.Register(app.Env);
    }
}
