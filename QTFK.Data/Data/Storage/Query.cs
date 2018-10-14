using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace QTFK.Data.Storage
{
    public class Query
    {
        private string statement;

        private void prv_setStatement(string value)
        {
            Asserts.isFilled(value, $"Value for '{nameof(Statement)}' property cannot be empty.");
            this.statement = value;
        }

        public static Query Empty { get { return new Query(); } }

        private Query()
        {
        }

        public Query(string query)
        {
            prv_setStatement(query);
            this.Parameters = new Dictionary<string, object>();
        }

        public Query(Query innerQuery, Func<string, string> outerStatement)
        {
            string completeStatement, innerStatement;

            innerStatement = innerQuery.Statement;
            completeStatement = outerStatement(innerStatement);
            prv_setStatement(completeStatement);

            foreach (KeyValuePair<string, object> parameter in innerQuery.Parameters)
                this.Parameters.Add(parameter);
        }

        public string Statement
        {
            get { return this.statement; }
            set { prv_setStatement(value); }
        }

        public IDictionary<string, object> Parameters { get; }

        public static implicit operator Query(string query)
        {
            return new Query(query);
        }

        public static Query operator +(Query leftQuery, Query rightQuery)
        {
            Query result;

            result = $"{leftQuery.Statement} {rightQuery.Statement}";

            foreach (KeyValuePair<string, object> parameter in leftQuery.Parameters)
                result.Parameters.Add(parameter);

            foreach (KeyValuePair<string, object> parameter in rightQuery.Parameters)
                result.Parameters.Add(parameter);

            return result;
        }
    }
}