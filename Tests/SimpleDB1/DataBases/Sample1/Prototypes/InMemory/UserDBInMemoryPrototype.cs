using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            public IPageView<IUser> paginate(int pageSize, int page)
            {
                int pagesCount, offset;
                IEnumerable<IUser> paginatedUsers;

                offset = pageSize * page;
                pagesCount = (int)Math.Ceiling((decimal)this.users.Count / pageSize);
                paginatedUsers = this.users
                    .Skip(offset)
                    .Take(pageSize);

                return new PrvPageView<IUser>(pageSize, page, pagesCount, paginatedUsers);
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
