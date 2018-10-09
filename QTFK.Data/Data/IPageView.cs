using System.Collections.Generic;

namespace QTFK.Data
{
    public interface IPageView<T> : IEnumerable<T>
    {
        int Count { get; }
    }
}