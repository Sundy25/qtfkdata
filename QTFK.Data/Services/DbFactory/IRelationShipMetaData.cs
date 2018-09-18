namespace QTFK.Services.DbFactory
{
    public interface IRelationShipMetaData : IMetaData
    {
        RelationShipKind Kind { get; }
    }
}