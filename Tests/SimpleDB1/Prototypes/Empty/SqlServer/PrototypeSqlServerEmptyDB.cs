using QTFK.Data;
using QTFK.Data.Storage;
using QTFK.Services.DBIO;
using SimpleDB1.DataBases.Empty;

namespace SimpleDB1.Prototypes.Empty.SqlServer
{
    public class PrototypeSqlServerEmptyDB : IEmptyDB
    {
        private class PrvEngineFeatures : IEngineFeatures
        {
            public PrvEngineFeatures()
            {
                this.SupportsStoredProcedures = true;
                this.SupportsTransactions = true;
            }
            public bool SupportsTransactions { get; }
            public bool SupportsStoredProcedures { get; }
        }

        private readonly ISqlServerStorage storage;

        public PrototypeSqlServerEmptyDB(ISqlServerStorage storage)
        {
            this.EngineFeatures = new PrvEngineFeatures();
            this.storage = storage;
        }

        public IEngineFeatures EngineFeatures { get; }

        public void save()
        {
            this.storage.commit();
        }
    }
}
