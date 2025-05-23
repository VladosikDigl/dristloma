using FinTracker.Models;
using FinTracker.Models.Database;
using FinTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FinTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly NotificationService _notificationService;


        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, NotificationService notificationService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _notificationService = notificationService;
        }
        public IActionResult test()
        {
            var model = new HomeViewModel
            {
                Reminders = new List<string>
            {
                "Собеседование в 15:00",
                "Оплатить интернет",
                "Купить продукты"
            },
                Buttons = new Dictionary<string, string>
            {
                { "Сохранить", "btn-primary" },
                { "Удалить", "btn-danger" },
                { "Экспорт", "btn-success" }
            },
                ChartData = new ChartData
                {
                    Labels = new List<string> { "Пн", "Вт", "Ср", "Чт", "Пт" },
                    Values = new List<int> { 12, 19, 3, 5, 8 },
                    BackgroundColor = "#cc65fe"
                }
            };

            return View(model);
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult MainWindow()
        {
            return View();
        }

        public IActionResult AccountantView()
        {
            var notifications = _notificationService.GetNotifications();

            return View(notifications);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Welcome", "Account");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}