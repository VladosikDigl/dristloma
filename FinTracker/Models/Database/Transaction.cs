using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTracker.Models.Database;

public partial class Transaction
{
    [Key]
    public int IdTransaction { get; set; }

    public decimal Amount { get; set; }

    [Column(TypeName = "nvarchar(24)")]
    public TransactionType Type { get; set; }
    
    public int IdCategory { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? Description { get; set; }

    [ForeignKey("IdCategory")]
    public virtual Category Category { get; set; } = null!;
}
public enum TransactionType
{
    Income,
    ExpensePaid,
    ExpanseNotPaid
}
