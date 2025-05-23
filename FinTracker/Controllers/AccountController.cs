using FinTracker.Models;
using FinTracker.Models.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly FintrackContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(ILogger<AccountController> logger, FintrackContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Welcome()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                Position = model.Position,
                Salary = model.Salary,
                HireDate = DateTime.Now
            };

            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при добавлении сотрудника: " + ex.Message);
                ModelState.AddModelError("", "Ошибка при добавлении сотрудника.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                EmployeeId = employee.IdEmployee
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Employee");

                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Ошибка при назначении роли пользователю.");

                    await _userManager.DeleteAsync(user);
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();

                    foreach (var error in roleResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("MainWindow", "Home");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                _logger.LogError("Ошибка при создании пользователя: " + error.Description);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login (string login, string password)
        {
            var user = await _userManager.FindByNameAsync(login);

            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь не найден");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("MainWindow", "Home");
            }

            ModelState.AddModelError("", "Неверный пароль");
            return View();
        }
    }
}
