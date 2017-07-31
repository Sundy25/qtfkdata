using QTFK.Models;
using System;
using System.Reflection;

namespace QTFK.Services
{
    public interface IQueryFilterFactory
    {
        IQueryFilter Build(MethodBase method, Type type);
    }
}