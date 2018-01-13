using System.Collections.Generic;
using System.Linq;
using QTFK.Services;

namespace QTFK.Models.QueryFilters
{
    public class OleDBAndQueryFilter : AbstractBooleanQueryFilter, IAndQueryFilter
    {
        protected override string prv_buildBooleanSegment(string left, string right)
        {
            return $" ( {left} AND {right} ) ";
        }
    }
}