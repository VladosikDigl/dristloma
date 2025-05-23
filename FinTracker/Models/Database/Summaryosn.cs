using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTracker.Models.Database;

public partial class Summaryosn
{
    [Key]
    public int IdOsn { get; set; }

    public decimal Revenues { get; set; }

    public decimal AllExpenses { get; set; }

    public decimal MaterialExpenses { get; set; }

    public decimal AccruedNds { get; set; }

    public decimal InputNds { get; set; }

    public int PercentageNds { get; set; }

    public decimal PropertyTax { get; set; }

    public decimal TotalOsn { get; set; }

    [Column(TypeName = "date")]
    public DateTime Month { get; set; }
}
