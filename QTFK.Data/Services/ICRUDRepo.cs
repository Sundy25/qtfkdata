using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface ICRUDRepo<T> where T : class, new()
    {
        T Get(T item);
        T Add(T item);
        bool Set(T item);
        bool Remove(T id);
    }
}
