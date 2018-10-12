using System;
using QTFK.Data;
using QTFK.Services;
using SimpleDB1.DataBases.Empty;

namespace SimpleDB1.Prototypes.Empty.InMemory
{
    public class PrototypeInMemoryEmptyDB : IEmptyDB
    {
        private class PrvEngineFeatures : IEngineFeatures
        {
            public PrvEngineFeatures()
            {
                this.SupportsStoredProcedures = false;
                this.SupportsTransactions = false;
            }
            public bool SupportsTransactions { get; }
            public bool SupportsStoredProcedures { get; }
        }

        public PrototypeInMemoryEmptyDB()
        {
            this.EngineFeatures = new PrvEngineFeatures();
        }

        public IEngineFeatures EngineFeatures { get; }
    }
}
