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

        public IDictionary<string, object> getParameters()
        {
            IDictionary<string, object> parameters;

            parameters = new Dictionary<string, object>
            {
                {
                    this.field.Name,
                    new SetColumn
                    {
                        Name = this.field.Name,
                        Value = this.field.Value,
                        Parameter = this.field.Parameter,
                    }
                }
            };

            return parameters;
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