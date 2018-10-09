using System.Collections.Generic;

namespace QTFK.Data.Storage
{
    public class Query
    {
        private string instruction;

        private void prv_setInstruction(string value)
        {
            Asserts.isFilled(value, $"Value for '{nameof(Instruction)}' property cannot be empty.");
            this.instruction = value;
        }

        public Query(string query)
        {
            prv_setInstruction(query);
            this.Parameters = new Dictionary<string, object>();
        }

        public string Instruction
        {
            get => this.instruction;
            set => prv_setInstruction(value);
        }

        public IDictionary<string, object> Parameters { get; }

        public static implicit operator Query(string query)
        {
            return new Query(query);
        }
    }
}