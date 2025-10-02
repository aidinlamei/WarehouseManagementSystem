using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Warehouses;
using WarehouseManagement.Core.Interfaces.IServices;

namespace WarehouseManagement.Web.Controllers
{
    public class WarehouseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}