namespace QTFK.Models.QueryFilters
{
    public interface IBooleanQueryFilter : IQueryFilter
    {
        IQueryFilter Left { get; set; }
        IQueryFilter Right { get; set; }
    }
}