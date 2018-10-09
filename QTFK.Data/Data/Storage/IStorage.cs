namespace QTFK.Data.Storage
{
    public interface IStorage : IStorageOperator
    {
        ITransaction beginTransaction();
    }
}
