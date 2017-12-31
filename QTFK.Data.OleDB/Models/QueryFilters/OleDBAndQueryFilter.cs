using System.Collections.Generic;
using System.Linq;
using QTFK.Services;

namespace QTFK.Models.QueryFilters
{
    public class OleDBAndQueryFilter : AbstractBooleanQueryFilter,  IAndQueryFilter
    {
        public override string Compile()
        {
            return $" ( {Left.Compile()} AND {Right.Compile()} ) ";
        }
    }
}