namespace QTFK.Services.DbFactory
{
    public interface IMetadataBuilder
    {
        IDbMetadata<T> scan<T>() where T : IDB;
    }
}