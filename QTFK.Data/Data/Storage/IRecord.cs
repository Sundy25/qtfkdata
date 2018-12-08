namespace QTFK.Data.Storage
{
    public interface IRecord
    {
        T get<T>(string fieldName);
        T get<T>(int i);
        int FieldCount { get; }
    }
}