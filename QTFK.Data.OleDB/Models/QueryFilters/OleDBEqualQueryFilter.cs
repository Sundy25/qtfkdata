using System.Collections.Generic;
using QTFK.Services;

namespace QTFK.Models.QueryFilters
{
    public class OleDBEqualQueryFilter : AbstractComparerQueryFilter, IEqualQueryFilter
    {
        protected override string prv_buildComparerSegment(string fieldName, string parameter)
        {
            return $" ( [{fieldName}] = {parameter} ) ";
        }
    }
}