using System;
using System.Collections.Generic;
using MiniLinkXml;
using UnityEngine.SceneManagement;

public class CustomTypes
{
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
}
