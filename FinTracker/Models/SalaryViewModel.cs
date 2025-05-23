using FinTracker.Models.Database;

namespace FinTracker.Models
{
    public class SalaryViewModel
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } 
        public DateTime Date { get; set; }       
        public decimal Amount { get; set; }     
        public string Type { get; set; }
        public decimal NDFL { get; set; }
        public bool IsPaid { get; set; }
    }

}
