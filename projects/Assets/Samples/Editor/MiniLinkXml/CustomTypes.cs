using System;
using System.Collections.Generic;
using MiniLinkXml;
using UnityEngine.SceneManagement;

public class CustomTypes
{
    /// <summary>
    /// 直接配置需要保留的目标类型(必须是静态成员)
    /// </summary>
    [Link]
    readonly static List<Type> linkXmlCustomTypes = new List<Type>()
    {
        typeof(UnityEngine.GameObject),
        typeof(UnityEngine.Transform),
    };
    [Link]
    readonly static List<Type> defaultBaseTypes = new List<Type>()
    {
        typeof(Byte),
        typeof(SByte),
        typeof(Int16),
        typeof(UInt16),
        typeof(Int32),
        typeof(UInt32),
        typeof(Int64),
        typeof(UInt64),
        typeof(Char),
        typeof(Single),
        typeof(Double),
        typeof(String),
    };

    /// <summary>
    /// 自定义link.xml内容:
    /// 第一个成员为程序集名称, 之后的视为类型名称
    /// </summary>
    [Link]
    readonly static List<IEnumerable<string>> customConfigureXml = new List<IEnumerable<string>>()
    {
        new List<string>(){"System.Core", "System.Linq.Expressions.Interpreter.LightLambda" }
    };
}
