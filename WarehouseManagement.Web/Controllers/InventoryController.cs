using Microsoft.AspNetCore.Mvc;

namespace WarehouseManagement.Web.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
