using System;

namespace QTFK.Services.DbFactory
{
    public interface IEntityMetaData : IMetaData
    {
        Type InterfaceType { get; }
        string Table { get; }
        IColumnMetaData[] Columns { get; }
        IRelationShipMetaData[] RelationShips { get; }
    }
}