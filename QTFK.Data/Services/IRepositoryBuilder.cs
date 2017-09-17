using System;
using System.Collections.Generic;
using System.Reflection;

namespace QTFK.Services
{
    public interface IRepositoryBuilder
    {
        //IRepository<T> Build(typeof(ISampleRepository), queryFactory, methodParsers);
        //T Build<T>(queryFactory, methodParsers) where T: IRepository<>;
        Assembly Build(Type interfaceType, IQueryFactory<> queryFactory, IEnumerable<IMethodParser> methodParsers);
    }
}