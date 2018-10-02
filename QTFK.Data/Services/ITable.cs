using System;

namespace QTFK.Services
{
    public interface ITable<T> where T : IEntity
    {
        T create(Action<T> item);
        void update(T item);
        void delete(T item);
    }
}
