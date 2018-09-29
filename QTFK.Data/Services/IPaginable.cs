using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IPaginable<T> : IEnumerable<T>
    {
        IPageView<T> paginate(int pageSize, int page);
        int Count { get; }
    }
}
