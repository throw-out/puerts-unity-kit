using Puerts;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;
using System.Collections;
using XOR;

[Configure]
public class PuertsConfig
{
    class SingletonExportDeclare : Singleton<SingletonExportDeclare> { }
    class SingletonMonoExportDeclare : SingletonMonoBehaviour<SingletonMonoExportDeclare> { }

    [Typing]
    static IEnumerable<Type> Typeing
    {
        get
        {
            return new List<Type>()
            {
                //Unity Engine
                typeof(Handheld),
                typeof(IEnumerable),
                typeof(IEnumerable<int>),
                typeof(Dictionary<int,int>),
                new Dictionary<int,int>().Keys.GetType(),
                //Custom
                typeof(ProxyAction<bool>),
                typeof(Singleton<SingletonExportDeclare>),
                typeof(SingletonMonoBehaviour<SingletonMonoExportDeclare>),
                //System.Action
                typeof(Action),
                typeof(Action<int, int>),
                //XOR
                typeof(XOR.Services.CSharpInterfaces),
                typeof(XOR.Services.TSInterfaces),
                typeof(XOR.Services.Program),
                typeof(XOR.Services.ProgramState),
                typeof(XOR.Services.Statement),
                typeof(XOR.Services.EnumDeclaration),
                typeof(XOR.Services.TypeDeclaration),
                typeof(XOR.Services.PropertyDeclaration),
                typeof(XOR.Services.EnumPropertyDeclaration),
                typeof(XOR.ReflectionUtil),
            };
        }
    }
    static IEnumerable<Type> Bindings
    {
        get
        {
            return new List<Type>()
            {
                //Unity Engine
#if !UNITY_STANDALONE_WIN
                typeof(Handheld), //Windows平台无法调用
#endif
                typeof(IEnumerator),
                typeof(Coroutine),
                typeof(UnityEventBase),
                typeof(UnityEvent),
                typeof(UnityEvent<int>),
                typeof(UnityEvent<int,int>),
                typeof(UnityEvent<int,int,int>),
                typeof(UnityAction),
                typeof(UnityAction<int>),
                typeof(UnityAction<int,int>),
                typeof(UnityAction<int,int,int>),
                //C# System
                typeof(Array),
                typeof(Guid),
                typeof(Hashtable),
                typeof(Delegate),
                typeof(DateTime),
                typeof(System.Object),
                typeof(List<int>),
                typeof(Dictionary<int,int>),
                //C# System.Reflection
                typeof(Type),
                typeof(MemberInfo),
                typeof(MethodBase),
                typeof(MethodInfo),
                typeof(ConstructorInfo),
                typeof(PropertyInfo),
                typeof(FieldInfo),
                typeof(ParameterInfo),
                //C# System.IO
                typeof(File),
                typeof(Directory),
                typeof(FileInfo),
                typeof(DirectoryInfo),
                typeof(Path),
                typeof(Stream),
                //C# System.Text
                typeof(string),
                typeof(System.Text.Encoding),
                //Puerts
                typeof(JsEnv),
                typeof(ILoader),
            };
        }
    }

    [BlittableCopy]
    static IEnumerable<Type> Blittables
    {
        get
        {
            return new List<Type>()
            {
                //打开这个可以优化Vector3的GC，但需要开启unsafe编译
                //typeof(Vector3),
            };
        }
    }

    [Binding]
    static IEnumerable<Type> DynamicBindings
    {
        get
        {
            // 在这里添加名字空间
            var namespaces = new List<string>()
            {
                //Unity Api
                "UnityEngine",
                "UnityEngine.UI",

                "Puerts",
                "XOR",
                "XOR.Services",
            };
            var selectTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                               where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                               from type in assembly.GetExportedTypes()
                               where type.Namespace != null && namespaces.Contains(type.Namespace) && !IsExcluded(type)
                               select type);

            string[] customAssemblys = new string[] {
                "Assembly-CSharp",
            };
            var customTypes = (from assembly in customAssemblys.Select(s => Assembly.Load(s))
                               where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                               from type in assembly.GetExportedTypes()
                               where !IsExcluded(type)
                               select type);

            return selectTypes
                .Concat(customTypes)
                .Concat(Bindings)
                .Distinct();
        }
    }
    static bool IsExcluded(Type type)
    {
        if (type == null)
            return false;
        if (string.IsNullOrEmpty(type.FullName))
            return true;

        string assemblyName = Path.GetFileName(type.Assembly.Location);
        if (excludeAssemblys.Contains(assemblyName))
            return true;

        string fullname = type.FullName != null ? type.FullName.Replace("+", ".") : "";
        if (excludeTypes.Contains(fullname))
            return true;

        if (type.BaseType != null &&
            !string.IsNullOrEmpty(type.BaseType.FullName) &&
            type != typeof(System.Object) &&
            type != typeof(System.ValueType) &&
            type != typeof(void) &&
            !baseTypes.Contains(type.BaseType.FullName.Replace("+", ".")))
        {
            return IsExcluded(type.BaseType);
        }
        return false;
    }
    static List<string> excludeAssemblys = new List<string>{
        "UnityEditor.dll",
        "Assembly-CSharp-Editor.dll",
        "com.tencent.puerts.core.Editor.dll",
        "puerts.unity.kit.xor.Editor.dll",
    };
    static List<string> baseTypes = new List<string>
    {
        //System ValueType
        "System.Void", "System.Tuple", "System.ValueType",
        "System.String", "System.Single", "System.Double", "System.Boolean",
        "System.Byte", "System.SByte", "System.Int16", "System.Int32",
        "System.Int64", "System.UInt16", "System.UInt32", "System.UInt64", "System.ArgIterator",
        "System.SpanExtensions", "System.TypedReference", "System.StringBuilderExt",
    };
    static List<string> excludeTypes = new List<string>(baseTypes)
    {
        "UnityEngine.iPhone",
        "UnityEngine.iPhoneTouch",
        "UnityEngine.iPhoneKeyboard",
        "UnityEngine.iPhoneInput",
        "UnityEngine.iPhoneAccelerationEvent",
        "UnityEngine.iPhoneUtils",
        "UnityEngine.iPhoneSettings",
        "UnityEngine.AndroidInput",
        "UnityEngine.AndroidJavaProxy",
        "UnityEngine.BitStream",
        "UnityEngine.ADBannerView",
        "UnityEngine.ADInterstitialAd",
        "UnityEngine.RemoteNotification",
        "UnityEngine.LocalNotification",
        "UnityEngine.NotificationServices",
        "UnityEngine.MasterServer",
        "UnityEngine.Network",
        "UnityEngine.NetworkView",
        "UnityEngine.ParticleSystemRenderer",
        "UnityEngine.ParticleSystem.CollisionEvent",
        "UnityEngine.ProceduralPropertyDescription",
        "UnityEngine.ProceduralTexture",
        "UnityEngine.ProceduralMaterial",
        "UnityEngine.ProceduralSystemRenderer",
        "UnityEngine.TerrainData",
        "UnityEngine.HostData",
        "UnityEngine.RPC",
        "UnityEngine.AnimationInfo",
        "UnityEngine.UI.IMask",
        "UnityEngine.Caching",
        "UnityEngine.Handheld",
        "UnityEngine.MeshRenderer",
        "UnityEngine.UI.DefaultControls",
        "UnityEngine.AnimationClipPair", //Obsolete
        "UnityEngine.CacheIndex", //Obsolete
        "UnityEngine.SerializePrivateVariables", //Obsolete
        "UnityEngine.Networking.NetworkTransport", //Obsolete
        "UnityEngine.Networking.ChannelQOS", //Obsolete
        "UnityEngine.Networking.ConnectionConfig", //Obsolete
        "UnityEngine.Networking.HostTopology", //Obsolete
        "UnityEngine.Networking.GlobalConfig", //Obsolete
        "UnityEngine.Networking.ConnectionSimulatorConfig", //Obsolete
        "UnityEngine.Networking.DownloadHandlerMovieTexture", //Obsolete
        "AssetModificationProcessor", //Obsolete
        "AddressablesPlayerBuildProcessor", //Obsolete
        "UnityEngine.WWW", //Obsolete
        "UnityEngine.EventSystems.TouchInputModule", //Obsolete
        "UnityEngine.MovieTexture", //Obsolete[ERROR]
        "UnityEngine.NetworkPlayer", //Obsolete[ERROR]
        "UnityEngine.NetworkViewID", //Obsolete[ERROR]
        "UnityEngine.NetworkMessageInfo", //Obsolete[ERROR]
        "UnityEngine.UI.BaseVertexEffect", //Obsolete[ERROR]
        "UnityEngine.UI.IVertexModifier", //Obsolete[ERROR]
        //Windows Obsolete[ERROR]
        "UnityEngine.EventProvider",
        "UnityEngine.UI.GraphicRebuildTracker",
        "UnityEngine.GUI.GroupScope",
        "UnityEngine.GUI.ScrollViewScope",
        "UnityEngine.GUI.ClipScope",
        "UnityEngine.GUILayout.HorizontalScope",
        "UnityEngine.GUILayout.VerticalScope",
        "UnityEngine.GUILayout.AreaScope",
        "UnityEngine.GUILayout.ScrollViewScope",
        "UnityEngine.GUIElement",
        "UnityEngine.GUILayer",
        "UnityEngine.GUIText",
        "UnityEngine.GUITexture",
        "UnityEngine.ClusterInput",
        "UnityEngine.ClusterNetwork",
    };
}