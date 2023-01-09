using UnityEngine;

namespace XOR
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    [CreateAssetMenu(fileName = "ScriptableObject", menuName = "XOR/ScriptableObject", order = 0)]
#endif
    public class TsScriptableObject : UnityEngine.ScriptableObject
    {
        [SerializeField]
        internal string ScriptId;       //脚本classId
        [SerializeField]
        internal string ScriptPath;     //脚本SourceFile路径
        [SerializeField]
        internal string Version;
    }
}