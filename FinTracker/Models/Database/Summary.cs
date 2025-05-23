using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinTracker.Models.Database;

public partial class Summary
{
    [Key]
    public int IdSummary { get; set; }

    public decimal Revenues { get; set; }

    public decimal Expenses { get; set; }

    public decimal Usn6 { get; set; }

    public decimal Usn15 { get; set; }

    public decimal NetIncome { get; set; }
}
