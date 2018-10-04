using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IView<T> : IEnumerable<T> where T: IEntity
    {
        IPageView<T>[] getPages(int pageSize);
        int Count { get; }
    }
}
