using System;

namespace QTFK.Attributes
{
    public class AliasAttribute : Attribute
    {
        public AliasAttribute(string name)
        {
            Asserts.isSomething(name, "Parameter 'name' cannot be empty.");
            Name = name;
        }

        public string Name { get; }
    }
}