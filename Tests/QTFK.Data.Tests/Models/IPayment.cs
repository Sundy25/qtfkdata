using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Data.Tests.Models
{
    public interface IPayment : IEntity
    {
        [Id]
        int Id { get; set; }

        decimal Amount { get; set; }

        IExpense Expense { get; }
        IUser User { get; }
        ICurrency Currency { get; }
    }
}