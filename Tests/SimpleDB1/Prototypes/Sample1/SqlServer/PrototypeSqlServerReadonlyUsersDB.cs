using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QTFK;
using QTFK.Extensions.DataReader;
using QTFK.Extensions.DBIO;
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
            private static string prv_getSelectCountQuery()
            {
                return "SELECT COUNT(id) FROM [user]";
            }

            private static string prv_getSelectQuery()
            {
                return $@"
SELECT [id], [name], [birthDate], [isEnabled] 
FROM [user]
";
            }

            private static string prv_getSelectWithRowNumberQuery(string orderByField, string direction)
            {
                return $@"
SELECT  [id], [name], [birthDate], [isEnabled] 
        ROW_NUMBER() OVER ( ORDER BY {orderByField} {direction} ) AS [__row]
FROM [user]
";
            }

            private static string prv_getPageSelectQuery(int offset, int pageSize, string orderByField, string direction)
            {
                string subQuery;

                subQuery = prv_getSelectWithRowNumberQuery(orderByField, direction);

                return $@"
SELECT *
FROM ({subQuery})
WHERE {offset} <= [__row] AND [__row] < {offset + pageSize} 
";
                throw new NotImplementedException();
            }

            private static IUser prv_map(IDataRecord record)
            {
                return new PrvUser
                {
                    Id = record.Get<int>("id"),
                    Name = record.Get<string>("name"),
                    BirthDate = record.Get<DateTime>("birthDate"),
                    IsEnabled = record.Get<bool>("isEnabled"),
                };
            }

            private static IEnumerator<IUser> prv_getEnumerator(ISqlServerDBIO dbio)
            {
                IEnumerator<IUser> enumerator;
                string query;

                query = prv_getSelectQuery();

                enumerator = dbio
                    .Get<IUser>(query, prv_map)
                    .GetEnumerator();

                return enumerator;
            }

            private static IEnumerator<IUser> prv_getPageEnumerator(ISqlServerDBIO dbio, int offset, int pageSize, string orderByField, string direction)
            {
                IEnumerator<IUser> enumerator;
                string query;

                query = prv_getPageSelectQuery(offset, pageSize, orderByField, direction);

                enumerator = dbio
                    .Get<IUser>(query, prv_map)
                    .GetEnumerator();

                return enumerator;
            }

            private readonly ISqlServerDBIO dbio;

            public PrvUsersView(ISqlServerDBIO dbio)
            {
                this.dbio = dbio;
            }

            public int Count
            {
                get
                {
                    int rowsCount;
                    string query;

                    query = prv_getSelectCountQuery();
                    rowsCount = this.dbio.GetScalar<int>(query);

                    return rowsCount;
                }
            }

            public IEnumerator<IUser> GetEnumerator()
            {
                return prv_getEnumerator(this.dbio);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return prv_getEnumerator(this.dbio);
            }

            public IPageCollection<IUser> getPages(int pageSize)
            {
                PageCollection<IUser> pageCollection;
                int pagesCount, lastPageSize;
                Func<IEnumerator<IUser>>[] enumeratorCreatorDelegates;

                pagesCount = Math.DivRem(this.Count, pageSize, out lastPageSize);
                if (lastPageSize > 0)
                    pagesCount++;

                enumeratorCreatorDelegates = new Func<IEnumerator<IUser>>[pagesCount];

                for (int i = 0; i < pagesCount; i++)
                {
                    int offset;

                    offset = pageSize * i;
                    enumeratorCreatorDelegates[i] = () => prv_getPageEnumerator(this.dbio, offset, pageSize, "name", "ASC");
                }

                pageCollection = new PageCollection<IUser>(enumeratorCreatorDelegates, pageSize, lastPageSize);

                return pageCollection;
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
