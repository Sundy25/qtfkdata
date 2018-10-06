using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Data.Tests.Models
{
    public interface ICategory : IEntity
    {
        [Id]
        int Id { get; set; }
        string Name { get; set; }

        ICategory Parent { get; set; }

        [Foreign("parentCategoryId")]
        IView<ICategory> SubCategories { get; }
        IView<IExpense> Expenses { get; }
    }
}