using System.Collections.Generic;
using System.Reflection;
using QTFK.Models;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Attributes;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.Objects.DictionaryConverter;
using QTFK.Services.Factories;
using QTFK.Extensions.DBIO.EngineAttribute;
using System.Data;
using System.Linq;
using QTFK.Extensions.TypeInfo;
using QTFK.Extensions.EntityDescription;

namespace QTFK.Services.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : new()
    {
        private readonly IEntityDescription entityDescription;
        private IEntityQueryFactory entityQueryFactory;

        //private readonly IEnumerable<IMethodParser> methodParsers;

        public BaseRepository(
            //IEnumerable<IMethodParser> methodParsers
            )
        {
            //this.methodParsers = methodParsers;
        }

        public BaseRepository(IEntityDescriber entityDescriber)
        {
            Asserts.isSomething(entityDescriber, $"Parameter '{nameof(entityDescriber)}' cannot be null.");

            this.entityDescription = entityDescriber.describe(typeof(T));
        }

        public IDBIO DB { get; set; }
        public IQueryFactory QueryFactory { get; set; }

        public RepositoryOperationResult Delete(T item)
        {
            RepositoryOperationResult operationResult;
            int result;
            IDBQueryDelete deleteQuery;

            prv_prepareEngine();
            deleteQuery = this.entityQueryFactory.newDelete();
            prv_setFilter(deleteQuery, item);
            result = this.DB.Set(deleteQuery);
            operationResult = result == 1
                ? RepositoryOperationResult.Deleted
                : RepositoryOperationResult.NonDeleted
                ;

            return operationResult;
        }

        public IEnumerable<T> Get()
        {
            IEnumerable<T> items;
            IDBQuerySelect selectQuery;

            prv_prepareEngine();
            selectQuery = this.entityQueryFactory.newSelect();
            items = this.DB.Get<T>(selectQuery, prv_map);

            return items;
        }

        public RepositoryOperationResult Set(T item)
        {
            RepositoryOperationResult operationResult;
            object id, newId;
            IDBQueryInsert insertQuery;
            IDBQueryUpdate updateQuery;
            IDictionary<string, object> values;
            int affectedRows;

            prv_prepareEngine();

            id = this.entityQueryFactory
                    .EntityDescription
                    .getId(item);

            values = item.toDictionary(p => p.GetCustomAttribute<KeyAttribute>() == null);

            if (id == null)
            {
                insertQuery = this.entityQueryFactory.newInsert();
                foreach (var f in values)
                    insertQuery.Parameters[$"@{f.Key}"] = f.Value;

                newId = null;
                this.DB.Set(cmd =>
                {
                    int affected = cmd
                        .SetCommandText(insertQuery.Compile())
                        .AddParameters(insertQuery.Parameters)
                        .ExecuteNonQuery()
                        ;

                    if (affected <= 1)
                        throw new RepositoryInsertException(insertQuery);

                    newId = this.DB.GetLastID(cmd);
                    return affected;
                });

                this.entityQueryFactory.EntityDescription.setId(item, newId);
                operationResult = RepositoryOperationResult.Added;
            }
            else
            {
                updateQuery = this.entityQueryFactory.newUpdate();
                foreach (var f in values)
                    updateQuery.Parameters[$"@{f.Key}"] = f.Value;

                prv_setFilter(updateQuery, item);
                affectedRows = this.DB.Set(updateQuery);
                operationResult = affectedRows == 1
                    ? RepositoryOperationResult.Updated
                    : RepositoryOperationResult.NonUpdated
                    ;
            }

            return operationResult;
        }

        //protected IQueryFilter GetFilter(MethodBase method)
        //{
        //    return this.methodParsers
        //        .Select(p => p.Parse(method, this.entityQueryFactory.EntityDescription, this.entityQueryFactory))
        //        .SingleOrDefault()
        //        ;
        //}

        //protected IQueryFilter GetFilter<T1>(MethodBase method) where T1 : struct
        //{
        //    return this.methodParsers
        //        .Select(p => p.Parse<T1>(method, this.entityQueryFactory.EntityDescription, this.entityQueryFactory))
        //        .SingleOrDefault()
        //        ;
        //}

        private void prv_setFilter(IDBQueryFilterable query, T item)
        {
            IByParamEqualsFilter filter;
            string param;

            filter = this.entityQueryFactory.buildFilter<IByParamEqualsFilter>();
            filter.Field = this.entityDescription.Keys;
            param = $"@{this.entityQueryFactory.EntityDescription.Keys}";
            filter.Parameter = param;
            query.Parameters[param] = this.entityQueryFactory
                .EntityDescription
                .getId(item)
                .ToString();

            query.Filter = filter;
        }

        private void prv_prepareEngine()
        {
            Asserts.isSomething(DB, $"Property '{nameof(DB)}' not established");
            Asserts.isSomething(QueryFactory, $"Property '{nameof(QueryFactory)}' not established");
            Asserts.check(DB.getDBEngine() == QueryFactory.getDBEngine(), $"Database engine missmatch for '{DB.GetType().FullName}' and '{QueryFactory.GetType().FullName}'.");

            if (this.entityQueryFactory == null)
                this.entityQueryFactory = new EntityQueryFactory()
                {
                    EntityDescription = this.entityDescription,
                };
            this.entityQueryFactory.QueryFactory = this.QueryFactory;
            this.entityQueryFactory.Prefix = this.QueryFactory.Prefix;
        }

        private T prv_map(IDataRecord record)
        {
            T item = new T();

            foreach (var field in this.entityDescription.getKeysAndFields())
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

            return item;
        }

    }
}