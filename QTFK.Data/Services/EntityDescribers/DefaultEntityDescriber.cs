using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;
using System.Reflection;
using QTFK.Attributes;
using QTFK.Extensions.TypeInfo;

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
                    entityDescription.addKey(fieldName);
                else
                    entityDescription.addField(fieldName);
            }
            if(entityDescription.Keys.Count == 0)
            {
                string idKey;

                idKey = entityDescription.Fields.FirstOrDefault(prv_fieldIsId);

                if (idKey != null)
                {
                    entityDescription.addKey(idKey);
                    entityDescription.removeField(idKey);
                }
            }

            Asserts.check(entityDescription.Keys.Count > 0, $"Parameter '{nameof(entityType)}' has no property with '{typeof(KeyAttribute).FullName}' attribute neither a property with Id name.");
            return entityDescription;
        }

        private static bool prv_fieldIsId(string field)
        {
            return StringComparer
                .InvariantCultureIgnoreCase
                .Compare(field, DEFAULT_ID_FIELD) == 0;
        }

        private class PrvEntityDescription : IEntityDescription
        {
            private List<string> fields;
            private List<string> keys;

            internal PrvEntityDescription()
            {
                this.fields = new List<string>();
                this.keys = new List<string>();
            }

            public string Name { get; internal set; }
            public Type Entity { get; internal set; }
            public IReadOnlyList<string> Fields => this.fields.AsReadOnly();
            public IReadOnlyList<string> Keys => this.keys.AsReadOnly();

            internal void addField(string field)
            {
                this.fields.Add(field);
            }

            internal void addKey(string field)
            {
                this.keys.Add(field);
            }

            internal void removeField(string idKey)
            {
                this.fields.Remove(idKey);
            }
        }

    }
}
