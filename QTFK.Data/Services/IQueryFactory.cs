using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;

namespace QTFK.Services
{
    public interface IQueryFactory<T> : IQueryFactory where T : new()
    {
        IQueryFilter GetFilterForMethodName(string methodName, params object[] args);
    }
}
