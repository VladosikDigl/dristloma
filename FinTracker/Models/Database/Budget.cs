using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinTracker.Models.Database;

public partial class Budget
{
    [Key]
    public int IdBudget { get; set; }
    public decimal BudgetAmount { get; set; }

}
