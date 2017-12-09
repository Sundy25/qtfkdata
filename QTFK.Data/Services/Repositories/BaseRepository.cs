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

namespace QTFK.Services.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : new()
    {
        private readonly IEnumerable<IMethodParser> methodParsers;
        private IEntityQueryFactory entityQueryFactory;

        public BaseRepository(
            IEnumerable<IMethodParser> methodParsers
            )
        {
            this.methodParsers = methodParsers;
        }

        public IDBIO DB
        {
            get
            {
                Asserts.isSomething(this.entityQueryFactory, $"'{nameof(this.entityQueryFactory)}' not established!");
                return this.entityQueryFactory.DB;
            }
            set
            {
                Asserts.isSomething(value, "New DB cannot be null!");
                prv_initEntityQueryFactory();
                this.entityQueryFactory.DB = value;
            }
        }

        private void prv_initEntityQueryFactory()
        {
            if (this.entityQueryFactory == null)
                this.entityQueryFactory = new EntityQueryFactory();
        }

        public IQueryFactory QueryFactory
        {
            get
            {
                Asserts.isSomething(this.entityQueryFactory, $"'{nameof(this.entityQueryFactory)}' not established!");
                return this.entityQueryFactory.QueryFactory;
            }
            set
            {
                Asserts.isSomething(value, "New QueryFactory cannot be null!");
                prv_initEntityQueryFactory();
                this.entityQueryFactory.QueryFactory = value;
            }
        }

        public RepositoryOperationResult Delete(T item)
        {
            RepositoryOperationResult operationResult;
            int result;
            IDBQueryDelete q;
            IByParamEqualsFilter filter;
            string param;

            q = this.entityQueryFactory.newDelete();
            filter = this.entityQueryFactory.buildFilter<IByParamEqualsFilter>();

            filter.Field = this.entityQueryFactory.EntityDescription.Id;
            param = $"@{this.entityQueryFactory.EntityDescription.Id}";
            filter.Parameter = param;
            q.Parameters[param] = this.entityQueryFactory.EntityDescription.PropertyId.GetValue(item).ToString();
            q.Filter = filter;

            result = this.entityQueryFactory.DB.Set(q);
            operationResult = result == 1
                ? RepositoryOperationResult.Deleted
                : RepositoryOperationResult.NonDeleted
                ;

            return operationResult;
        }

        public IEnumerable<T> Get()
        {
            return this.entityQueryFactory.Select<T>();
        }

        public RepositoryOperationResult Set(T item)
        {
            RepositoryOperationResult operationResult;
            object id, newId;
            IDBQueryInsert insertQuery;
            IDBQueryUpdate updateQuery;
            IByParamEqualsFilter filter;
            IDictionary<string, object> values;
            int affectedRows;

            id = null;
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

                this.entityQueryFactory.EntityDescription.PropertyId.SetValue(item, newId);
                operationResult = RepositoryOperationResult.Added;
            }
            else
            {
                updateQuery = this.entityQueryFactory.newUpdate();
                foreach (var f in values)
                    updateQuery.Parameters[$"@{f.Key}"] = f.Value;

                filter = this.entityQueryFactory.buildFilter<IByParamEqualsFilter>();

                filter.Field = this.entityQueryFactory.EntityDescription.Id;
                string param = $"@{this.entityQueryFactory.EntityDescription.Id}";
                filter.Parameter = param;
                updateQuery.Parameters[param] = this.entityQueryFactory
                    .EntityDescription
                    .PropertyId
                    .GetValue(item)
                    .ToString();

                updateQuery.Filter = filter;
                affectedRows = this.entityQueryFactory.DB.Set(updateQuery);
                operationResult = affectedRows == 1
                    ? RepositoryOperationResult.Updated
                    : RepositoryOperationResult.NonUpdated
                    ;
            }

            return operationResult;
        }

        protected IQueryFilter GetFilter(MethodBase method)
        {
            return this.methodParsers
                .Select(p => p.Parse(method, this.entityQueryFactory.EntityDescription, this.entityQueryFactory))
                .SingleOrDefault()
                ;
        }

        protected IQueryFilter GetFilter<T1>(MethodBase method) where T1 : struct
        {
            return this.methodParsers
                .Select(p => p.Parse<T1>(method, this.entityQueryFactory.EntityDescription, this.entityQueryFactory))
                .SingleOrDefault()
                ;
        }
    }
}