namespace QTFK.Services.Repositories
{
    public class Repository<T> : BaseRepository<T> where T : class, new()
    {
        public Repository(IEntityDescriber entityDescriber, IExpressionFilterParser expressionFilterParser) 
            : base(entityDescriber, expressionFilterParser)
        {
        }
    }
}