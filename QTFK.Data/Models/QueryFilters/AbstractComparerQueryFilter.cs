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
            string segment;
            QueryParameter filterParameter;

            Asserts.isFilled(this.fieldName, $"Parameter '{nameof(this.fieldName)}' cannot be null");

            filterParameter = new QueryParameter()
            {
                Parameter = parameterBuilder.buildParameter(this.fieldName),
                Value = this.fieldValue,
            };
            segment = prv_buildComparerSegment(this.fieldName, filterParameter.Parameter);
            result = new FilterCompilation(segment, new QueryParameter[] { filterParameter });

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
