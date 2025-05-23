using FinTracker.Models;
using FinTracker.Models.Database;
using FinTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Controllers
{
    [Authorize(Roles = "Accountant")]
    public class AccountantController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly FintrackContext _context;
        private readonly BalanceService _balanceService;
        

        public AccountantController(ILogger<AccountController> logger, FintrackContext context, BalanceService balanceService)
        {
            _logger = logger;
            _context = context;
            _balanceService = balanceService;
        }

        public IActionResult AddEmployee()
        {
            return View();
        }

        public IActionResult AddTransaction()
        {
            var viewModel = new AddTransactionViewModel
            {
                CategoryList = _context.Categories
                    .Select(c => new SelectListItem { Value = c.IdCategory.ToString(), Text = c.CategoryName })
                    .ToList(),

                Transactions = _context.Transactions
                    .Include(t => t.Category)
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => new TransactionViewModel
                    {
                        Id = t.IdTransaction,
                        Amount = t.Amount,
                        Type = TransactionType.ExpensePaid,
                        CategoryName = t.Category.CategoryName,
                        Date = t.TransactionDate,
                        Description = t.Description
                    }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction(AddTransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryList = _context.Categories
                    .Select(c => new SelectListItem { Value = c.IdCategory.ToString(), Text = c.CategoryName })
                    .ToList();

                model.Transactions = _context.Transactions
                    .Include(t => t.Category)
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => new TransactionViewModel
                    {
                        Id = t.IdTransaction,
                        Amount = t.Amount,
                        Type = t.Type,
                        CategoryName = t.Category.CategoryName,
                        Date = t.TransactionDate,
                        Description = t.Description
                    }).ToList();

                return View(model);
            }

            var transaction = new Transaction
            {
                Amount = model.Amount,
                Type = model.Type,
                IdCategory = model.CategoryId,
                Description = model.Description,
                TransactionDate = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            await _balanceService.UpdateBalanceAsync();

            return RedirectToAction(nameof(AddTransaction));
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string categoryName, string categoryDescription)
        {
            var category = new Category
            {
                CategoryName = categoryName,
                CategoryDescription = categoryDescription
            };

            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при добавлении сотрудника: " + ex.Message);
                ModelState.AddModelError("", "Ошибка при добавлении сотрудника.");
                return View("AddTransaction");
            }

            TempData["Success"] = "Категория добавлена успешно!";

            return RedirectToAction("AddTransaction");
        }

        [HttpGet]
        public async Task<IActionResult> FormASalary()
        {
            var salaries = await _context.Salaries
                .Include(s => s.Employee)
                .OrderByDescending(s => s.SalaryDate)
                .ToListAsync();

            var unpaidList = salaries
                .Where(s => s.PaymentStatus != PaymentStatus.Paid)
                .Select(s => new SelectListItem(
                    text: $"{s.Employee.LastName} {s.Employee.FirstName} — {s.Amount:C} от {s.SalaryDate:dd.MM.yyyy}",
                    value: s.IdSalary.ToString()))
                .ToList();

            var vmList = salaries.Select(s => new SalaryViewModel
            {
                Id = s.IdSalary,
                EmployeeName = s.Employee.LastName + " " + s.Employee.FirstName + " " + s.Employee.MiddleName,
                Date = s.SalaryDate,
                Amount = s.Amount,
                Type = s.Type == SalaryType.Advance ? "Аванс" : "Окончательный расчет",
                NDFL = s.Type == SalaryType.Advance ? 0 : s.Amount * 0.13m,
                IsPaid = s.PaymentStatus == PaymentStatus.Paid
            }).ToList();

            var vm = new FormASalaryViewModel
            {
                SalaryList = unpaidList,
                Salaries = vmList
            };

            return View(vm);
        }


        [HttpPost]
        public IActionResult CalculateSalary(SalaryType salaryType)
        {
            var employees = _context.Employees.ToList();
            var today = DateTime.Today;
            var currentMonth = today.Month;
            var currentYear = today.Year;

            const decimal ndflRate = 0.13m;

            foreach (var emp in employees)
            {
                bool alreadyExists = _context.Salaries.Any(s =>
                    s.IdEmploee == emp.IdEmployee &&
                    s.SalaryDate.Month == currentMonth &&
                    s.SalaryDate.Year == currentYear &&
                    (
                        (salaryType == SalaryType.Advance && s.Amount == Math.Round(emp.Salary / 2, 2)) ||
                        (salaryType == SalaryType.Final && s.Amount == Math.Round((emp.Salary / 2) * (1 - ndflRate), 2))
                    )
                );

                if (alreadyExists)
                    continue;

                var gross = emp.Salary;
                decimal amount = 0;

                if (salaryType == SalaryType.Advance)
                {
                    amount = Math.Round(gross / 2, 2);
                }
                else if (salaryType == SalaryType.Final)
                {
                    var half = gross / 2;
                    var ndfl = Math.Round(half * ndflRate, 2);
                    amount = Math.Round(half - ndfl, 2);
                }

                var salary = new Salary
                {
                    IdEmploee = emp.IdEmployee,
                    SalaryDate = today,
                    Amount = amount,
                    Type = salaryType,
                    PaymentStatus = PaymentStatus.NotPaid
                };

                _context.Salaries.Add(salary);
            }

            _context.SaveChanges();
            return RedirectToAction("FormASalary");
        }


        [HttpPost]
        public async Task<IActionResult> MarkSalaryAsPaid(FormASalaryViewModel model)
        {

            var salary = await _context.Salaries
                .Include(s => s.Employee).Where(s => s.PaymentStatus == PaymentStatus.NotPaid).ToListAsync();

            if (!salary.Any())
            {
                TempData["Message"] = "Нет записей для оплаты.";
                return RedirectToAction(nameof(FormASalary));
            }

            const decimal insuranceRate = 0.30m;

            decimal salaryAmount = 0;
            decimal insuranceAmount = 0;

            foreach (var sal in salary)
            {
                sal.PaymentStatus = PaymentStatus.Paid;
                _context.Salaries.Update(sal);

                salaryAmount += sal.Amount;
                if (sal.Type == SalaryType.Final)
                    insuranceAmount += sal.Employee.Salary * insuranceRate;
            }

            var salaryCategorySal = _context.Categories
                .FirstOrDefault(c => c.CategoryName == "Выплата ЗП");

            var salaryCategoryIns = _context.Categories
                .FirstOrDefault(c => c.CategoryName == "Страховые взносы");

            var transactionSalary = new Transaction
            {
                Amount = salaryAmount,
                Type = TransactionType.ExpensePaid,
                IdCategory = salaryCategorySal.IdCategory,
                TransactionDate = DateTime.Now,
                Description = "Выплата заработной платы сотрудникам."
            };
          
            var transactionInsurance = new Transaction
            {
                Amount = insuranceAmount,
                Type = TransactionType.ExpensePaid,
                IdCategory = salaryCategoryIns.IdCategory,
                TransactionDate = DateTime.Now,
                Description = "Вычет страховых взносов."
            };

            try
            {
                _context.Transactions.Add(transactionSalary);
                if (insuranceAmount != 0)
                    _context.Transactions.Add(transactionInsurance);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при добавлении записи: " + ex.Message);
                ModelState.AddModelError("", "Ошибка при добавлении записи.");
                return RedirectToAction(nameof(FormASalary), model);
            }

            await _balanceService.UpdateBalanceAsync();

            return RedirectToAction(nameof(FormASalary));
        }
        [HttpGet]
        public async Task<IActionResult> ReportTaxes()
        {
            var now = DateTime.Now;
            int currentQuarter = (now.Month - 1) / 3 + 1;

            var model = new TaxReportViewModel
            {
                Year = now.Year,
                Quarter = currentQuarter,
                TaxSystem = "УСН",
                UsnRateOption = "6",
                PropertyTaxApplied = false,
                PropertyCadCost = 0,
                PropertyRate = 2.0m
            };

            // Получаем даты начала и конца квартала
            DateTime startDate = new DateTime(model.Year, (model.Quarter - 1) * 3 + 1, 1);
            DateTime endDate = startDate.AddMonths(3).AddDays(-1);

            // Получаем транзакции за период
            var incomes = await _context.Transactions
                .Where(t => t.Type == TransactionType.Income && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Select(t => new IncomeEntry
                {
                    Id = t.IdTransaction,
                    Date = t.TransactionDate,
                    Amount = t.Amount,
                    Description = t.Description
                })
                .ToListAsync();

            var expenses = await _context.Transactions
                .Where(t => t.Type == TransactionType.ExpensePaid && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Select(t => new ExpenseEntry
                {
                    Id = t.IdTransaction,
                    Date = t.TransactionDate,
                    Amount = t.Amount,
                    Description = t.Description
                })
                .ToListAsync();

            model.Incomes = incomes;
            model.Expenses = expenses;

            model.SelectedIncomeIds = incomes.Select(i => i.Id).ToList();
            model.SelectedExpenseIds = expenses.Select(e => e.Id).ToList();

            model.IncomeVatRates = incomes.ToDictionary(i => i.Id, i => 20); // по умолчанию 20%

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ReportTaxes(TaxReportViewModel model)
        {
            var startDate = new DateTime(model.Year, (model.Quarter - 1) * 3 + 1, 1);
            var endDate = startDate.AddMonths(3).AddDays(-1);

            // Загрузка доходов и расходов за период
            var incomes = await _context.Transactions
                .Where(t => t.Type == TransactionType.Income && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Select(t => new IncomeEntry
                {
                    Id = t.IdTransaction,
                    Date = t.TransactionDate,
                    Amount = t.Amount,
                    Description = t.Description
                })
                .ToListAsync();

            var expenses = await _context.Transactions
                .Where(t => t.Type == TransactionType.ExpensePaid && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Select(t => new ExpenseEntry
                {
                    Id = t.IdTransaction,
                    Date = t.TransactionDate,
                    Amount = t.Amount,
                    Description = t.Description
                })
                .ToListAsync();

            model.Incomes = incomes;
            model.Expenses = expenses;

            // Если пользователь не выбрал вручную — выбираем все
            if (model.SelectedIncomeIds == null || !model.SelectedIncomeIds.Any())
                model.SelectedIncomeIds = incomes.Select(i => i.Id).ToList();

            if (model.SelectedExpenseIds == null || !model.SelectedExpenseIds.Any())
                model.SelectedExpenseIds = expenses.Select(e => e.Id).ToList();

            model.IncomeVatRates = incomes.ToDictionary(i => i.Id, i => 20); // по умолчанию 20%

            // Расчёт налогов
            if (model.TaxSystem == "OSNO")
            {
                // Пример расчета
                var selectedIncome = incomes.Where(i => model.SelectedIncomeIds.Contains(i.Id)).Sum(i => i.Amount);
                var selectedExpense = expenses.Where(e => model.SelectedExpenseIds.Contains(e.Id)).Sum(e => e.Amount);

                model.TaxAmount = selectedIncome * 0.2m; // Пример: налог на прибыль
                model.NdflAmount = selectedExpense * 0.13m; // Пример: НДФЛ
            }
            else if (model.TaxSystem == "USN")
            {
                var selectedIncome = incomes.Where(i => model.SelectedIncomeIds.Contains(i.Id)).Sum(i => i.Amount);
                var selectedExpense = expenses.Where(e => model.SelectedExpenseIds.Contains(e.Id)).Sum(e => e.Amount);

                if (model.UsnRateOption == "6")
                {
                    model.TaxAmount = selectedIncome * 0.06m;
                }
                else if (model.UsnRateOption == "15")
                {
                    model.TaxAmount = (selectedIncome - selectedExpense) * 0.15m;
                }
            }

            if (model.PropertyTaxApplied && model.PropertyCadCost > 0)
            {
                model.PropertyTaxAmount = model.PropertyCadCost * model.PropertyRate / 100m;
            }

            model.IsCalculated = true;

            return View(model);
        }
        public async Task<IActionResult> LoadTaxPartial(string system, string rate, int quarter)
        {
            var now = DateTime.Now;

            // Валидация квартала
            if (quarter < 1 || quarter > 4)
            {
                quarter = (now.Month - 1) / 3 + 1; // fallback на текущий
            }

            var model = new TaxReportViewModel
            {
                Year = now.Year,
                Quarter = quarter, // Используем переданный квартал
                TaxSystem = system,
                UsnRateOption = rate,
                PropertyTaxApplied = false,
                PropertyCadCost = 0,
                PropertyRate = 2.0m
            };

            // Расчет периода для выбранного квартала
            DateTime startDate = new DateTime(model.Year, (model.Quarter - 1) * 3 + 1, 1);
            DateTime endDate = startDate.AddMonths(3).AddDays(-1);

            model.Incomes = await _context.Transactions
                .Where(t => t.Type == TransactionType.Income
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .Select(t => new IncomeEntry
                {
                    Id = t.IdTransaction,
                    Date = t.TransactionDate,
                    Amount = t.Amount,
                    Description = t.Description
                }).ToListAsync();

            model.Expenses = await _context.Transactions
                .Where(t => t.Type == TransactionType.ExpensePaid
                    && t.TransactionDate >= startDate
                    && t.TransactionDate <= endDate)
                .Select(t => new ExpenseEntry
                {
                    Id = t.IdTransaction,
                    Date = t.TransactionDate,
                    Amount = t.Amount,
                    Description = t.Description
                }).ToListAsync();

            if (system == "OSNO")
                return PartialView("ReportTaxes_OSNO", model);
            if (system == "USN" && (rate == "6" || rate == "15"))
                return PartialView("ReportTaxes_USN", model);

            return Content("");
        }


    }
}