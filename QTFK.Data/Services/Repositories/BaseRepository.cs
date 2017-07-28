using System;
using System.Collections.Generic;
using QTFK.Models;

namespace QTFK.Services.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : new()
    {
        protected readonly IQueryFactory<T> _queryFactory;

        public BaseRepository(IQueryFactory<T> queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public RepositoryOperationResult Delete(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get()
        {
            throw new NotImplementedException();
        }

        public RepositoryOperationResult Set(T item)
        {
            throw new NotImplementedException();
        }
    }
}