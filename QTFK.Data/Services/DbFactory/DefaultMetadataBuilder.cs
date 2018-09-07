namespace QTFK.Services.DbFactory
{
    public class DefaultMetadataBuilder : IMetadataBuilder
    {
        public DbMetadata scan<TDB>() where TDB : IDB
        {
            throw new System.NotImplementedException();
        }
    }
}