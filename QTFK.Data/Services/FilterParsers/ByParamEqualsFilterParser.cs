using System.Reflection;
using QTFK.Models;
using System.Linq;
using System;

namespace QTFK.Services.FilterParsers
{
    public class ByParamEqualsFilterParser : IMethodParser
    {
        public IQueryFilter Parse(MethodBase method, IEntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection)
        {
            string pattern = "GetBy{0}";
            string methodName = method.Name;
            foreach (var field in entityDescription.Fields)
            {
                if (string.Format(pattern, field) == methodName)
                {
                    var factory = filterFactoryCollection.Build<IByParamEqualsFilterFactory>();
                    if (factory != null)
                    {
                        var filter = factory.NewByParamEqualsFilter();
                        filter.Field = field;
                        return filter;
                    }
                    return null;
                }
            }
            return null;
        }

        public IQueryFilter Parse<T1>(MethodBase method, IEntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection) where T1 : struct
        {
            return null;
        }

        public IQueryFilter Parse<T1, T2>(MethodBase method, IEntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection)
            where T1 : struct
            where T2 : struct
        {
            return null;
        }
    }
}
