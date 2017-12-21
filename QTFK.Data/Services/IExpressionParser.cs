using QTFK.Models;
using System;
using System.Linq.Expressions;

namespace QTFK.Services
{
    public interface IExpressionParser<T>
    {
        IQueryFilter parse(Expression<Func<T, bool>> filterExpression);
    }
}