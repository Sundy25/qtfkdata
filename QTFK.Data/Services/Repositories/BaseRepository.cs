using System.Collections.Generic;
using System.Reflection;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Attributes;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.Objects.DictionaryConverter;
using System;
using QTFK.Services.Factories;
using QTFK.Extensions.EntityDescriptions;

namespace QTFK.Services.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : new()
    {
        //private readonly IEnumerable<IMethodParser> methodParsers;
        private IEntityQueryFactory entityQueryFactory;

        public BaseRepository(
            //IEnumerable<IMethodParser> methodParsers
            )
        {
            //this.methodParsers = methodParsers;
        }

        public BaseRepository(IDBIO db, IQueryFactory queryFactory)
        {
            prv_setDB(db, queryFactory);
        }

        public RepositoryOperationResult Delete(T item)
        {
            RepositoryOperationResult operationResult;
            int result;
            IDBQueryDelete deleteQuery;

            deleteQuery = this.entityQueryFactory.newDelete();
            prv_setFilter(deleteQuery, item);
            result = this.entityQueryFactory.DB.Set(deleteQuery);
            operationResult = result == 1
                ? RepositoryOperationResult.Deleted
                : RepositoryOperationResult.NonDeleted
                ;

            return operationResult;
        }

        public IEnumerable<T> Get()
        {
            Asserts.isSomething(this.entityQueryFactory, $"Repository not initialized. Call '{nameof(setDB)}' first.");

            return this.entityQueryFactory.Select<T>();
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
                this.entityQueryFactory.DB.Set(cmd =>
                {
                    int affected = cmd
                        .SetCommandText(insertQuery.Compile())
                        .AddParameters(insertQuery.Parameters)
                        .ExecuteNonQuery()
                        ;

                    if (affected <= 1)
                        throw new RepositoryInsertException(insertQuery);

                    newId = this.entityQueryFactory.DB.GetLastID(cmd);
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
                affectedRows = this.entityQueryFactory.DB.Set(updateQuery);
                operationResult = affectedRows == 1
                    ? RepositoryOperationResult.Updated
                    : RepositoryOperationResult.NonUpdated
                    ;
            }

            return operationResult;
        }

        public void setDB(IDBIO db, IQueryFactory queryFactory)
        {
            prv_setDB(db, queryFactory);
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

        private void prv_setDB(IDBIO db, IQueryFactory queryFactory)
        {
            Asserts.isSomething(db, $"Parameter {nameof(db)} cannot be null!");
            Asserts.isSomething(queryFactory, $"Parameter {nameof(queryFactory)} cannot be null!");

            if (this.entityQueryFactory == null)
            {
                this.entityQueryFactory = new EntityQueryFactory();
                this.entityQueryFactory.Entity = typeof(T);
            }

            this.entityQueryFactory.DB = db;
            this.entityQueryFactory.QueryFactory = queryFactory;
        }

        private void prv_setFilter(IDBQueryFilterable query, T item)
        {
            IByParamEqualsFilter filter;
            string param;

            filter = this.entityQueryFactory.buildFilter<IByParamEqualsFilter>();
            filter.Field = this.entityQueryFactory.EntityDescription.Id;
            param = $"@{this.entityQueryFactory.EntityDescription.Id}";
            filter.Parameter = param;
            query.Parameters[param] = this.entityQueryFactory
                .EntityDescription
                .getId(item)
                .ToString();

            query.Filter = filter;
        }
    }
}