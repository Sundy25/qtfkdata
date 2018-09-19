namespace QTFK.Services.DbFactory
{
    public class DefaultMetadataBuilder : IMetadataBuilder
    {
        public IDbMetadata<T> scan<T>() where T : IDB
        {
            throw new System.NotImplementedException();
        }
    }
}