using System;
using System.Linq;
using QTFK.Data.Storage;

namespace QTFK.Data.Abstracts
{
    public abstract class AbstractSqlServerTable<TEntity, TStorage> : AbstractSqlServerView<TEntity, TStorage>, ITable<TEntity>
        where TEntity : class, IEntity, new()
        where TStorage : ISqlServerStorage
    {
        protected abstract Query prv_getInsertQuery(TEntity entity);
        protected abstract bool prv_getSelectQueryIfEntityHasAutoKeyColumn(TEntity entity, out Query selectQuery);
        protected abstract Query prv_getDeleteQuery(TEntity item);
        protected abstract Query prv_getDeleteAllQuery();
        protected abstract Query prv_getUpdateQuery(TEntity item);

        public AbstractSqlServerTable(TStorage storage) : base(storage)
        {
        }

        public TEntity create(Func<TEntity, bool> item)
        {
            TEntity entity;
            bool submit;

            entity = new TEntity();
            submit = item(entity);

            if (submit)
            {
                Query query;
                int insertResult;
                bool hasAutoKey;

                query = prv_getInsertQuery(entity);
                insertResult = this.storage.write(query);
                Asserts.check(insertResult == 1, $"Insert statement has returned unexpected inserted rows count: {insertResult}");

                //newId = transaction.readSingle<int>("SELECT SCOPE_IDENTITY()");
                hasAutoKey = prv_getSelectQueryIfEntityHasAutoKeyColumn(entity, out query);

                if (hasAutoKey)
                {
                    entity = this.storage
                        .read(query)
                        .Select<IRecord, TEntity>(prv_mapEntity)
                        .Single<TEntity>();
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
