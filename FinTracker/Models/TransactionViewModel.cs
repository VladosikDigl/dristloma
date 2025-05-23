using FinTracker.Models.Database;
using System.Transactions;

namespace FinTracker.Models
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
    }

}
