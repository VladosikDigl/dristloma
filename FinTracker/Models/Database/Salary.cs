using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTracker.Models.Database;

public partial class Salary
{
    [Key]
    public int IdSalary { get; set; }

    public int IdEmploee { get; set; }

    public DateTime SalaryDate { get; set; }

    public decimal Amount { get; set; }

    [Column(TypeName = "nvarchar(24)")]
    public SalaryType Type { get; set; }

    [Column(TypeName = "nvarchar(24)")]
    public PaymentStatus PaymentStatus { get; set; }

    [ForeignKey("IdEmploee")]
    public virtual Employee Employee { get; set; } = null!;
}
public enum SalaryType
{
    Final,
    Advance
}
public enum PaymentStatus
{
    NotPaid,
    Paid
}