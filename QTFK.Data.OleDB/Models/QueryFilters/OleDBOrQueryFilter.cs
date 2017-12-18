namespace QTFK.Models.QueryFilters
{
    public class OleDBOrQueryFilter : IOrQueryFilter
    {
        public IQueryFilter Left { get; set; }
        public IQueryFilter Right { get; set; }

        public string Compile()
        {
            return $" ( {Left.Compile()} OR {Right.Compile()} ) ";
        }

        public void SetValues(params object[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}