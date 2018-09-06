using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IPageView<T> : IEnumerable<T>
    {
        int PagesCount { get; }
        int CurrentPage { get; }
        int PageSize { get; }
    }
}