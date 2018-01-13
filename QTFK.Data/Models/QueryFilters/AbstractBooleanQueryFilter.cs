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

        public FilterCompilation Compile(IParameterBuilder parameterBuilder)
        {
            Asserts.isSomething(this.Left, $"Property '{nameof(this.Left)}' cannot be null.");
            Asserts.isSomething(this.Right, $"Property '{nameof(this.Right)}' cannot be null.");

            FilterCompilation filterCompilation;
            FilterCompilation leftFilterCompilation, rightFilterCompilation;
            string segment;
            IEnumerable<QueryParameter> queryParameters;

            leftFilterCompilation = this.Left.Compile(parameterBuilder);
            rightFilterCompilation = this.Right.Compile(parameterBuilder);
            segment = prv_buildBooleanSegment(leftFilterCompilation.FilterSegment, rightFilterCompilation.FilterSegment);
            queryParameters = leftFilterCompilation.Parameters.Concat(rightFilterCompilation.Parameters);
            filterCompilation = new FilterCompilation(segment, queryParameters);

            return filterCompilation;
        }

        protected abstract string prv_buildBooleanSegment(string left, string right);
    }
}
