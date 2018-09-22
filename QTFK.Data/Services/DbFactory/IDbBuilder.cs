namespace QTFK.Services.DbFactory
{
    public interface IDbBuilder
    {
        TDB createDb<TDB>(IDbMetadata<TDB> dbMetadata) where TDB : class, IDB;
    }
}