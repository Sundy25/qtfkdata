using QTFK.Data.Attributes;

namespace QTFK.Data.Tests.Models
{
    public interface IPayment : IEntity
    {
        [Id]
        int Id { get; set; }

        decimal Amount { get; set; }
        int CurrenyId { get; set; }

        IExpense Expense { get; }

        [Column("id_usuario")]
        IUser User { get; set; }

        [Column("currencyId")]
        ICurrency Currency { get; }
    }
}