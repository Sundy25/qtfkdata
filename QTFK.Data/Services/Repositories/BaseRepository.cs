using System.Collections.Generic;
using System.Reflection;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Attributes;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.Objects.DictionaryConverter;

namespace QTFK.Services.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : new()
    {
        protected readonly IQueryFactory<T> queryFactory;
        private readonly IEnumerable<IMethodParser> methodParsers;

        public BaseRepository(
            IQueryFactory<T> queryFactory
            , IEnumerable<IMethodParser> methodParsers
            )
        {
            this.queryFactory = queryFactory;
            this.methodParsers = methodParsers;
        }

        public RepositoryOperationResult Delete(T item)
        {
            var q = this.queryFactory.NewDelete();

            var filter = this.queryFactory
                .Build<IByParamEqualsFilterFactory>()
                .NewByParamEqualsFilter()
                ;

            filter.Field = this.queryFactory.EntityDescription.Id;
            string param = $"@{this.queryFactory.EntityDescription.Id}";
            filter.Parameter = param;
            q.Parameters[param] = this.queryFactory.EntityDescription.PropertyId.GetValue(item).ToString();
            q.Filter = filter;

            int result = this.queryFactory.DB.Set(q);
            return result == 1
                ? RepositoryOperationResult.Deleted
                : RepositoryOperationResult.NonDeleted
                ;
        }

        public IEnumerable<T> Get()
        {
            return this.queryFactory.Select();
        }

        public RepositoryOperationResult Set(T item)
        {
            object id = null;
            var values = item.toDictionary(p => p.GetCustomAttribute<KeyAttribute>() == null);

            if(id == null)
            {
                var q = this.queryFactory.NewInsert();
                foreach (var f in values)
                    q.Parameters[$"@{f.Key}"] = f.Value;

                object newId = null;
                this.queryFactory.DB.Set(cmd =>
                {
                    int affected = cmd
                        .SetCommandText(q.Compile())
                        .AddParameters(q.Parameters)
                        .ExecuteNonQuery()
                        ;
                    if (affected <= 1)
                        throw new RepositoryInsertException(q);

                    newId = this.queryFactory.DB.GetLastID(cmd);
                    return affected;
                });

                this.queryFactory.EntityDescription.PropertyId.SetValue(item, newId);
                return RepositoryOperationResult.Added;
            }
            else
            {
                var q = this.queryFactory.NewUpdate();
                foreach (var f in values)
                    q.Parameters[$"@{f.Key}"] = f.Value;

                var filter = this.queryFactory
                    .Build<IByParamEqualsFilterFactory>()
                    .NewByParamEqualsFilter()
                    ;

                filter.Field = this.queryFactory.EntityDescription.Id;
                string param = $"@{this.queryFactory.EntityDescription.Id}";
                filter.Parameter = param;
                q.Parameters[param] = this.queryFactory.EntityDescription.PropertyId.GetValue(item).ToString();
                q.Filter = filter;

                int result = this.queryFactory.DB.Set(q);

                return result == 1
                    ? RepositoryOperationResult.Updated
                    : RepositoryOperationResult.NonUpdated
                    ;
            }
        }

        protected IQueryFilter GetFilter(MethodBase method)
        {
            return this.methodParsers
                .Select(p => p.Parse(method, this.queryFactory.EntityDescription, this.queryFactory))
                .SingleOrDefault()
                ;
        }

        protected IQueryFilter GetFilter<T1>(MethodBase method) where T1: struct
        {
            return this.methodParsers
                .Select(p => p.Parse<T1>(method, this.queryFactory.EntityDescription, this.queryFactory))
                .SingleOrDefault()
                ;
        }
    }
}