using System;

namespace QTFK.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class PrefixAttribute : Attribute
    {
        public PrefixAttribute(Type dbEngine, string prefix)
        {
            Asserts.isSomething(dbEngine, "Parameter 'dbEngine' cannot be empty.");
            Asserts.isSomething(prefix, "Parameter 'prefix' cannot be empty.");

            DBEngine = dbEngine;
            Prefix = prefix;
        }

        public Type DBEngine { get; }
        public string Prefix { get; }

    }
}