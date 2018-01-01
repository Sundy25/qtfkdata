using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Services;

namespace QTFK.Models.QueryFilters
{
    public abstract class AbstractBooleanQueryFilter : IBooleanQueryFilter
    {
        public IQueryFilter Left { get; set; }
        public IQueryFilter Right { get; set; }

        public abstract string Compile();

        public virtual IEnumerable<QueryParameter> getParameters()
        {
            if(Left != null)
                foreach (var item in Left.getParameters())
                    yield return item;

            if (Right != null)
                foreach (var item in Right.getParameters())
                    yield return item;
        }
    }
}
