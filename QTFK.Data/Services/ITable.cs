using System;

namespace QTFK.Services
{
    public interface ITable<T> : IView<T> where T : IEntity
    {
        T create(Func<T, bool> item);
        void update(T item);
        void delete(T item);
        int deleteAll();
    }
}
