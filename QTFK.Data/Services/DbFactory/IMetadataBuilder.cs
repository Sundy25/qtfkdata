namespace QTFK.Services.DbFactory
{
    public interface IMetadataBuilder
    {
        IDbMetadata scan<T>() where T : IDB;
    }
}