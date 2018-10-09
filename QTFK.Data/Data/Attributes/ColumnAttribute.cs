using System;

namespace QTFK.Data.Attributes
{
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }

        public string ColumnName { get; }
    }
}