using System;
using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IMetaDataProvider
    {
        string GetEntityName(Type t);
        IEnumerable<string> GetPropertyNames(Type t);
        IEnumerable<string> GetKeys(Type t);
    }
}