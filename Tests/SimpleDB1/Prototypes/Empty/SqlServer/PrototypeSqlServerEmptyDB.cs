using System;
using QTFK.Services;
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

        private readonly ISqlServerDBIO dbio;

        public PrototypeSqlServerEmptyDB(ISqlServerDBIO dbio)
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
