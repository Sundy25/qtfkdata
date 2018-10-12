using System;
using QTFK.Data.Storage;

namespace QTFK.Data.Abstracts
{
    public abstract class AbstractSqlServerTable<TEntity, TStorage> : AbstractSqlServerView<TEntity, TStorage>, ITable<TEntity>
        where TEntity : class, IEntity, new()
        where TStorage : ISqlServerStorage
    {
        protected abstract Query prv_getInsertQuery(TEntity entity);
        protected abstract bool prv_entityHasAutoKey();
        protected abstract Query prv_getNewAutoKeySelectQuery();

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

                using (IStorageTransaction transaction = this.storage.beginTransaction())
                {
                    Query query;
                    int insertResult;
                    bool hasAutoKey;

                    query = prv_getInsertQuery(entity);

                    insertResult = transaction.write(query);
                    Asserts.check(insertResult == 1, $"Insert statement has returned unexpected inserted rows count: {insertResult}");

                    hasAutoKey = prv_entityHasAutoKey();

                    if(hasAutoKey)
                    {
                        query = prv_getNewAutoKeySelectQuery();
                    }
                }

                throw new NotImplementedException();
            }
            else
            {
                return null;
            }
        }

        public void delete(TEntity item)
        {
            throw new NotImplementedException();
        }

        public int deleteAll()
        {

            throw new NotImplementedException();
        }

        public void update(TEntity item)
        {
            throw new NotImplementedException();
        }
    }
}
