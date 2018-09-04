using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IQuery<TDB, TResult>
    {
        IEnumerable<TResult> get(TDB db);
    }
}
