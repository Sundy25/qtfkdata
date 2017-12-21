using QTFK.Models;

namespace QTFK.Services
{
    public interface IExpressionParserFactory
    {
        IExpressionParser<T> build<T>(IEntityDescription entityDescription, IQueryFactory queryFactory);
    }
}