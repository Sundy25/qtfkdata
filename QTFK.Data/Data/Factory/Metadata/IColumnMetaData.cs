using System;

namespace QTFK.Data.Factory.Metadata
{
    public interface IColumnMetaData : IMetaData
    {
        string ColumnName { get; }
        Type ColumnType { get; }
        bool IsPrimaryKey { get; }
        bool IsForeignKey { get; }
        bool IsAlternativeKey { get; }
        bool IsHidden { get; }
    }
}