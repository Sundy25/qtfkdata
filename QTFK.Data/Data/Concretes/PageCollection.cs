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
                Asserts.isNotNull(enumeratorDelegate);
                Asserts.isTrue(pageSize >= 0);

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
            Asserts.isNotNull(enumeratorCreatorDelegates);
            Asserts.isTrue(pageSize >= 0);
            Asserts.isTrue(0 <= lastPageSize && lastPageSize <= pageSize);

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

                Asserts.isTrue(0 <= index && index < this.Count);

                if (index == this.Count - 1)
                    currentPageSize = this.lastPageSize;
                else
                    currentPageSize = this.pageSize;

                enumeratorCreator = this.enumeratorCreatorDelegates[index];
                Asserts.isNotNull(enumeratorCreator);

                pageView = new PrvPageView(enumeratorCreator, currentPageSize);

                return pageView;
            }
        }

        public int Count { get; }
    }
}
