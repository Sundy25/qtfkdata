using System;
using QTFK.Data.Storage;

namespace QTFK.Data.Abstracts
{
    public abstract class AbstractSqlServerTable<TEntity, TStorage> : AbstractSqlServerView<TEntity, TStorage>, ITable<TEntity>
        where TEntity : class, IEntity, new()
        where TStorage : ISqlServerStorage
    {
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
