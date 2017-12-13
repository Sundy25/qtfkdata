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

            prv_assertEngine();
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
            
            prv_assertEngine();
            selectQuery = this.entityQueryFactory.newSelect();
            items = this.DB.Get<T>(selectQuery);

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

        private void prv_assertEngine()
        {
            Asserts.isSomething(DB, $"Property '{nameof(DB)}' not established");
            Asserts.isSomething(QueryFactory, $"Property '{nameof(QueryFactory)}' not established");
            Asserts.check(DB.getDBEngine() == QueryFactory.getDBEngine(), $"Database engine missmatch for '{DB.GetType().FullName}' and '{QueryFactory.GetType().FullName}'.");

            if (this.entityQueryFactory == null)
                this.entityQueryFactory = new EntityQueryFactory()
                {
                    EntityDescription = this.entityDescription,
                    QueryFactory = this.QueryFactory,
                    Prefix = this.QueryFactory.Prefix,
                };
        }

    }
}