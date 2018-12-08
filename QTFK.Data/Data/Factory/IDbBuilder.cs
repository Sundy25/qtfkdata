using QTFK.Data.Factory.Metadata;

namespace QTFK.Data.Factory
{
    public interface IDbBuilder
    {
        TDB createDb<TDB>(IDbMetadata<TDB> dbMetadata) where TDB : class, IDB;
    }
}