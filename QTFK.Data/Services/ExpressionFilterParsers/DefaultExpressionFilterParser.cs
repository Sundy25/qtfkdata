using System;
using System.Linq.Expressions;
using QTFK.Models;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Models.QueryFilters;
using System.Reflection;

namespace QTFK.Services.ExpressionFilterParsers
{
    public class DefaultExpressionFilterParser : IExpressionFilterParser
    {
        private class PrvArgs
        {
            public IQueryFactory QueryFactory { get; set; }
            public IEntityDescription EntityDescription { get; set; }
        }

        public IQueryFilter parse<T>(Expression<Func<T, bool>> filterExpression, IQueryFactory queryFactory, IEntityDescription entityDescription)
        {
            IQueryFilter queryFilter;
            PrvArgs builders;

            Asserts.isSomething(filterExpression, $"Parameter '{nameof(filterExpression)}' cannot be null.");
            Asserts.isSomething(queryFactory, $"Parameter '{nameof(queryFactory)}' cannot be null.");
            Asserts.isSomething(entityDescription, $"Parameter '{nameof(entityDescription)}' cannot be null.");

            builders = new PrvArgs
            {
                EntityDescription = entityDescription,
                QueryFactory = queryFactory,
            };

            queryFilter = prv_parseExpr(filterExpression.Body, builders);

            return queryFilter;
        }

        private IQueryFilter prv_parseExpr(Expression expression, PrvArgs builders)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.OrElse:
                    return prv_parseOr((BinaryExpression)expression, builders);
                case ExpressionType.AndAlso:
                    return prv_parseAnd((BinaryExpression)expression, builders);
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return prv_parseComparer(expression, builders);
                default:
                    throw new NotSupportedException($"Parse error or unsupported expression of node type {expression.NodeType}: '{expression.ToString()}'");
            }
        }

        private IQueryFilter prv_parseOr(BinaryExpression expression, PrvArgs builders)
        {
            IOrQueryFilter orFilter;

            orFilter = builders.QueryFactory.buildFilter<IOrQueryFilter>();
            orFilter.Left = prv_parseExpr(expression.Left, builders);
            orFilter.Right = prv_parseExpr(expression.Right, builders);

            return orFilter;
        }

        private IQueryFilter prv_parseAnd(BinaryExpression expression, PrvArgs builders)
        {
            IAndQueryFilter orFilter;

            orFilter = builders.QueryFactory.buildFilter<IAndQueryFilter>();
            orFilter.Left = prv_parseExpr(expression.Left, builders);
            orFilter.Right = prv_parseExpr(expression.Right, builders);

            return orFilter;
        }

        private IQueryFilter prv_parseComparer(Expression expression, PrvArgs builders)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.GreaterThan:
                    return prv_parseConcreteComparer<IGreaterThanQueryFilter>((BinaryExpression)expression, builders);
                case ExpressionType.GreaterThanOrEqual:
                    return prv_parseConcreteComparer<IGreaterThanOrEqualQueryFilter>((BinaryExpression)expression, builders);
                case ExpressionType.LessThan:
                    return prv_parseConcreteComparer<ILessThanQueryFilter>((BinaryExpression)expression, builders);
                case ExpressionType.LessThanOrEqual:
                    return prv_parseConcreteComparer<ILessThanOrEqualQueryFilter>((BinaryExpression)expression, builders);
                case ExpressionType.Equal:
                    return prv_parseConcreteComparer<IEqualQueryFilter>((BinaryExpression)expression, builders);
                case ExpressionType.NotEqual:
                    return prv_parseConcreteComparer<INotEqualQueryFilter>((BinaryExpression)expression, builders);
                default:
                    throw new NotSupportedException($"Parse error or unsupported expression of node type {expression.NodeType}: '{expression.ToString()}'");
            }
        }

        private IQueryFilter prv_parseConcreteComparer<TFilter>(BinaryExpression expression, PrvArgs builders) where TFilter : IComparerQueryFilter
        {
            TFilter filter;
            MemberExpression leftExpression, rightExpression;
            ConstantExpression rightConstantExpression;
            string fieldName, propertyName;
            object value;
            PropertyInfo property;

            Asserts.check(expression.Left.NodeType == ExpressionType.MemberAccess, $"Un expected {expression.Left.NodeType} node type: {expression.Left.ToString()}");
            Asserts.check(expression.Right.NodeType == ExpressionType.MemberAccess, $"Un expected {expression.Right.NodeType} node type: {expression.Right.ToString()}");

            leftExpression = (MemberExpression)expression.Left;
            Asserts.check(leftExpression.Member is PropertyInfo, $"Expected property for left member of expression '{expression.ToString()}'");
            property = (PropertyInfo)leftExpression.Member;
            propertyName = leftExpression.Member.Name;
            //fieldName = builders.EntityDescription.getField(propertyName);

            rightExpression = (MemberExpression)expression.Right;
            Asserts.check(rightExpression.Expression is ConstantExpression, $"Expected value for right member of expression '{expression.ToString()}'");
            rightConstantExpression = (ConstantExpression)rightExpression.Expression;
            value = rightConstantExpression.Value;



            filter = builders.QueryFactory.buildFilter<TFilter>();
            //filter.setFieldValue(fieldName);



            throw new NotImplementedException();
        }
    }
}