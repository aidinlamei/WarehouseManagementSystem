using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Products()
        {
            return View();
        }

        public IActionResult Categories()
        {
            return View();
        }

        public IActionResult Inventory()
        {
            return View();
        }
    }
}
