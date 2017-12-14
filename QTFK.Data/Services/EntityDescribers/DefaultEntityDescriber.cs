using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;
using System.Reflection;
using QTFK.Attributes;
using QTFK.Extensions.TypeInfo;
using System.Collections.ObjectModel;

namespace QTFK.Services.EntityDescribers
{
    public class DefaultEntityDescriber : IEntityDescriber
    {
        const string DEFAULT_ID_FIELD = "id";

        public IEntityDescription describe(Type entityType)
        {
            PrvEntityDescription entityDescription;
            IEnumerable<PropertyInfo> fields;

            entityDescription = new PrvEntityDescription
            {
                Name = entityType.getNameOrAlias(),
                Entity = entityType,
            };

            fields = entityType
                .GetProperties()
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var field in fields)
            {
                string fieldName;

                fieldName = field.getNameOrAlias();
                if (field.isKey())
                    entityDescription.Keys.Add(fieldName, field);
                else
                    entityDescription.Fields.Add(fieldName, field);
            }
            if(entityDescription.Keys.Count == 0)
            {
                KeyValuePair<string, PropertyInfo> id;
                StringComparer comparer;

                comparer = StringComparer.InvariantCultureIgnoreCase;
                id = entityDescription.Fields.FirstOrDefault(pair => comparer.Equals(pair.Key,DEFAULT_ID_FIELD));

                if (id.Key != null)
                {
                    entityDescription.Keys.Add(id);
                    entityDescription.Fields.Remove(id);
                }
            }

            Asserts.check(entityDescription.Keys.Count > 0, $"Type '{entityType.FullName}' has no property tagged with '{typeof(KeyAttribute).FullName}' neither a '{DEFAULT_ID_FIELD}' named property.");
            return entityDescription;
        }

        private class PrvEntityDescription : IEntityDescription
        {
            private IDictionary<string, PropertyInfo> fields;
            private IDictionary<string, PropertyInfo> keys;

            internal PrvEntityDescription()
            {
                this.fields = new Dictionary<string, PropertyInfo>();
                this.keys = new Dictionary<string, PropertyInfo>();
            }

            public string Name { get; internal set; }
            public Type Entity { get; internal set; }
            internal IDictionary<string, PropertyInfo> Fields { get; set; }
            internal IDictionary<string, PropertyInfo> Keys { get; set; }
            IReadOnlyDictionary<string, PropertyInfo> IEntityDescription.Fields => new ReadOnlyDictionary<string, PropertyInfo>(this.fields);
            IReadOnlyDictionary<string, PropertyInfo> IEntityDescription.Keys => new ReadOnlyDictionary<string, PropertyInfo>(this.keys);

        }

    }
}
