using QTFK.Services;

namespace SimpleDB1
{
    public interface IUsersDB : IDB
    {
        IRepository<IUser> Users { get; }
    }
}
