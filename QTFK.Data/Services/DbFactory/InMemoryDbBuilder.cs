namespace QTFK.Services.DbFactory
{
    public class InMemoryDbBuilder : IDbBuilder
    {
        public TDB createDb<TDB>(IDbMetadata<TDB> dbMetadata) where TDB : IDB
        {
            throw new System.NotImplementedException();
        }
    }
}