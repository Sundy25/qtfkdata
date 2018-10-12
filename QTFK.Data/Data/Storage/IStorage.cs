using System;

namespace QTFK.Data.Storage
{
    public interface IStorage : IDisposable
    {
        IStorageTransaction getTransaction();
    }
}
