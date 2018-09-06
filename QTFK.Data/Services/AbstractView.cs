using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public abstract class AbstractView<TDB, T> : IView<T> where TDB : IDB
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
