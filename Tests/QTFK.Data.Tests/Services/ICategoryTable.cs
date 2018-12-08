using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface ICategoryTable : ITable<ICategory>
    {
        IView<ICategory> whereParentIsNull();
        IView<ICategory> whereParentNameIs(string parentName);
    }
}