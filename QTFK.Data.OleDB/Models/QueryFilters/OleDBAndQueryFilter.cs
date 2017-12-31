using System.Collections.Generic;
using System.Linq;
using QTFK.Services;

namespace QTFK.Models.QueryFilters
{
    public class OleDBAndQueryFilter : IAndQueryFilter
    {
        public IQueryFilter Left { get; set; }
        public IQueryFilter Right { get; set; }

        public string Compile()
        {
            return $" ( {Left.Compile()} AND {Right.Compile()} ) ";
        }

        public IDictionary<string, object> getParameters()
        {
            return Left.getParameters()
                .Concat(Right.getParameters())
                .ToDictionary(item => item.Key, item => item.Value);
        }

        public void setParameterBuilder(IParameterBuilder parameterBuilder)
        {
            Left.setParameterBuilder(parameterBuilder);
            Right.setParameterBuilder(parameterBuilder);
        }
    }
}