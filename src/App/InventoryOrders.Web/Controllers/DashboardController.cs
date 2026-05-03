using InventoryOrders.Application.CommonModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrders.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        // Replace with real DB data
        ViewBag.TotalProducts = 120;
        ViewBag.TotalOrders = 45;
        ViewBag.TotalSales = 78500;
        ViewBag.LowStockCount = 8;

        ViewBag.RecentOrders = new[]
        {
            new { Id = 101, CustomerName = "Raj", OrderDate = DateTime.Now.AddDays(-1), TotalAmount = 2500 },
            new { Id = 102, CustomerName = "Amit", OrderDate = DateTime.Now.AddDays(-2), TotalAmount = 1800 }
        };

        ViewBag.LowStockProducts = new[]
        {
            new { Name = "Laptop", SKU = "LP1001", QuantityInStock = 2 },
            new { Name = "Mouse", SKU = "MS2002", QuantityInStock = 4 }
        };

        return View();
    }

    [HttpPost]
    public IActionResult SetTimeZone([FromBody] TimeZoneRequest request)
    {
        return Json(new { success = true, timeZone = request.TimeZone });
    }
}
