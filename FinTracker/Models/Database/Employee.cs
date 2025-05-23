using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinTracker.Models.Database;

public partial class Employee
{
    [Key]
    public int IdEmployee { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string Position { get; set; } = null!;

    public decimal Salary { get; set; }

    public DateTime HireDate { get; set; }

    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();

}
