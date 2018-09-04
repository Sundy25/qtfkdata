using System.Collections.Generic;
using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface IUserRepository : IRepository<IUser>
    {
        IEnumerable<IUser> getWhereMailContains(string mail);
        int deleteWhereNameStartsWith(string namePrefix);
    }
}