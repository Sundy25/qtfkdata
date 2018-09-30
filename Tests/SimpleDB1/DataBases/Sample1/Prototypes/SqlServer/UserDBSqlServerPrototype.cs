using System;
using System.Collections;
using System.Collections.Generic;
using QTFK.Services;
using QTFK.Services.DBIO;

namespace SimpleDB1.DataBases.Sample1.Prototypes.SqlServer
{
    public class UserDBSqlServerPrototype : IReadonlyUsersDB
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

        private class PrvPageView<T> : IPageView<T>
        {
            private readonly IEnumerable<T> items;

            public PrvPageView(int pageSize, int page, int pagesCount, IEnumerable<T> items)
            {
                this.PageSize = pageSize;
                this.CurrentPage = page;
                this.PagesCount = pagesCount;
                this.items = items;
            }

            public int PagesCount { get; }
            public int CurrentPage { get; }
            public int PageSize { get; }

            public IEnumerator<T> GetEnumerator()
            {
                return this.items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.items.GetEnumerator();
            }
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

            public IPageView<IUser> paginate(int pageSize, int page)
            {
                IEnumerable<IUser> paginatedUsers;
                int pagesCount;

                throw new NotImplementedException();

                return new PrvPageView<IUser>(pageSize, page, pagesCount, paginatedUsers);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private readonly ISqlServerDBIO dbio;

        public UserDBSqlServerPrototype(ISqlServerDBIO dbio)
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
