using System.Collections.Generic;
using QTFK.Services;

namespace QTFK.Models.QueryFilters
{
    public class OleDBEqualQueryFilter : IEqualQueryFilter
    {
        private SetColumn field;
        private readonly IParameterBuilder parameterBuilder;

        public OleDBEqualQueryFilter(IParameterBuilder parameterBuilder)
        {
            this.field = new SetColumn();
            this.parameterBuilder = parameterBuilder;
        }

        public string Compile()
        {
            return $" ( [{this.field.Name}] = {this.field.Parameter} ) ";
        }

        public IEnumerable<QueryParameter> getParameters()
        {
            yield return new QueryParameter
            {
                Parameter = this.field.Parameter,
                Value = this.field.Value,
            };
        }

        public void setFieldValue(string fieldName, object value)
        {
            Asserts.isFilled(fieldName, $"Parameter '{nameof(fieldName)}' cannot be null");

            this.field.Name = fieldName;
            this.field.Value = value;
            this.field.Parameter = this.parameterBuilder.buildParameter(fieldName);
        }
    }
}