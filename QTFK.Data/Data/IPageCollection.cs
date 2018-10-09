namespace QTFK.Data
{
    public interface IPageCollection<T> where T : IEntity
    {
        int Count { get; }
        IPageView<T> this[int index] { get; }
    }
}