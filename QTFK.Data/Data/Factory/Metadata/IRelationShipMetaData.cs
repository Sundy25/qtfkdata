namespace QTFK.Data.Factory.Metadata
{
    public interface IRelationShipMetaData : IMetaData
    {
        RelationShipKind Kind { get; }
    }
}