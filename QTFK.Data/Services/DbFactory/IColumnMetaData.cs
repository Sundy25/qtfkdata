using System;

namespace QTFK.Services.DbFactory
{
    public interface IColumnMetaData : IMetaData
    {
        string ColumnName { get; }
        Type ColumnType { get; }
        bool IsPrimaryKey { get; }
        bool IsForeignKey { get; }
        bool IsAlternativeKey { get; }
    }
}