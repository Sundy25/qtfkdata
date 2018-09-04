using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public class ExpensesQuery : IQuery<IExpensesDB, IExpense>
    {
        public IEnumerable<IExpense> get(IExpensesDB db)
        {
            throw new NotImplementedException();
        }
    }
}
