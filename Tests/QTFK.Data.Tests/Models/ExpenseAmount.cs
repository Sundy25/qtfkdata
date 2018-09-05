namespace QTFK.Data.Tests.Models
{
    public class ExpenseAmount
    {
        public int Id { get; set; }
        public string Concept { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalContributors { get; set; }
    }
}