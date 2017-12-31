using System.Collections.Generic;
using System.Linq;
using QTFK.Services;

namespace QTFK.Models.QueryFilters
{
    public class OleDBOrQueryFilter : AbstractBooleanQueryFilter, IOrQueryFilter
    {
        public override string Compile()
        {
            return $" ( {Left.Compile()} OR {Right.Compile()} ) ";
        }
    }
}