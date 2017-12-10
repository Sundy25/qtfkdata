namespace QTFK.Services.Repositories
{
    public class Repository<T> : BaseRepository<T> where T : class, new()
    {
        public Repository() 
            : base()
        {
        }
        public Repository(IDBIO db, IQueryFactory queryFactory) 
            : base(db, queryFactory)
        {
        }
    }
}