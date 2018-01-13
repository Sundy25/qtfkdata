using System.Collections.Generic;
using System.Linq;
using QTFK.Services;

namespace QTFK.Models.QueryFilters
{
    public class OleDBOrQueryFilter : AbstractBooleanQueryFilter, IOrQueryFilter
    {
        protected override string prv_buildBooleanSegment(string left, string right)
        {
            return $" ( {left} OR {right} ) ";
        }
    }
}