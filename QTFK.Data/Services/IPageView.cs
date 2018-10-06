using System.Collections.Generic;

namespace QTFK.Services
{
    public interface IPageView<T> : IEnumerable<T>
    {
        int Count { get; }
    }
}