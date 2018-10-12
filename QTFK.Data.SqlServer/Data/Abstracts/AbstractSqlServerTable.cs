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
        protected abstract bool prv_getSelectQueryIfEntityHasAutoKeyColumn(IStorageTransaction transaction, TEntity entity, out Query selectQuery);
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

            if(submit)
            {

                using (IStorageTransaction transaction = this.storage.getTransaction())
                {
                    Query query;
                    int insertResult;
                    bool hasAutoKey;

                    query = prv_getInsertQuery(entity);

                    insertResult = transaction.write(query);
                    Asserts.check(insertResult == 1, $"Insert statement has returned unexpected inserted rows count: {insertResult}");

                    //newId = transaction.readSingle<int>("SELECT SCOPE_IDENTITY()");
                    hasAutoKey = prv_getSelectQueryIfEntityHasAutoKeyColumn(transaction, entity, out query);

                    if(hasAutoKey)
                    {
                        entity = transaction
                            .read(query)
                            .Select<IRecord, TEntity>(prv_mapEntity)
                            .Single<TEntity>();
                    }
                    transaction.commit();
                }
            }
            else
            {
                entity = null;
            }

            return entity;
        }


        public void delete(TEntity item)
        {
            using (IStorageTransaction transaction = this.storage.getTransaction())
            {
                Query query;
                int deletedItems;

                query = prv_getDeleteQuery(item);
                deletedItems = transaction.write(query);
                Asserts.check(deletedItems == 1, $"Expected only one affected row after delete statement execution.");

                transaction.commit();
            }
        }

        public int deleteAll()
        {
            Query query;
            int deletedItems;

            using (IStorageTransaction transaction = this.storage.getTransaction())
            {
                query = prv_getDeleteAllQuery();
                deletedItems = transaction.write(query);
                transaction.commit();
            }

            return deletedItems;
        }

        public void update(TEntity item)
        {
            Query query;
            int updatedItems;

            using (IStorageTransaction transaction = this.storage.getTransaction())
            {
                query = prv_getUpdateQuery(item);
                updatedItems = transaction.write(query);
                Asserts.check(updatedItems == 1, $"Expected only one affected row after delete statement execution.");

                transaction.commit();
            }
        }
    }
}
