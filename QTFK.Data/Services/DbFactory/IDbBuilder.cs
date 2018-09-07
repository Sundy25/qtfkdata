namespace QTFK.Services.DbFactory
{
    public interface IDbBuilder
    {
        TDB createDb<TDB>(DbMetadata dbMetadata, IDBIO driver) where TDB : IDB;
    }
}