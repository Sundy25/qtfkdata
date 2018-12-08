using System.Collections.Generic;

namespace QTFK.Data
{
    public interface IView<T> : IEnumerable<T> where T: IEntity
    {
        IPageCollection<T> getPages(int pageSize);
        int Count { get; }
    }
}
