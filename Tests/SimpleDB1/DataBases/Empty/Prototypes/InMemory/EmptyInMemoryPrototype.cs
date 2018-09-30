using System;
using QTFK.Services;

namespace SimpleDB1.DataBases.Empty.Prototypes.InMemory
{
    public class EmptyInMemoryPrototype : IEmptyDB
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

        public EmptyInMemoryPrototype()
        {
            this.EngineFeatures = new PrvEngineFeatures();
        }

        public IEngineFeatures EngineFeatures { get; }

        public void transact(Func<bool> transactionBlock)
        {
            throw new NotSupportedException();
        }

    }
}
