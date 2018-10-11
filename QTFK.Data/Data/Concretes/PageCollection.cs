using System;
using System.Collections;
using System.Collections.Generic;

namespace QTFK.Data.Concretes
{
    public class PageCollection<T> : IPageCollection<T> where T : IEntity
    {
        private class PrvPageView : IPageView<T>
        {
            private readonly Func<IEnumerator<T>> enumeratorDelegate;

            private IEnumerator<T> prv_getEnumerator()
            {
                return this.enumeratorDelegate();
            }

            public PrvPageView(Func<IEnumerator<T>> enumeratorDelegate, int pageSize)
            {
                Asserts.isSomething(enumeratorDelegate, $"Constructor parameter '{enumeratorDelegate}' cannot be null.");
                Asserts.check(pageSize >= 0, $"Constructor parameter '{pageSize}' must be greater or equal than zero.");

                this.enumeratorDelegate = enumeratorDelegate;
                this.Count = pageSize;
            }

            public int Count { get; }

            public IEnumerator<T> GetEnumerator()
            {
                return prv_getEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return prv_getEnumerator();
            }
        }

        private readonly Func<IEnumerator<T>>[] enumeratorCreatorDelegates;
        private readonly int pageSize;
        private readonly int lastPageSize;

        public PageCollection(Func<IEnumerator<T>>[] enumeratorCreatorDelegates, int pageSize, int lastPageSize)
        {
            Asserts.isSomething(enumeratorCreatorDelegates, $"Constructor parameter '{nameof(enumeratorCreatorDelegates)}' cannot be null.");
            Asserts.check(pageSize >= 0, $"Constructor parameter '{nameof(pageSize)}' must be grater or equal than zero.");
            Asserts.check(0 <= lastPageSize && lastPageSize <= pageSize, $"Constructor parameter '{nameof(lastPageSize)}' must be less or equal than contructor paramter '{nameof(pageSize)}'.");

            this.Count = enumeratorCreatorDelegates.Length;
            this.enumeratorCreatorDelegates = enumeratorCreatorDelegates;
            this.pageSize = pageSize;
            this.lastPageSize = lastPageSize;
        }

        public IPageView<T> this[int index]
        {
            get
            {
                PrvPageView pageView;
                Func<IEnumerator<T>> enumeratorCreator;
                int currentPageSize;

                Asserts.check(0 <= index && index < this.Count, $"Index operator value is out of range.");

                if (index == this.Count - 1)
                    currentPageSize = this.lastPageSize;
                else
                    currentPageSize = this.pageSize;

                enumeratorCreator = this.enumeratorCreatorDelegates[index];
                Asserts.isSomething(enumeratorCreator, $"Constructor paramater '{nameof(this.enumeratorCreatorDelegates)}[{index}]' cannot be null.");

                pageView = new PrvPageView(enumeratorCreator, currentPageSize);

                return pageView;
            }
        }

        public int Count { get; }
    }
}
