﻿using System;
using System.Collections.Generic;
using MiniLinkXml;

public class CustomTypes
{
    /// <summary>
    /// 直接配置需要保留的目标类型(必须是静态字段或属性)
    /// </summary>
    [Link]
    readonly static List<Type> customTypes = new List<Type>()
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
        new List<string>(){"System.Core", "System.Linq.Expressions.Interpreter.LightLambda" },
        //保留程序集下所有类
        new List<string>(){"com.tencent.puerts.core", "*" },
        new List<string>(){"com.tencent.puerts.commonjs", "*" },
        new List<string>(){"com.tencent.puerts.webgl", "*" },
    };

    /// <summary>
    /// 自定义过滤Type列表
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isLinkXml"></param>
    /// <returns>返回false表示不生成对应的Type</returns>
    [Filter]
    static bool FilterType(Type type, bool isLinkXml)
    {
        if (isLinkXml)
            return true;
        if (type.FullName == "Unity.VisualScripting.TypeUtility" ||
            type.FullName == "Unity.VisualScripting.ComponentHolderProtocol")
        {
            return false;
        }

        return true;
    }
}
