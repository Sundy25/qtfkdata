using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface IExpensesDB : IDB
    {
        IUserCrud Users { get; }
        ICrud<IExpense> Expenses { get; }
        IView<IPayment> Payments { get; }
        ICategoryCrud Categories { get; }

        ICrud<ICurrencyConversion> CurrencyExchanges { get; }
        ICrud<ICurrency> Currencies { get; }

        ExpenseAmountView ExpenseAmounts { get; }
    }
}
