using System;
using System.Collections.Generic;

namespace QTFK.Data.Storage
{
    public interface IStorageTransaction
    {
        IEnumerable<IRecord> read(Query query);
        T readSingle<T>(Query query) where T : struct;
        int write(Query query);
        void rollback();
        void commit();
    }
}
