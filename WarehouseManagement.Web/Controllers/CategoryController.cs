using Microsoft.AspNetCore.Mvc;

namespace WarehouseManagement.Web.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
