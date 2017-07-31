using System;
using System.Runtime.Serialization;

namespace QTFK.Models
{
    [Serializable]
    public class QueryFilterNotFoundException : Exception
    {
        public QueryFilterNotFoundException()
        {
        }

        public QueryFilterNotFoundException(string message) : base(message)
        {
        }

        public QueryFilterNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected QueryFilterNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}