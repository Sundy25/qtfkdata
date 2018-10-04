using System.Collections;
using System.Collections.Generic;

namespace QTFK.Services
{
    public class PageView<T> : IPageView<T>
    {
        private readonly IEnumerator<T> enumerator;

        public PageView(IEnumerator<T> enumerator)
        {
            Asserts.isSomething(enumerator, $"Constructor parameter '{enumerator}' cannot be null.");

            this.enumerator = enumerator;
        }


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
