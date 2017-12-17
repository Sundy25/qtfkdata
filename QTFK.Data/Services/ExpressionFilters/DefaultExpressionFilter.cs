using System;
using System.Linq.Expressions;
using QTFK.Models;

namespace QTFK.Services.ExpressionFilters
{
    public class DefaultExpressionFilter : IExpressionFilterParser
    {
        public IQueryFilter buildFilter<T>(IQueryFactory queryFactory, Expression<Func<T, bool>> filterExpression)
        {
            throw new NotImplementedException();
        }
    }
}