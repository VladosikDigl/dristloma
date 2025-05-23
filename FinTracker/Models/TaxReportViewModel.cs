using System.ComponentModel.DataAnnotations;

namespace FinTracker.Models
{
    public class TaxReportViewModel
    {
        // --- Входные параметры ---
        [Required]
        [Display(Name = "Система налогообложения")]
        public string TaxSystem { get; set; } = "";

        [Display(Name = "Ставка УСН")]
        public string? UsnRateOption { get; set; }      // "6" или "15"

        [Range(2000, 2100), Display(Name = "Год")]
        public int Year { get; set; }                  // для периода

        [Required(ErrorMessage = "Выберите квартал")]
        [Range(1, 4, ErrorMessage = "Некорректный квартал"), Display(Name = "Квартал")]
        public int Quarter { get; set; }

        // Списки операций за выбранный период
        public List<IncomeEntry> Incomes { get; set; } = new();
        public List<ExpenseEntry> Expenses { get; set; } = new();

        // Идентификаторы выбранных строк (биндятся с чекбоксами)
        public List<int> SelectedIncomeIds { get; set; } = new();
        public List<int> SelectedExpenseIds { get; set; } = new();

        // НДС по каждой доходной строчке: ключ — Id операции, значение — ставка (0, 10 или 20)
        public Dictionary<int, int> IncomeVatRates { get; set; } = new();

        // Имущественный налог
        [Display(Name = "Применить имущественный налог")]
        public bool PropertyTaxApplied { get; set; }

        [Display(Name = "Кадастровая стоимость")]
        public decimal PropertyCadCost { get; set; }

        [Display(Name = "Ставка имущественного налога (%)")]
        public decimal PropertyRate { get; set; }

        // --- Результаты расчёта ---
        [Display(Name = "Сумма налога")]
        public decimal TaxAmount { get; set; }

        [Display(Name = "НДФЛ с зарплат")]
        public decimal NdflAmount { get; set; }

        [Display(Name = "Сумма имущественного налога")]
        public decimal PropertyTaxAmount { get; set; }

        public bool IsCalculated { get; set; } = false;
    }
    
    // Каждая доходная операция в таблице
    public class IncomeEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
    }

    // Каждая расходная операция в таблице
    public class ExpenseEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
    }

}
