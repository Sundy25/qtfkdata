using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using QTFK.Data.Concretes;
using QTFK.Extensions.DBIO;
using QTFK.Services;

namespace QTFK.Data.Abstracts
{
    public abstract class AbstractSqlServerView<TEntity, TDBIO> : IView<TEntity> 
        where TEntity : IEntity
        where TDBIO : IDBIO
    {
        private static IEnumerator<TEntity> prv_getEnumerator(TDBIO dbio, string query, Func<IDataRecord, TEntity> entityMapFunction)
        {
            IEnumerator<TEntity> enumerator;

            enumerator = dbio
                .Get<TEntity>(query, entityMapFunction)
                .GetEnumerator();

            return enumerator;
        }

        private readonly TDBIO dbio;

        private IEnumerator<TEntity> prv_getEnumerator()
        {
            string query;

            query = prv_getSelectQuery();
            return prv_getEnumerator(this.dbio, query, prv_mapEntity);
        }

        protected abstract string prv_getSelectCountQuery();
        protected abstract string prv_getSelectQuery();
        protected abstract TEntity prv_mapEntity(IDataRecord record);
        protected abstract string prv_getPageSelectQuery(int offset, int pageSize, string orderByColumns, string orderByDirection);
        protected abstract string prv_getDefaultOrderByDirection();
        protected abstract string prv_getDefaultOrderByColumns();

        public AbstractSqlServerView(TDBIO dbio)
        {
            this.dbio = dbio;
        }

        public int Count
        {
            get
            {
                int rowsCount;
                string query;

                query = prv_getSelectCountQuery();
                rowsCount = this.dbio.GetScalar<int>(query);

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
                string query, orderByColumns, orderByDirection;

                offset = pageSize * i;
                orderByColumns = prv_getDefaultOrderByColumns();
                orderByDirection = prv_getDefaultOrderByDirection();
                query = prv_getPageSelectQuery(offset, pageSize, orderByColumns, orderByDirection);
                enumeratorCreatorDelegates[i] = () => prv_getEnumerator(this.dbio, query, prv_mapEntity);
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
