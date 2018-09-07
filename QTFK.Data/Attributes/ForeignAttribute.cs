using System;

namespace QTFK.Attributes
{
    public class ForeignAttribute : Attribute
    {
        public ForeignAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }

        public string ColumnName { get; }
    }
}