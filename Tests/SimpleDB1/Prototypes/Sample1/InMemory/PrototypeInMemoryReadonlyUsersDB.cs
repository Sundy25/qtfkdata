using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QTFK.Data;
using QTFK.Data.Concretes;
using QTFK.Services;
using SimpleDB1.DataBases.Sample1;

namespace SimpleDB1.Prototypes.Sample1.InMemory
{
    public class PrototypeInMemoryReadonlyUsersDB : IReadonlyUsersDB
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

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.users.GetEnumerator();
            }

            public IPageCollection<IUser> getPages(int pageSize)
            {
                PageCollection<IUser> pageCollection;
                int pagesCount, lastPageSize;
                Func<IEnumerator<IUser>>[] enumeratorCreatorDelegates;


                pagesCount = Math.DivRem(this.users.Count, pageSize, out lastPageSize);
                if (lastPageSize > 0)
                    pagesCount++;

                enumeratorCreatorDelegates = new Func<IEnumerator<IUser>>[pagesCount];

                for (int i = 0; i < pagesCount; i++)
                {
                    int offset;
                    Func<IEnumerator<IUser>> enumeratorCreatorDelegate;

                    offset = pageSize * i;
                    enumeratorCreatorDelegate = () =>
                    {
                        IEnumerator<IUser> enumerator;

                        enumerator = this.users
                            .Skip(offset)
                            .Take(pageSize)
                            .GetEnumerator();

                        return enumerator;
                    };

                    enumeratorCreatorDelegates[i] = enumeratorCreatorDelegate;
                }

                pageCollection = new PageCollection<IUser>(enumeratorCreatorDelegates, pageSize, lastPageSize);

                return pageCollection;
            }
        }

        public PrototypeInMemoryReadonlyUsersDB()
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
