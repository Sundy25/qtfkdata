using System.Collections.Generic;
using QTFK.Attributes;
using QTFK.Models;

namespace QTFK.Data.Tests.Models
{
    public interface IUser : IEntity
    {
        [Id]
        int Id { get; }
        string Name { get; set; }
        string Mail { get; set; }

        IEnumerable<IExpense> Debts { get; }
        IEnumerable<IPayment> Payments { get; }
    }
}