using QTFK.Data.Factory.Metadata;

namespace QTFK.Data.Factory
{
    public interface IMetadataBuilder
    {
        IDbMetadata<T> scan<T>() where T : IDB;
    }
}