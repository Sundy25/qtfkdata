using System;
using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IRepositoryExplorer
    {
        IEnumerable<Type> GetInterfaceTypes();
    }
}