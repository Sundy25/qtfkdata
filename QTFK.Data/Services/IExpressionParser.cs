using QTFK.Models;
using System;
using System.Linq.Expressions;

namespace QTFK.Services
{
    public interface IExpressionParser<T>
    {
        IQueryFactory QueryFactory { get; set; }

        IQueryFilter parse(Expression<Func<T, bool>> filterExpression);
    }
}