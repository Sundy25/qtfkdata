using System.Collections.Generic;
using QTFK.Attributes;
using QTFK.Models;

namespace QTFK.Data.Tests.Models
{
    public interface ICategory : IEntity
    {
        [Id]
        int Id { get; set; }
        string Name { get; set; }

        ICategory Parent { get; set; }
        IEnumerable<ICategory> SubCategories { get; }
        IEnumerable<IExpense> Expenses { get; }
    }
}