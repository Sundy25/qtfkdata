using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IFactory<TService> where TService : class
    {
        TService GetDefault();
        TService Get(Type type);
        TService Get<T>();
        IFactory<TService> Register<T>(Func<TService> builder, bool asDefault);
        IEnumerable<Type> RegisteredTypes();
    }
}
