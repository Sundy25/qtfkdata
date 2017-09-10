using System.Reflection;
using QTFK.Models;
using System.Linq;
using System;

namespace QTFK.Services.FilterParsers
{
    public class ByParamEqualsFilterParser : IMethodParser
    {
        public IQueryFilter Parse(MethodBase method, EntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection)
        {
            string pattern = "GetBy{0}";
            string methodName = method.Name;
            if (entityDescription.Fields.Any(f => string.Format(pattern, f) == methodName))
            {
                var factory = filterFactoryCollection.Build<IByParamEqualsFilterFactory>();
                return factory?.NewByParamEqualsFilter();
            }
            return null;
        }

        public IQueryFilter Parse<T1>(MethodBase method, EntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection) where T1 : struct
        {
            return null;
        }

        public IQueryFilter Parse<T1, T2>(MethodBase method, EntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection)
            where T1 : struct
            where T2 : struct
        {
            return null;
        }
    }
}
