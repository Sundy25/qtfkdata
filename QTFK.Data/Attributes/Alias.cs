using System;

namespace QTFK.Attributes
{
    public class AliasAttribute : Attribute
    {

        public AliasAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}