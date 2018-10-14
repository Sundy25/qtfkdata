using System.Collections.Generic;
using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface IUserTable : ITable<IUser>
    {
        IView<IUser> whereMailContains(string mail);
        int deleteWhereNameStartsWith(string namePrefix);
    }
}