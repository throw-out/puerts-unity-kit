using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{
    public class TsComponent : MonoBehaviour
    {
        [SerializeField]
        internal string ScriptId;       //脚本classId
        [SerializeField]
        internal string ScriptPath;     //脚本SourceFile路径
        [SerializeField]
        internal string Version;

    }
}
