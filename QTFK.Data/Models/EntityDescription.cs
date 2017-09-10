using System.Collections.Generic;

namespace QTFK.Models
{
    public class EntityDescription
    {
        public EntityDescription(string name, IEnumerable<string> fields)
        {
            Name = name;
            Fields = fields;
        }

        public string Name { get; }
        public IEnumerable<string> Fields { get; }
    }
}