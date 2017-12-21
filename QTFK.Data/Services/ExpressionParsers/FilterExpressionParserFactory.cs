using System;
using System.Linq.Expressions;
using QTFK.Models;
using QTFK.Models.QueryFilters;
using QTFK.Extensions.DBIO.QueryFactory;
using System.Reflection;

namespace QTFK.Services.ExpressionParsers
{
    public class FilterExpressionParserFactory : IExpressionParserFactory
    {
        public IExpressionParser<T> build<T>(IEntityDescription entityDescription, IQueryFactory queryFactory)
        {
            Asserts.isSomething(entityDescription, $"Constructor parameter '{nameof(entityDescription)}' cannot be null.");

            return new PrvExpressionParser<T>(entityDescription, queryFactory);
        }

        private class PrvExpressionParser<T> : IExpressionParser<T>
        {
            private IEntityDescription entityDescription;
            private readonly IQueryFactory queryFactory;

            public PrvExpressionParser(IEntityDescription entityDescription, IQueryFactory queryFactory)
            {
                this.entityDescription = entityDescription;
                this.queryFactory = queryFactory;
            }

            public IQueryFilter parse(Expression<Func<T, bool>> filterExpression)
            {
                IQueryFilter queryFilter;

                Asserts.isSomething(this.queryFactory, $"Property '{nameof(this.queryFactory)}' cannot be null.");
                Asserts.isSomething(filterExpression, $"Parameter '{nameof(filterExpression)}' cannot be null.");

                queryFilter = prv_parseExpr(filterExpression.Body);

                return queryFilter;
            }

            private IQueryFilter prv_parseExpr(Expression expression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.OrElse:
                        return prv_parseOr((BinaryExpression)expression);
                    case ExpressionType.AndAlso:
                        return prv_parseAnd((BinaryExpression)expression);
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                        return prv_parseComparer(expression);
                    default:
                        throw new NotSupportedException($"Parse error or unsupported expression of node type {expression.NodeType}: '{expression.ToString()}'");
                }
            }

            private IQueryFilter prv_parseOr(BinaryExpression expression)
            {
                IOrQueryFilter orFilter;

                orFilter = this.queryFactory.buildFilter<IOrQueryFilter>();
                orFilter.Left = prv_parseExpr(expression.Left);
                orFilter.Right = prv_parseExpr(expression.Right);

                return orFilter;
            }

            private IQueryFilter prv_parseAnd(BinaryExpression expression)
            {
                IAndQueryFilter orFilter;

                orFilter = this.queryFactory.buildFilter<IAndQueryFilter>();
                orFilter.Left = prv_parseExpr(expression.Left);
                orFilter.Right = prv_parseExpr(expression.Right);

                return orFilter;
            }

            private IQueryFilter prv_parseComparer(Expression expression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.GreaterThan:
                        return prv_parseConcreteComparer<IGreaterThanQueryFilter>((BinaryExpression)expression);
                    case ExpressionType.GreaterThanOrEqual:
                        return prv_parseConcreteComparer<IGreaterThanOrEqualQueryFilter>((BinaryExpression)expression);
                    case ExpressionType.LessThan:
                        return prv_parseConcreteComparer<ILessThanQueryFilter>((BinaryExpression)expression);
                    case ExpressionType.LessThanOrEqual:
                        return prv_parseConcreteComparer<ILessThanOrEqualQueryFilter>((BinaryExpression)expression);
                    case ExpressionType.Equal:
                        return prv_parseConcreteComparer<IEqualQueryFilter>((BinaryExpression)expression);
                    case ExpressionType.NotEqual:
                        return prv_parseConcreteComparer<INotEqualQueryFilter>((BinaryExpression)expression);
                    default:
                        throw new NotSupportedException($"Parse error or unsupported expression of node type {expression.NodeType}: '{expression.ToString()}'");
                }
            }

            private IQueryFilter prv_parseConcreteComparer<TFilter>(BinaryExpression expression) where TFilter : IComparerQueryFilter
            {
                TFilter filter;
                object value;
                string fieldName;

                fieldName = prv_parsePropertyName(expression.Left);
                value = prv_parseValue(expression.Right);

                filter = this.queryFactory.buildFilter<TFilter>();
                filter.setFieldValue(fieldName, value);

                return filter;
            }

            private string prv_parsePropertyName(Expression expression)
            {
                MemberExpression memberExpression;
                PropertyInfo property;
                string fieldName;

                Asserts.check(expression.NodeType == ExpressionType.MemberAccess, $"Un expected {expression.NodeType} node type: {expression.ToString()}");

                memberExpression = (MemberExpression)expression;

                Asserts.check(memberExpression.Member is PropertyInfo, $"Expected property for left member of expression '{expression.ToString()}'");

                property = (PropertyInfo)memberExpression.Member;
                fieldName = this.entityDescription.getField(property);

                return fieldName;
            }

            private object prv_parseValue(Expression expression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        return prv_parseMemberValue((MemberExpression)expression);
                    case ExpressionType.Constant:
                        return prv_parseConstant((ConstantExpression)expression);
                    default:
                        throw new NotSupportedException($"Parse error or unsupported expression of node type {expression.NodeType}: '{expression.ToString()}'");
                }
            }

            private object prv_parseConstant(ConstantExpression expression)
            {
                return expression.Value;
            }

            private object prv_parseMemberValue(MemberExpression rightExpression)
            {
                ConstantExpression rightConstantExpression;
                object value;
                FieldInfo fieldInfo;

                Asserts.check(rightExpression.Expression is ConstantExpression, $"Expected value for right member of expression '{rightExpression.ToString()}'");
                rightConstantExpression = (ConstantExpression)rightExpression.Expression;

                fieldInfo = (FieldInfo)rightExpression.Member;
                value = fieldInfo.GetValue(rightConstantExpression.Value);

                return value;
            }

        }
    }
}