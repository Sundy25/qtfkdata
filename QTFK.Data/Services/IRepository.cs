using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IRepository<T> : IEnumerable<T>
    {
        T create();
        void insert(ref T item);
        void update(T item);
        void delete(T item);
    }
}
