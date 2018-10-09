using System;

namespace QTFK.Data.Attributes
{
    public enum ColumnOrder
    {
        Ascendant,
        Descendant,
    }

    public class DefaulOrderAttribute : Attribute
    {
        public DefaulOrderAttribute() : this(ColumnOrder.Ascendant)
        {
        }

        public DefaulOrderAttribute(ColumnOrder order)
        {
            this.Order = order;
        }

        public ColumnOrder Order { get; }
    }
}