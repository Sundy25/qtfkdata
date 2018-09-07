namespace QTFK.Services.DbFactory
{
    public interface IMetadataBuilder
    {
        DbMetadata scan<T>() where T : IDB;
    }
}