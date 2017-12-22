using QTFK.Models;

namespace QTFK.Services
{
    public interface IExpressionParserFactory
    {
        IExpressionParser build(IEntityDescription entityDescription, IQueryFactory queryFactory);
    }
}