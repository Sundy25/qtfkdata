using System;

namespace QTFK.Data.Storage
{
    public interface ITransaction : IStorageOperator, IDisposable
    {
        void rollback();
        void commit();
    }
}
