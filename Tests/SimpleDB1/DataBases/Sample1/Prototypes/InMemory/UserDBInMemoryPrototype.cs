using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QTFK.Services;

namespace SimpleDB1.DataBases.Sample1.Prototypes.InMemory
{
    public class UserDBInMemoryPrototype : IReadonlyUsersDB
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

        private class PrvUsersView : IView<IUser>
        {
            private readonly IList<IUser> users;

            public PrvUsersView()
            {
                this.users = new List<IUser>();
            }

            public int Count
            {
                get
                {
                    return this.users.Count;
                }
            }

            public IEnumerator<IUser> GetEnumerator()
            {
                return this.users.GetEnumerator();
            }

            public IPageView<IUser>[] getPages(int pageSize)
            {
                IPageView<IUser>[] pagesResult;
                int pagesCount;

                pagesCount = (int)Math.Ceiling((decimal)this.users.Count / pageSize);
                pagesResult = new IPageView<IUser>[pagesCount];

                for (int i = 0; i < pagesCount; i++)
                {
                    int offset;
                    IEnumerator<IUser> paginatedUsers;

                    offset = pagesCount * i;
                    paginatedUsers = this.users
                        .Skip(offset)
                        .Take(pageSize)
                        .GetEnumerator();

                    pagesResult[i] = new PageView<IUser>(paginatedUsers);
                }

                return pagesResult;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.users.GetEnumerator();
            }

        }

        public UserDBInMemoryPrototype()
        {
            this.EngineFeatures = new PrvEngineFeatures();
            this.Users = new PrvUsersView();
        }

        public IEngineFeatures EngineFeatures { get; }
        public IView<IUser> Users { get; }

        public void transact(Func<bool> transactionBlock)
        {
            throw new NotSupportedException();
        }

    }
}
