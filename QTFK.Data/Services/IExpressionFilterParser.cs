using QTFK.Models;
using System;
using System.Linq.Expressions;

namespace QTFK.Services
{
    public interface IExpressionFilterParser
    {
        IQueryFilter buildFilter<T>(IQueryFactory queryFactory, Expression<Func<T, bool>> filterExpression);
    }
}