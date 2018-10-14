using QTFK.Data;

namespace SimpleDB1.DataBases.Sample1
{
    public interface IUserTable : ITable<IUser>
    {
        IView<IUser> whereNameEquals(string name);
    }
}