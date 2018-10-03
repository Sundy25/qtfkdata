using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface IExpensesDB : IDB
    {
        IUserCrud Users { get; }
        ITable<IExpense> Expenses { get; }
        IView<IPayment> Payments { get; }
        ICategoryCrud Categories { get; }

        ITable<ICurrencyConversion> CurrencyExchanges { get; }
        ITable<ICurrency> Currencies { get; }
    }
}
