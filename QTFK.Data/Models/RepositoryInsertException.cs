using System;
using System.Runtime.Serialization;

namespace QTFK.Models
{
    [Serializable]
    public class RepositoryInsertException : Exception
    {
        public RepositoryInsertException()
        {
        }

        public RepositoryInsertException(IDBQueryInsert q)
        {
            Query = q;
        }

        public RepositoryInsertException(string message) : base(message)
        {
        }

        public RepositoryInsertException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RepositoryInsertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IDBQueryInsert Query { get; }
    }
}