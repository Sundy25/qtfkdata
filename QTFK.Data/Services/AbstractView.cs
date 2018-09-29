using System;
using System.Collections;
using System.Collections.Generic;

namespace QTFK.Services
{
    public abstract class AbstractView<TDB, T> : IPaginable<T> where TDB : IDB
    {
        protected abstract IEnumerable<T> getRecords(TDB db);

        public int Count => throw new NotImplementedException();

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IPageView<T> paginate(int pageSize, int page)
        {
            throw new NotImplementedException();
        }
    }
}
