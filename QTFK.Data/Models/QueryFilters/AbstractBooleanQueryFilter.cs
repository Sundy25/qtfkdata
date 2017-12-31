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

        public virtual IDictionary<string, object> getParameters()
        {
            IEnumerable<KeyValuePair<string, object>> leftParameters, rightParameters;
            IDictionary<string, object> parameters;

            leftParameters = Left != null
                ? Left.getParameters()
                : Enumerable.Empty<KeyValuePair<string, object>>();
            rightParameters = Right != null
                ? Right.getParameters()
                : Enumerable.Empty<KeyValuePair<string, object>>();

            parameters = new Dictionary<string, object>();

            foreach (var item in leftParameters)
                parameters.Add(item);
            foreach (var item in rightParameters)
                parameters.Add(item);

            return parameters;
        }
    }
}
