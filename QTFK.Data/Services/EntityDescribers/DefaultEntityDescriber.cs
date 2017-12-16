using System;
using System.Collections.Generic;
using System.Linq;
using QTFK.Models;
using System.Reflection;
using QTFK.Attributes;
using QTFK.Extensions.TypeInfo;
using System.Data;
using QTFK.Extensions.DBIO.DBQueries;

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

            public void setId(object id, object item)
            {
                throw new NotImplementedException();
            }

            public IDBQueryDelete buildDelete(IQueryFactory queryFactory, object item)
            {
                IDBQueryDelete query;

                query = queryFactory.newDelete();
                query.Table = this.Name;

                throw new NotImplementedException();
            }

            public IDBQueryInsert buildInsert(IQueryFactory queryFactory, object item)
            {
                IDBQueryInsert query;

                if (this.UsesAutoId)
                    Asserts.check(this.prv_hasId(item) == false, $"Because of type '{this.Entity.FullName}' has autonumeric Id, parameter '{nameof(item)}' must have no id in order to add to repository.");
                else
                    Asserts.check(this.prv_hasId(item) == true, $"Because of type '{this.Entity.FullName}' has no autonumeric Id, parameter '{nameof(item)}' must have setted id in order to add to repository.");

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

                Asserts.check(this.entityDescription.hasId(item), $"Parameter '{nameof(item)}' must have setted id in order to update repository.");

                query = queryFactory.newUpdate();
                query.Table = this.Name;

                foreach (var field in this.Fields)
                    prv_setQueryColumn(queryFactory, item, query, field);

                throw new NotImplementedException();
            }

            private static void prv_setQueryColumn(IQueryFactory queryFactory, object item, IDBQueryWriteColumns query, KeyValuePair<string, PropertyInfo> field)
            {
                object fieldValue;
                string fieldParameter;

                fieldParameter = queryFactory.buildParameter(field.Key);
                fieldValue = field.Value.GetValue(item);
                query.SetColumn(field.Key, fieldValue, fieldParameter);
            }

            private bool prv_hasId(object item)
            {
                throw new NotImplementedException();
            }

        }

    }
}
