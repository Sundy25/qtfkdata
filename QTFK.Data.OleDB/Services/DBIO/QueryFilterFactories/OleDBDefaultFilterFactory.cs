using System;
using System.Reflection;
using QTFK.Models;

namespace QTFK.Services.DBIO.QueryFilterFactories
{
    public class OleDBDefaultFilterFactory : IQueryFilterFactory
    {
        public IQueryFilter Build(MethodBase method, Type type)
        {
            return null;
        }
    }
}