namespace QTFK.Services
{
    public interface IView<T> : IPaginable<T> where T: IEntity
    {
    }
}
