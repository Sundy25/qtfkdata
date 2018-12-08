using System;
using QTFK.Data.Attributes;

namespace QTFK.Data.Tests.Models
{
    public interface IUser : IEntity
    {
        [Id]
        int Id { get; }
        string Name { get; set; }
        string Mail { get; set; }

        [Column("creationDate")]
        DateTime SignDate { get; set; }

        IView<IExpense> Debts { get; }
        IView<IPayment> Payments { get; }
    }
}