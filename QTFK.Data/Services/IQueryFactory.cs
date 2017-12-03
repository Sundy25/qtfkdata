using QTFK.Models;

namespace QTFK.Services
{
    public interface IQueryFactory<T> : IQueryFactory, IQueryFilterFactoryCollection
        where T : new()
    {
        EntityDescription EntityDescription { get; }
    }
}
