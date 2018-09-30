using System;
using QTFK.Services;
using QTFK.Services.DBIO;

namespace SimpleDB1.DataBases.Empty.Prototypes.SqlServer
{
    public class EmptySqlServerPrototype : IEmptyDB
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

        private readonly ISqlServerDBIO dbio;

        public EmptySqlServerPrototype(ISqlServerDBIO dbio)
        {
            this.EngineFeatures = new PrvEngineFeatures();
            this.dbio = dbio;
        }

        public IEngineFeatures EngineFeatures { get; }

        public void transact(Func<bool> transactionBlock)
        {
            this.dbio.Set(command =>
            {
                bool blockResult;

                blockResult = transactionBlock();

                if (blockResult == false)
                    command.Transaction.Rollback();

                return 0;
            });
        }

    }
}
