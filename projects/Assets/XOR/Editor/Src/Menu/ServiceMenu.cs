using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    [InitializeOnLoad]
    public class ServiceMenu
    {
        static ServiceMenu()
        {
            EditorApplication.update += Update;

            if (IsEnableTsService && EnableValidate())
            {
                Enable();
            }
        }

        static void Update()
        {
            TsServiceProcess.Instance?.Tick();
        }

        static bool IsEnableTsService
        {
            get { return EditorPrefs.GetBool("Editor.EnableTsService", true); }
            set { EditorPrefs.SetBool("Editor.EnableTsService", value); }
        }
        [MenuItem("Tools/XOR/Reload")]
        static void Reload()
        {
            TsServiceProcess.ReleaseInstance();
            Enable();
        }
        [MenuItem("Tools/XOR/Enable")]
        static void Enable()
        {
            IsEnableTsService = true;
            try
            {
                TsServiceProcess process = TsServiceProcess.GetInstance();
                process.Env.Eval("require('./main')");

                Debug.Log($"XOR {nameof(TsServiceProcess)} Enable");
            }
            catch (Exception e)
            {
                TsServiceProcess.ReleaseInstance();
                throw e;
            }
        }
        [MenuItem("Tools/XOR/Enable", true)]
        static bool EnableValidate() => TsServiceProcess.Instance == null || !IsEnableTsService;
        [MenuItem("Tools/XOR/Disable")]
        static void Disable()
        {
            IsEnableTsService = false;
            TsServiceProcess.ReleaseInstance();

            Debug.Log($"XOR {nameof(TsServiceProcess)} Disable");
        }
        [MenuItem("Tools/XOR/Disable", true)]
        static bool DisableValidate() => !EnableValidate();
    }
}
