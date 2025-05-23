using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinTracker.Models
{
    public class FormASalaryViewModel
    {
        public int? SelectedSalaryId { get; set; }
        
        public List<SelectListItem> SalaryList { get; set; } = new();

        public List<SalaryViewModel> Salaries { get; set; } = new();
    }

}
