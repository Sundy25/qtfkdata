using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;

namespace QTFK.Services
{
    public interface IRepository<T> : IView<T> where T : IEntity
    {
        T create(Action<T> item);
        void update(T item);
        void delete(T item);
        int deleteAll();
    }
}
