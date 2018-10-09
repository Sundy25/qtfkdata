using System.Collections.Generic;

namespace QTFK.Data.Storage
{
    public interface IStorageOperator
    {
        IEnumerable<IRecord> read(IQuery query);
        T readSingle<T>(IQuery query) where T : struct;
        int write(IQuery query);
    }
}
