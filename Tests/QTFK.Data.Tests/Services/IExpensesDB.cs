using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface IExpensesDB : IDB
    {
        IUserCrud Users { get; }
        IFullTable<IExpense> Expenses { get; }
        IView<IPayment> Payments { get; }
        ICategoryCrud Categories { get; }

        IFullTable<ICurrencyConversion> CurrencyExchanges { get; }
        IFullTable<ICurrency> Currencies { get; }

        ExpenseAmountView ExpenseAmounts { get; }
    }
}
