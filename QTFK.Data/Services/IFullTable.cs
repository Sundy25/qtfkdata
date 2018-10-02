using System;

namespace QTFK.Services
{
    public interface IFullTable<T> : ITable<T>, IView<T> where T : IEntity
    {
        int deleteAll();
    }
}
