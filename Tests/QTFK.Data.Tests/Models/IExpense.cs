using System;
using System.Collections.Generic;
using QTFK.Attributes;
using QTFK.Models;

namespace QTFK.Data.Tests.Models
{
    public interface IExpense : IEntity
    {
        [Id]
        int Id { get; set; }
        string Concept { get; set; }
        DateTime Date { get; set; }

        IEnumerable<ICategory> Categories { get; }
        IEnumerable<IUser> Debtors { get; }
        IEnumerable<IPayment> Contributors { get; }
    }
}