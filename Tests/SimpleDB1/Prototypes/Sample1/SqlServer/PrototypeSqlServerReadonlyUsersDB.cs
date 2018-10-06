using System;
using System.Collections;
using System.Collections.Generic;
using QTFK.Services;
using QTFK.Services.DBIO;
using SimpleDB1.DataBases.Sample1;

namespace SimpleDB1.Prototypes.Sample1.SqlServer
{
    public class PrototypeSqlServerReadonlyUsersDB : IReadonlyUsersDB
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

        private class PrvUser : IUser
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime BirthDate { get; set; }
            public bool IsEnabled { get; set; }
        }

        private class PrvUsersView : IView<IUser>
        {
            private readonly ISqlServerDBIO dbio;

            public PrvUsersView(ISqlServerDBIO dbio)
            {
                this.dbio = dbio;
            }

            public int Count { get; }

            public IEnumerator<IUser> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public IPageCollection<IUser> getPages(int pageSize)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private readonly ISqlServerDBIO dbio;

        public PrototypeSqlServerReadonlyUsersDB(ISqlServerDBIO dbio)
        {
            this.EngineFeatures = new PrvEngineFeatures();
            this.dbio = dbio;
            this.Users = new PrvUsersView(dbio);
        }

        public IView<IUser> Users { get; }
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
