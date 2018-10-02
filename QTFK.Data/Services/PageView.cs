using System.Collections;
using System.Collections.Generic;

namespace QTFK.Services
{
    public class PageView<T> : IPageView<T>
    {
        private readonly IEnumerator<T> enumerator;

        public PageView(int pageSize, int page, int pagesCount, IEnumerator<T> enumerator)
        {
            Asserts.check(pageSize > 0, $"Constructor parameter '{pageSize}' must be greater than zero.");
            Asserts.check(pagesCount >= 0, $"Constructor parameter '{pagesCount}' must be greater than zero.");
            Asserts.check(page >= 0, $"Constructor parameter '{page}' must be greater or equal than zero.");
            Asserts.isSomething(enumerator, $"Constructor parameter '{enumerator}' cannot be null.");

            this.PageSize = pageSize;
            this.CurrentPage = page;
            this.PagesCount = pagesCount;
            this.enumerator = enumerator;
        }

        public int PagesCount { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return this.enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.enumerator;
        }
    }

}
