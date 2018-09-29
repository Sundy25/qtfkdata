using System.Collections.Generic;
using System.Linq;
using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public class ExpenseAmountView : AbstractView<IExpensesDB, ExpenseAmount>
    {
        protected override IEnumerable<ExpenseAmount> getRecords(IExpensesDB db)
        {
            return db.Expenses
                .Select(exp => new ExpenseAmount
                {
                    Id = exp.Id,
                    Concept = exp.Concept,
                    Amount = exp.Contributors.Sum(payment => payment.Amount),
                    TotalContributors = exp.Contributors.Count()
                });
        }
    }
}
