using System;
using System.Data;
using QTFK.Data;
using QTFK.Data.Abstracts;
using QTFK.Extensions.DataReader;
using QTFK.Extensions.DBIO;
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

        private class PrvUsersView : AbstractSqlServerView<IUser, ISqlServerDBIO>
        {
            protected override string prv_getDefaultOrderByColumns()
            {
                return "name";
            }

            protected override string prv_getDefaultOrderByDirection()
            {
                return "ASC";
            }

            protected override string prv_getPageSelectQuery(int offset, int pageSize, string orderByColumns, string orderByDirection)
            {
                string subQuery, superQuery;

                subQuery = $@"
SELECT  [id], [name], [birthDate], [isEnabled] 
        ROW_NUMBER() OVER ( ORDER BY {orderByColumns} {orderByDirection} ) AS [__row]
FROM [user]
;";

                superQuery = $@"
SELECT *
FROM ({subQuery})
WHERE {offset} <= [__row] AND [__row] < {offset + pageSize} 
;";

                return superQuery;
            }

            protected override string prv_getSelectCountQuery()
            {
                return "SELECT COUNT(id) FROM [user]";
            }

            protected override string prv_getSelectQuery()
            {
                return $@"
SELECT [id], [name], [birthDate], [isEnabled] 
FROM [user]
;";
            }

            protected override IUser prv_mapEntity(IDataRecord record)
            {
                return new PrvUser
                {
                    Id = record.Get<int>("id"),
                    Name = record.Get<string>("name"),
                    BirthDate = record.Get<DateTime>("birthDate"),
                    IsEnabled = record.Get<bool>("isEnabled"),
                };
            }

            public PrvUsersView(ISqlServerDBIO dbio) : base(dbio)
            {
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
