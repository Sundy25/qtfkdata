using QTFK.Data;
using QTFK.Data.Storage;
using QTFK.Services.DBIO;
using SimpleDB1.DataBases.Empty;

namespace SimpleDB1.Prototypes.Empty.SqlServer
{
    public class PrototypeSqlServerEmptyDB : IEmptyDB
    {
        private readonly ISqlServerStorage storage;

        public PrototypeSqlServerEmptyDB(ISqlServerStorage storage)
        {
            this.storage = storage;
        }

        public void save()
        {
            this.storage.commit();
        }
    }
}
