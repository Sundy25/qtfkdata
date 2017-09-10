using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace QTFK.Services
{
    public interface IMethodParser
    {
        IQueryFilter Parse(MethodBase method, EntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection);
        IQueryFilter Parse<T1>(MethodBase method, EntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection) where T1 : struct;
        IQueryFilter Parse<T1,T2>(MethodBase method, EntityDescription entityDescription, IQueryFilterFactoryCollection filterFactoryCollection) where T1 : struct where T2: struct;
    }
}
 