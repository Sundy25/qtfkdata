using System;
using System.Collections.Generic;
using System.Reflection;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Extensions.Objects.DictionaryConverter;
using QTFK.Attributes;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.DBCommand;

namespace QTFK.Services.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : new()
    {
        protected readonly IQueryFactory<T> _queryFactory;
        private readonly IEnumerable<IMethodParser> _methodParsers;

        public BaseRepository(
            IQueryFactory<T> queryFactory
            , IEnumerable<IMethodParser> methodParsers
            )
        {
            _queryFactory = queryFactory;
            _methodParsers = methodParsers;
        }

        public RepositoryOperationResult Delete(T item)
        {
            var q = _queryFactory.NewDelete();

            var filter = _queryFactory
                .Build<IByParamEqualsFilterFactory>()
                .NewByParamEqualsFilter()
                ;

            filter.Field = _queryFactory.EntityDescription.Id;
            string param = $"@{_queryFactory.EntityDescription.Id}";
            filter.Parameter = param;
            q.Parameters[param] = _queryFactory.EntityDescription.PropertyId.GetValue(item).ToString();
            q.Filter = filter;

            int result = _queryFactory.DB.Set(q);
            return result == 1
                ? RepositoryOperationResult.Deleted
                : RepositoryOperationResult.NonDeleted
                ;
        }

        public IEnumerable<T> Get()
        {
            return _queryFactory.Select();
        }

        public RepositoryOperationResult Set(T item)
        {
            object id = null;
            var values = item.ToDictionary(p => p.GetCustomAttribute<KeyAttribute>() == null);

            if(id == null)
            {
                var q = _queryFactory.NewInsert();
                foreach (var f in values)
                    q.Parameters[$"@{f.Key}"] = f.Value;

                object newId = null;
                _queryFactory.DB.Set(cmd =>
                {
                    int affected = cmd
                        .SetCommandText(q.Compile())
                        .AddParameters(q.Parameters)
                        .ExecuteNonQuery()
                        ;
                    if (affected <= 1)
                        throw new RepositoryInsertException(q);

                    newId = _queryFactory.DB.GetLastID(cmd);
                    return affected;
                });

                _queryFactory.EntityDescription.PropertyId.SetValue(item, newId);
                return RepositoryOperationResult.Added;
            }
            else
            {
                var q = _queryFactory.NewUpdate();
                foreach (var f in values)
                    q.Parameters[$"@{f.Key}"] = f.Value;

                var filter = _queryFactory
                    .Build<IByParamEqualsFilterFactory>()
                    .NewByParamEqualsFilter()
                    ;

                filter.Field = _queryFactory.EntityDescription.Id;
                string param = $"@{_queryFactory.EntityDescription.Id}";
                filter.Parameter = param;
                q.Parameters[param] = _queryFactory.EntityDescription.PropertyId.GetValue(item).ToString();
                q.Filter = filter;

                int result = _queryFactory.DB.Set(q);

                return result == 1
                    ? RepositoryOperationResult.Updated
                    : RepositoryOperationResult.NonUpdated
                    ;
            }
        }

        protected IQueryFilter GetFilter(MethodBase method)
        {
            return _methodParsers
                .Select(p => p.Parse(method, _queryFactory.EntityDescription, _queryFactory))
                .SingleOrDefault()
                ;
        }

        protected IQueryFilter GetFilter<T1>(MethodBase method) where T1: struct
        {
            return _methodParsers
                .Select(p => p.Parse<T1>(method, _queryFactory.EntityDescription, _queryFactory))
                .SingleOrDefault()
                ;
        }
    }
}