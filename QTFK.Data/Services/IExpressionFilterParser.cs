using QTFK.Models;
using System;
using System.Linq.Expressions;

namespace QTFK.Services
{
    public interface IExpressionFilterParser
    {
        IQueryFilter parse<T>(Expression<Func<T, bool>> filterExpression, IQueryFactory queryFactory, IEntityDescription entityDescription);
    }
}