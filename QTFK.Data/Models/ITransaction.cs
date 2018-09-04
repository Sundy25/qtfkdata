namespace QTFK.Models
{
    public interface ITransaction
    {
        void commit();
        void rollback();
    }
}