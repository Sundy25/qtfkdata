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

        public void SetValues(params object[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}