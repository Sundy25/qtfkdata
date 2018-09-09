using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface IExpensesDB : IDB
    {
        IUserRepository Users { get; }
        IRepository<IExpense> Expenses { get; }
        IView<IPayment> Payments { get; }
        ICategoryRepository Categories { get; }

        IRepository<ICurrencyConversion> CurrencyExchanges { get; }
        IRepository<ICurrency> Currencies { get; }

        ExpenseAmountView ExpenseAmounts { get; }
    }
}
