using System;

namespace QTFK.Data.Factory.Metadata
{
    public interface IEntityMetaData : IMetaData
    {
        Type InterfaceType { get; }
        string Table { get; }
        IColumnMetaData[] Columns { get; }
    }
}