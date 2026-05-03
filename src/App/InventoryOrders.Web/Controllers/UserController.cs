using Microsoft.AspNetCore.Mvc;

namespace InventoryOrders.Web.Controllers;

public class UserController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
