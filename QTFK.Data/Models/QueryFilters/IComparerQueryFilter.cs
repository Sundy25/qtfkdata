namespace QTFK.Models.QueryFilters
{
    public interface IComparerQueryFilter : IQueryFilter
    {
        void setFieldValue(string fieldName, object value);
    }
}