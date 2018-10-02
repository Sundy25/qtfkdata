using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services
{
    public abstract class AbstractView<TDB, T> : IPaginable<T> where TDB : IDB
    {
        private TDB db;

        protected abstract IEnumerable<T> getRecords(TDB db);

        public void setDb(TDB db)
        {
            Asserts.isSomething(db, $"Parameter {db} cannot be null.");
            this.db = db;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return getRecords(this.db)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return getRecords(this.db)
                .GetEnumerator();
        }

        public IPageView<T> paginate(int pageSize, int page)
        {
            PageView<T> pageViewResult;
            int offset;
            IEnumerable<T> items;
            IEnumerator<T> enumerator;

            items = getRecords(this.db);
            offset = pageSize * page;
            enumerator = items
                .Skip(offset)
                .Take(pageSize)
                .GetEnumerator();
            pageViewResult = new PageView<T>(pageSize, page, 0, enumerator);

            return pageViewResult;
        }
    }
}
