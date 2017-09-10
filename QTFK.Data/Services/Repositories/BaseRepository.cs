using System;
using System.Collections.Generic;
using System.Reflection;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions.DBIO.QueryFactory;

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
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get()
        {
            return _queryFactory
                .Select(q => { })
                ;
        }

        public RepositoryOperationResult Set(T item)
        {

            throw new NotImplementedException();
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