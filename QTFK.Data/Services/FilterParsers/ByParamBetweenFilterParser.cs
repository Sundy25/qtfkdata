using System.Reflection;
using QTFK.Models;
using System.Linq;
using System;

namespace QTFK.Services.FilterParsers
{
    public class ByParamBetweenFilterParser : IMethodParser
    {
        public IQueryFilter Parse(MethodBase method, EntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection)
        {
            return null;
        }

        public IQueryFilter Parse<T1>(MethodBase method, EntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection) where T1 : struct
        {
            string pattern = "GetBy{0}Between";
            string methodName = method.Name;
            if (entityDescription.Fields.Any(f => string.Format(pattern, f) == methodName))
            {
                var factory = filterFactoryCollection.Build<IByParamBetweenFilterFactory>();
                return factory?.NewByParamBetweenFilter<T1>();
            }
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
