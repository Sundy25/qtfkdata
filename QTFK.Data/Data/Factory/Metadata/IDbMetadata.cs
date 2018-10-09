namespace QTFK.Data.Factory.Metadata
{
    public interface IDbMetadata<T> : IClassMetaData where T : IDB
    {
        IEntityMetaData[] Entities { get; }
        IViewMetaData[] Views { get; }
    }
}