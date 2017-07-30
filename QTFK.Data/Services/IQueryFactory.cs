using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;
using System.Reflection;

namespace QTFK.Services
{
    public interface IQueryFactory<T> : IQueryFactory where T : new()
    {
        IQueryFilter GetFilter(MethodBase method);
    }
}
