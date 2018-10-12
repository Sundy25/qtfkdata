﻿using System;
using QTFK.Data;
using QTFK.Data.Abstracts;
using QTFK.Data.Storage;
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

        private class PrvUsersView : AbstractSqlServerView<IUser, ISqlServerStorage>
        {
            protected override Query prv_getPageSelectQuery(int offset, int pageSize)
            {
                Query outerQuery;
                string innerQuery;

                innerQuery = $@"
SELECT  [id], [name], [birthDate], [isEnabled] 
        , ROW_NUMBER() OVER ( ORDER BY [name] ASC ) AS [__row]
FROM [user]
";

                outerQuery = $@"
SELECT *
FROM ({innerQuery}) as __s
WHERE @lowerRow <= [__row] AND [__row] < @upperRow 
";

                outerQuery.Parameters.Add("@lowerRow", offset);
                outerQuery.Parameters.Add("@upperRow", offset + pageSize);

                return outerQuery;
            }

            protected override Query prv_getSelectCountQuery()
            {
                return "SELECT COUNT(id) FROM [user]";
            }

            protected override Query prv_getSelectQuery()
            {
                return $@"
SELECT [id], [name], [birthDate], [isEnabled] 
FROM [user]
";
            }

            protected override IUser prv_mapEntity(IRecord record)
            {
                return new PrvUser
                {
                    Id = record.get<int>("id"),
                    Name = record.get<string>("name"),
                    BirthDate = record.get<DateTime>("birthDate"),
                    IsEnabled = record.get<bool>("isEnabled"),
                };
            }

            public PrvUsersView(ISqlServerStorage storage) : base(storage)
            {
            }
        }

        private readonly ISqlServerStorage storage;

        public PrototypeSqlServerReadonlyUsersDB(ISqlServerStorage storage)
        {
            this.EngineFeatures = new PrvEngineFeatures();
            this.storage = storage;
            this.Users = new PrvUsersView(storage);
        }

        public IView<IUser> Users { get; }
        public IEngineFeatures EngineFeatures { get; }

        public void transact(Func<bool> transactionBlock)
        {
            throw new NotSupportedException();
            //using (IStorageTransaction transaction = this.storage.getTransaction())
            //{
            //    bool commit;

            //    commit = transactionBlock();

            //    if (commit)
            //        transaction.commit();
            //    else
            //        transaction.rollback();
            //}
        }

    }
}
