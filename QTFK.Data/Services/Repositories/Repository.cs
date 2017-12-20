namespace QTFK.Services.Repositories
{
    public class Repository<T> : BaseRepository<T> where T : class, new()
    {
        public Repository(IEntityDescriber entityDescriber, IExpressionParserFactory expressionParserFactory) 
            : base(entityDescriber, expressionParserFactory)
        {
        }
    }
}