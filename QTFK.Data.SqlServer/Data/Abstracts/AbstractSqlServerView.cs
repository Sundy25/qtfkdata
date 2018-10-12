using System;
using System.Collections;
using System.Collections.Generic;
using QTFK.Data.Concretes;
using QTFK.Data.Storage;
using System.Linq;
using QTFK.Data.Extensions.Storages;

namespace QTFK.Data.Abstracts
{
    public abstract class AbstractSqlServerView<TEntity, TStorage> : IView<TEntity>
        where TEntity : IEntity
        where TStorage : ISqlServerStorage
    {
        private static IEnumerator<TEntity> prv_getEnumerator(TStorage storage, Query query, Func<IRecord, TEntity> entityMapFunction)
        {
            IEnumerator<TEntity> enumerator;

            enumerator = storage
                .read(query)
                .Select<IRecord, TEntity>(entityMapFunction)
                .GetEnumerator();

            return enumerator;
        }

        protected readonly TStorage storage;

        private IEnumerator<TEntity> prv_getEnumerator()
        {
            Query query;

            query = prv_getSelectQuery();
            return prv_getEnumerator(this.storage, query, prv_mapEntity);
        }

        protected abstract Query prv_getSelectCountQuery();
        protected abstract Query prv_getSelectQuery();
        protected abstract TEntity prv_mapEntity(IRecord record);
        protected abstract Query prv_getPageSelectQuery(int offset, int pageSize);

        public AbstractSqlServerView(TStorage storage)
        {
            this.storage = storage;
        }

        public int Count
        {
            get
            {
                int rowsCount;
                Query query;

                query = prv_getSelectCountQuery();
                rowsCount = this.storage.readSingle<int>(query);

                return rowsCount;
            }
        }

        public IPageCollection<TEntity> getPages(int pageSize)
        {
            PageCollection<TEntity> pageCollection;
            int pagesCount, lastPageSize;
            Func<IEnumerator<TEntity>>[] enumeratorCreatorDelegates;

            pagesCount = Math.DivRem(this.Count, pageSize, out lastPageSize);
            if (lastPageSize > 0)
                pagesCount++;

            enumeratorCreatorDelegates = new Func<IEnumerator<TEntity>>[pagesCount];

            for (int i = 0; i < pagesCount; i++)
            {
                int offset;
                Query query;

                offset = (pageSize * i) + 1;
                query = prv_getPageSelectQuery(offset, pageSize);
                enumeratorCreatorDelegates[i] = () => prv_getEnumerator(this.storage, query, prv_mapEntity);
            }

            pageCollection = new PageCollection<TEntity>(enumeratorCreatorDelegates, pageSize, lastPageSize);

            return pageCollection;
        }


        public IEnumerator<TEntity> GetEnumerator()
        {
            return prv_getEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return prv_getEnumerator();
        }
    }
}
