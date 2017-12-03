using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QTFK.Services
{
    public interface IRepository
    {
    }

    public interface IRepository<T> 
        : IRepository 
        where T : new()
    {
        IEnumerable<T> Get();
        RepositoryOperationResult Set(T item);
        RepositoryOperationResult Delete(T item);
    }
}