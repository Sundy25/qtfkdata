using System;

namespace QTFK.Attributes
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