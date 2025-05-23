using FinTracker.Models.Database;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinTracker.Models
{
    public class AddTransactionViewModel
    {
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }


        public List<SelectListItem> CategoryList { get; set; } = new();
        public List<TransactionViewModel> Transactions { get; set; } = new();
    }


}
