using System;
using System.Collections.Generic;
using System.Linq;
using QTFK.Models;
using System.Reflection;
using QTFK.Attributes;
using QTFK.Extensions.TypeInfo;
using System.Data;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Models.QueryFilters;
using System.Linq.Expressions;

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
            internal PrvEntityDescription()
            {
                this.Fields = new Dictionary<string, PropertyInfo>();
                this.Keys = new Dictionary<string, PropertyInfo>();
            }

            public string Name { get; internal set; }
            internal Type Entity { get; set; }
            internal IDictionary<string, PropertyInfo> Fields { get; }
            internal IDictionary<string, PropertyInfo> Keys { get; }

            public bool UsesAutoId { get; }

            public object buildEntity(IDataRecord record)
            {
                object item;

                item = Activator.CreateInstance(this.Entity);

                foreach (var field in this.Keys)
                    prv_map(record, field, item);
                foreach (var field in this.Fields)
                    prv_map(record, field, item);

                return item;
            }

            public void setAutoId(object id, object item)
            {
                PropertyInfo fieldProperty;

                fieldProperty = this.Keys
                    .Single(pair => pair.Value.isAutonumeric())
                    .Value;

                fieldProperty.SetValue(item, id);
            }

            public IDBQueryUpdate buildUpdate(IQueryFactory queryFactory, object item)
            {
                IDBQueryUpdate query;
                IKeyFilter filter;
                bool autoKeyIsFilled, normalKeysAreFilled;

                autoKeyIsFilled = this.prv_autoKeyFieldIsFilled(item);
                normalKeysAreFilled = this.prv_nonAutoKeyFieldsAreFilled(item);

                Asserts.check(autoKeyIsFilled && normalKeysAreFilled, $"Parameter '{nameof(item)}' must have setted id fields in order to update repository.");

                query = queryFactory.newUpdate();
                query.Table = this.Name;

                foreach (var field in this.Fields)
                    prv_setQueryColumn(queryFactory, item, query, field);

                filter = queryFactory.buildFilter<IKeyFilter>();
                filter.setIdValuePairs(this.Keys, item);
                query.Filter = filter;

                return query;
            }

            public string getField(PropertyInfo property)
            {
                return this.prv_keysAndFields()
                    .Single(pair => pair.Value.Equals(property))
                    .Key
                    ;
            }

            public IEnumerable<KeyValuePair<string, object>> getKeyValues(object item)
            {
                Asserts.isSomething(item, $"Parameter '{nameof(item)}' cannot be null.");

                foreach (var pair in this.Keys)
                    yield return prv_getFilledKey(pair, item);
            }

            private static KeyValuePair<string, object> prv_getFilledKey(KeyValuePair<string, PropertyInfo> pair, object item)
            {
                PropertyInfo property;
                object value;
                KeyValuePair<string, object> itemKey;

                property = pair.Value;
                value = property.GetValue(item);
                prv_assertKeyIsFilled(property, value);
                itemKey = new KeyValuePair<string, object>(pair.Key, value);

                return itemKey;
            }

            private static void prv_assertKeyIsFilled(PropertyInfo field, object value)
            {
                Type type;
                object defaultValue;

                type = value.GetType();

                if (type.IsValueType)
                {
                    defaultValue = Activator.CreateInstance(type);
                    Asserts.check(value.Equals(defaultValue) == false, $"Key Value property '{field.Name}' of '{field.DeclaringType.FullName}' cannot have default value.");
                }
                else
                {
                    Asserts.isSomething(value, $"Key property '{field.Name}' of '{field.DeclaringType.FullName}' cannot be null.");
                }
            }

            private IEnumerable<KeyValuePair<string, PropertyInfo>> prv_keysAndFields()
            {
                foreach (var pair in this.Keys)
                    yield return pair;
                foreach (var pair in this.Fields)
                    yield return pair;
            }

            private static void prv_setQueryColumn(object item, IDBQueryWriteColumns query, KeyValuePair<string, PropertyInfo> field)
            {
                object fieldValue;

                fieldValue = field.Value.GetValue(item);
                query.SetColumn(field.Key, fieldValue, $"@{field.Key}");
            }

            private bool prv_nonAutoKeyFieldsAreFilled(object item)
            {
                throw new NotImplementedException();
            }

            private bool prv_autoKeyFieldIsFilled(object item)
            {
                throw new NotImplementedException();
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

        }

    }
}
