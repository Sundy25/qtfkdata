using System;
using System.Linq.Expressions;
using QTFK.Models;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Models.QueryFilters;

namespace QTFK.Services.ExpressionFilterParsers
{
    public class DefaultExpressionFilterParser : IExpressionFilterParser
    {
        

        public IQueryFilter buildFilter<T>(IQueryFactory queryFactory, Expression<Func<T, bool>> filterExpression)
        {
            IQueryFilter queryFilter;

            Asserts.isSomething(queryFactory, $"Parameter '{nameof(queryFactory)}' cannot be null.");
            Asserts.isSomething(filterExpression, $"Parameter '{nameof(filterExpression)}' cannot be null.");

            queryFilter = prv_parseExpr(filterExpression.Body, queryFactory);

            return queryFilter;
        }

        private IQueryFilter prv_parseExpr(Expression expression, IQueryFactory queryFactory)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.OrElse:
                    return prv_parseOr((BinaryExpression)expression, queryFactory);
                case ExpressionType.AndAlso:
                    return prv_parseAnd((BinaryExpression)expression, queryFactory);
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return prv_parseComparer(expression, queryFactory);
                default:
                    throw new NotSupportedException($"Parse error or unsupported expression of node type {expression.NodeType}: '{expression.ToString()}'");
            }
        }

        private IQueryFilter prv_parseOr(BinaryExpression expression, IQueryFactory queryFactory)
        {
            IOrQueryFilter orFilter;

            orFilter = queryFactory.buildFilter<IOrQueryFilter>();
            orFilter.Left = prv_parseExpr(expression.Left, queryFactory);
            orFilter.Right = prv_parseExpr(expression.Right, queryFactory);

            return orFilter;
        }

        private IQueryFilter prv_parseAnd(BinaryExpression expression, IQueryFactory queryFactory)
        {
            IAndQueryFilter orFilter;

            orFilter = queryFactory.buildFilter<IAndQueryFilter>();
            orFilter.Left = prv_parseExpr(expression.Left, queryFactory);
            orFilter.Right = prv_parseExpr(expression.Right, queryFactory);

            return orFilter;
        }

        private IQueryFilter prv_parseComparer(Expression expression, IQueryFactory queryFactory)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.GreaterThan:
                    return prv_parseConcreteComparer<IGreaterThanQueryFilter>((BinaryExpression)expression, queryFactory);
                case ExpressionType.GreaterThanOrEqual:
                    return prv_parseConcreteComparer<IGreaterThanOrEqualQueryFilter>((BinaryExpression)expression, queryFactory);
                case ExpressionType.LessThan:
                    return prv_parseConcreteComparer<ILessThanQueryFilter>((BinaryExpression)expression, queryFactory);
                case ExpressionType.LessThanOrEqual:
                    return prv_parseConcreteComparer<ILessThanOrEqualQueryFilter>((BinaryExpression)expression, queryFactory);
                case ExpressionType.Equal:
                    return prv_parseConcreteComparer<IEqualQueryFilter>((BinaryExpression)expression, queryFactory);
                case ExpressionType.NotEqual:
                    return prv_parseConcreteComparer<INotEqualQueryFilter>((BinaryExpression)expression, queryFactory);
                default:
                    throw new NotSupportedException($"Parse error or unsupported expression of node type {expression.NodeType}: '{expression.ToString()}'");
            }
        }

        private IQueryFilter prv_parseConcreteComparer<TFilter>(BinaryExpression expression, IQueryFactory queryFactory) where TFilter : IComparerQueryFilter
        {
            TFilter filter;

            filter = queryFactory.buildFilter<TFilter>();
            throw new NotImplementedException();
        }
    }
}