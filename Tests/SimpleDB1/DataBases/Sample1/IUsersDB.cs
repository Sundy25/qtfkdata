using QTFK.Services;

namespace SimpleDB1.DataBases.Sample1
{
    public interface IUsersDB : IDB
    {
        IFullTable<IUser> Users { get; }
    }
}
