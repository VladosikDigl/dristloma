using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTracker.Models.Database
{
    public class BudgetSnapshots
    {
        [Key]
        public int Id { get; set; }
        public decimal Balance { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

    }
}
