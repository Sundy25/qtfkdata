namespace QTFK.Services.DbFactory
{
    public interface IDbBuilder
    {
        TDB createDb<TDB>() where TDB : IDB;
    }
}