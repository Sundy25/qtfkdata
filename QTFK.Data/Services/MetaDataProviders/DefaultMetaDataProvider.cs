using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services.MetaDataProviders
{
    public class DefaultMetaDataProvider : IMetaDataProvider
    {
        public DefaultMetaDataProvider()
        {
        }

        public string GetEntityName(Type t)
        {
            return t.Name;
        }

        public IEnumerable<string> GetKeys(Type t)
        {
            return t.GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name.ToLower().StartsWith("id"))
                .Select(p => p.Name)
                ;
        }

        public IEnumerable<string> GetPropertyNames(Type t)
        {
            return t.GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p => p.Name)
                ;
        }
    }
}