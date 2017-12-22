using QTFK.Models;
using System;
using System.Linq.Expressions;

namespace QTFK.Services
{
    public interface IExpressionParser
    {
        IQueryFilter parse<T>(Expression<Func<T, bool>> filterExpression);
    }
}