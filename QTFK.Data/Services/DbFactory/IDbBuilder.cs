namespace QTFK.Services.DbFactory
{
    public interface IDbBuilder
    {
        TDB createDb<TDB>(IDbMetadata dbMetadata) where TDB : IDB;
    }
}