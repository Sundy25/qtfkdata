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
using System.Data;

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
            if (entityDescription.Keys.Count == 0)
            {
                KeyValuePair<string, PropertyInfo> id;
                StringComparer comparer;

                comparer = StringComparer.InvariantCultureIgnoreCase;
                id = entityDescription.Fields.FirstOrDefault(pair => comparer.Equals(pair.Key, DEFAULT_ID_FIELD));

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

            internal string Name { get; set; }
            internal Type Entity { get; set; }
            internal IDictionary<string, PropertyInfo> Fields { get; set; }
            internal IDictionary<string, PropertyInfo> Keys { get; set; }

            public bool UsesAutoId { get; }

            public object build(IDataRecord record)
            {
                object item;

                item = Activator.CreateInstance(this.Entity);

                foreach (var field in this.Keys)
                    prv_map(record, field, item);
                foreach (var field in this.Fields)
                    prv_map(record, field, item);

                return item;
            }

            private static void prv_map(IDataRecord record, KeyValuePair<string, PropertyInfo> field, object item)
            {
                int fieldIndex;
                object value;
                string fieldName;
                PropertyInfo fieldProperty;

                fieldName = field.Key;
                fieldProperty = field.Value;
                fieldIndex = record.GetOrdinal(fieldName);
                Asserts.check(fieldIndex >= 0, $"Returned field index below zero '{fieldIndex}' from '{record.GetType().FullName}'.");
                value = record[fieldIndex];
                fieldProperty.SetValue(item, value);
            }

            public bool hasId(object item)
            {
                throw new NotImplementedException();
            }

            public void prepare(object item, IDBQueryDelete deleteQuery)
            {
                throw new NotImplementedException();
            }

            public void prepare(object item, IDBQueryInsert deleteQuery)
            {
                throw new NotImplementedException();
            }

            public void prepare(object item, IDBQueryUpdate deleteQuery)
            {
                throw new NotImplementedException();
            }
        }

    }
}
