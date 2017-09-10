using System.Collections.Generic;
using System.Reflection;

namespace QTFK.Models
{
    public class EntityDescription
    {
        public EntityDescription(string id, PropertyInfo keyProp, string name, IEnumerable<string> fields)
        {
            Name = name;
            Fields = fields;
            Id = id;
            PropertyId = keyProp;
        }

        public string Name { get; }
        public IEnumerable<string> Fields { get; }
        public string Id { get; set; }
        public PropertyInfo PropertyId { get; }
    }
}