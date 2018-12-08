using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface IExpensesDB : IDB
    {
        IUserTable Users { get; }
        ITable<IExpense> Expenses { get; }
        IView<IPayment> Payments { get; }
        ICategoryTable Categories { get; }

        ITable<ICurrencyConversion> CurrencyExchanges { get; }
        ITable<ICurrency> Currencies { get; }
    }
}
