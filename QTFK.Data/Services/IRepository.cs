using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QTFK.Services
{
    public interface IRepository<T> where T : new()
    {
        IDBIO DB { get; set; }
        IQueryFactory QueryFactory { get; set; }

        IEnumerable<T> Get();
        RepositoryOperationResult Set(T item);
        RepositoryOperationResult Delete(T item);
    }
}