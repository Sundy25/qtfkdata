namespace QTFK.Services
{
    public interface IQueryFilterFactoryCollection
    {
        T Build<T>() where T: class, IQueryFilterFactory;
    }
}