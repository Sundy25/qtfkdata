namespace QTFK.Services.DbFactory
{
    public class DefaultMetadataBuilder : IMetadataBuilder
    {
        public IDbMetadata scan<TDB>() where TDB : IDB
        {
            throw new System.NotImplementedException();
        }
    }
}