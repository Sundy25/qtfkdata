using System.Collections.Generic;

namespace QTFK.Data.Storage
{
    public class SqlServerStorage : ISqlServerStorage
    {
        public ITransaction beginTransaction()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IRecord> read(IQuery query)
        {
            throw new System.NotImplementedException();
        }

        public T readSingle<T>(IQuery query) where T : struct
        {
            throw new System.NotImplementedException();
        }

        public int write(IQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}
