using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Services;

namespace QTFK.Models.QueryFilters
{
    public abstract class AbstractComparerQueryFilter : IComparerQueryFilter
    {
        protected string fieldName;
        protected object fieldValue;

        public FilterCompilation Compile(IParameterBuilder parameterBuilder)
        {
            FilterCompilation result;
            string segment, parameterName;
            KeyValuePair<string, object> filterParameter;
            IEnumerable<KeyValuePair<string, object>> parameters;

            Asserts.isFilled(this.fieldName, $"Parameter '{nameof(this.fieldName)}' cannot be null");

            parameterName = parameterBuilder.buildParameter(this.fieldName);
            filterParameter = new KeyValuePair<string, object>(parameterName, this.fieldValue);
            segment = prv_buildComparerSegment(this.fieldName, filterParameter.Key);
            parameters = new KeyValuePair<string, object>[] { filterParameter };
            result = new FilterCompilation(segment, parameters);

            return result;
        }

        public void setFieldValue(string fieldName, object value)
        {
            Asserts.isFilled(fieldName, $"Parameter '{nameof(fieldName)}' cannot be null");

            this.fieldName = fieldName;
            this.fieldValue = value;
        }

        protected abstract string prv_buildComparerSegment(string fieldName, string parameter);
    }
}
