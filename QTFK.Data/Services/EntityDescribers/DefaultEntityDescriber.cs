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

            internal string Name { get; set; }
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

            public IDBQueryDelete buildDelete(IQueryFactory queryFactory, object item)
            {
                IDBQueryDelete query;
                IKeyFilter filter;
                bool autoKeyIsFilled, normalKeysAreFilled;
                
                autoKeyIsFilled = this.prv_autoKeyFieldIsFilled(item);
                normalKeysAreFilled = this.prv_nonAutoKeyFieldsAreFilled(item);

                Asserts.check(autoKeyIsFilled && normalKeysAreFilled, $"Parameter '{nameof(item)}' must have setted id fields in order to update repository.");

                query = queryFactory.newDelete();
                query.Table = this.Name;
                filter = queryFactory.buildFilter<IKeyFilter>();
                filter.setIdValuePairs(this.Keys, item);
                query.Filter = filter;

                return query;
            }

            public IDBQueryInsert buildInsert(IQueryFactory queryFactory, object item)
            {
                IDBQueryInsert query;
                bool autoKeyIsFilled, normalKeysAreFilled;

                autoKeyIsFilled = this.prv_autoKeyFieldIsFilled(item);
                normalKeysAreFilled = this.prv_nonAutoKeyFieldsAreFilled(item);

                Asserts.check(normalKeysAreFilled, $"Item of type '{this.Entity.FullName}' must have setted id fields in order to add to repository.");
                if (this.UsesAutoId)
                    Asserts.check(autoKeyIsFilled == false, $"Item of type '{this.Entity.FullName}' must have autonumeric id field empty in order to add to repository.");

                query = queryFactory.newInsert();
                query.Table = this.Name;

                foreach (var field in this.Fields)
                    prv_setQueryColumn(queryFactory, item, query, field);
                foreach (var id in this.Keys.Where(id => id.Value.isAutonumeric() == false))
                    prv_setQueryColumn(queryFactory, item, query, id);

                return query;
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

            private IEnumerable<KeyValuePair<string, PropertyInfo>> prv_keysAndFields()
            {
                foreach (var pair in this.Keys)
                    yield return pair;
                foreach (var pair in this.Fields)
                    yield return pair;
            }

            //private static void prv_setQueryColumn(IQueryFactory queryFactory, object item, IDBQueryWriteColumns query, KeyValuePair<string, PropertyInfo> field)
            //{
            //    object fieldValue;
            //    string fieldParameter;

            //    fieldParameter = queryFactory.buildParameter(field.Key);
            //    fieldValue = field.Value.GetValue(item);
            //    query.SetColumn(field.Key, fieldValue, fieldParameter);
            //}
            private static void prv_setQueryColumn(IQueryFactory queryFactory, object item, IDBQueryWriteColumns query, KeyValuePair<string, PropertyInfo> field)
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
