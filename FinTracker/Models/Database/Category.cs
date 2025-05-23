using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinTracker.Models.Database;

public partial class Category
{
    [Key]
    public int IdCategory { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? CategoryDescription { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
