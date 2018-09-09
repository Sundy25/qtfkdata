using QTFK.Data.Tests.Models;
using QTFK.Services;
using System.Collections.Generic;

namespace QTFK.Data.Tests.Services
{
    public interface ICategoryRepository : IRepository<ICategory>
    {
        IView<ICategory> whereParentIsNull();
        IView<ICategory> whereParentNameIs(string parentName);
    }
}