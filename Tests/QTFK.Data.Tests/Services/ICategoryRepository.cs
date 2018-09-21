﻿using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface ICategoryRepository : IRepository<ICategory>
    {
        IView<ICategory> whereParentIsNull();
        IView<ICategory> whereParentNameIs(string parentName);
    }
}