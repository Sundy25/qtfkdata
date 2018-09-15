namespace QTFK.Services.DbFactory
{
    public interface IDbBuilder
    {
        TDB createDb<TDB>(DbMetadata dbMetadata) where TDB : IDB;
    }
}