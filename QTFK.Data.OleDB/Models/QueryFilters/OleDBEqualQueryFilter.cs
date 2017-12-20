namespace QTFK.Models.QueryFilters
{
    public class OleDBEqualQueryFilter : IEqualQueryFilter
    {
        private string fieldName;
        private object value;



        public string Compile()
        {
            return $" ( [{this.fieldName}] = '{this.value}' ) ";
        }

        public void setFieldValue(string fieldName, object value)
        {
            Asserts.isFilled(fieldName, $"Parameter '{nameof(fieldName)}' cannot be null");

            this.fieldName = fieldName;
            this.value = value;
        }

        public void SetValues(params object[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}