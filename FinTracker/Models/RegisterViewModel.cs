using System.ComponentModel.DataAnnotations;

namespace FinTracker.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? MiddleName { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Зарплата не может быть отрицательной")]
        public decimal Salary { get; set; }

        //[Required]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }
    }
}
