using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface ICRUDRepositoryBuilder : IDependency
    {
        IEnumerable<Type> Build(IEnumerable<Assembly> assemblies);
    }
}
