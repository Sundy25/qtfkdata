using System;

namespace QTFK.Services.DbFactory
{
    public interface IEntityMetaData
    {
        string Name { get; }
        Type InterfaceType { get; }
        string Table { get; }
        IColumnMetaData[] Columns { get; }
    }
}