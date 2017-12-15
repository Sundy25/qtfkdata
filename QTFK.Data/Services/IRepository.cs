using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QTFK.Services
{
    public interface IRepository<T> where T : new()
    {
        IEnumerable<T> get(Expression<Func<T,bool>> filterExpression);
        void add(T item);
        void update(T item);
        void delete(T item);

        IQueryFactory QueryFactory { get; set; }
        IDBIO DB { get; set; }
    }
}