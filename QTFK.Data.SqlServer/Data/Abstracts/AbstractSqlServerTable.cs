using System;
using System.Linq;
using QTFK.Data.Storage;

namespace QTFK.Data.Abstracts
{
    public abstract class AbstractSqlServerTable<TEntity, TStorage> : AbstractSqlServerView<TEntity, TStorage>, ITable<TEntity>
        where TEntity : class, IEntity
        where TStorage : ISqlServerStorage
    {
        protected abstract Query prv_getInsertQuery(TEntity item);
        protected abstract bool prv_needsReloadAfterInsert();
        protected abstract Query prv_getDeleteQuery(TEntity item);
        protected abstract Query prv_getDeleteAllQuery();
        protected abstract Query prv_getUpdateQuery(TEntity item);
        protected abstract TEntity prv_getNewEntity();

        public AbstractSqlServerTable(TStorage storage) : base(storage)
        {
        }

        public TEntity create(Func<TEntity, bool> item)
        {
            TEntity entity;
            bool submit;

            entity = prv_getNewEntity();
            submit = item(entity);

            if (submit)
            {
                Query query;
                bool needsReload;

                needsReload = prv_needsReloadAfterInsert();
                query = prv_getInsertQuery(entity);

                if (needsReload)
                {
                    entity = this.storage
                        .read(query)
                        .Select<IRecord, TEntity>(prv_mapEntity)
                        .Single<TEntity>();
                }
                else
                {
                    int insertResult;

                    insertResult = this.storage.write(query);
                    Asserts.check(insertResult == 1, $"Insert statement has returned unexpected inserted rows count: {insertResult}");
                }
            }
            else
                entity = null;

            return entity;
        }

        public void delete(TEntity item)
        {
            Query query;
            int deletedItems;

            query = prv_getDeleteQuery(item);
            deletedItems = this.storage.write(query);
            Asserts.check(deletedItems == 1, $"Expected only one affected row after delete statement execution.");
        }

        public int deleteAll()
        {
            Query query;
            int deletedItems;

            query = prv_getDeleteAllQuery();
            deletedItems = this.storage.write(query);

            return deletedItems;
        }

        public void update(TEntity item)
        {
            Query query;
            int updatedItems;

            query = prv_getUpdateQuery(item);
            updatedItems = this.storage.write(query);
            Asserts.check(updatedItems == 1, $"Expected only one affected row after delete statement execution.");
        }
    }
}
