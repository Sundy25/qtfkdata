using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QTFK.Services
{
    public interface IRepository<T> where T : new()
    {
        IQueryFactory<T> QueryFactory { get; }
        MetaDataInfo Entity { get; }
        //RepositoryOperationResult Set(T item);
        //IEnumerable<T> Get(Expression<Func<T, bool>> filter);
    }
}