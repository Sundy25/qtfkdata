using System;

namespace QTFK.Data.Attributes
{
    public class ForeignAttribute : Attribute
    {
        public ForeignAttribute(string foreignTableColumnName)
        {
            this.ColumnName = foreignTableColumnName;
        }

        public string ColumnName { get; }
    }
}