using System;

namespace MiniLinkXml
{
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class LinkAttribute : Attribute
    {
    }
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class LinkXmlAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class FilterAttribute : Attribute
    {
    }
}