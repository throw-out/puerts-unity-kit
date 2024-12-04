using System;

namespace XOR.Behaviour
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    public class ArgsAttribute : Attribute
    {
        public Type[] Args { get; private set; }
        public ArgsAttribute(params Type[] args)
        {
            Args = args;
        }
    }
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    public class TitleAttribute : Attribute
    {
        public string Name { get; private set; }
        public TitleAttribute(string name)
        {
            Name = name;
        }
    }

}