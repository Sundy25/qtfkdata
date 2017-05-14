using QTFK.Services;

namespace QTFK.Services
{
    public interface IAbstractEntityFactory
    {
        T Get<T>(); // where T: ICRUDRepository<TKey, TItem>;
    }
}