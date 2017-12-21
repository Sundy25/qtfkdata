using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Models.QueryFilters
{
    public interface IKeyFilter : IQueryFilter
    {
        void setKey(string key, object value);
    }
}
