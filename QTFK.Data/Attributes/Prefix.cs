using System;

namespace QTFK.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class PrefixAttribute : Attribute
    {
        public PrefixAttribute(Type dbEngine, string prefix)
        {
            DBEngine = dbEngine;
            Prefix = prefix;
        }

        public Type DBEngine { get; }
        public string Prefix { get; }

    }
}