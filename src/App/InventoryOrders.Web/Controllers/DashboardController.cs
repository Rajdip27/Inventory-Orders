using InventoryOrders.Application.Repositories;
using InventoryOrders.Application.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrders.Web.Controllers;

[Authorize]
public class DashboardController(IDashboardRepository _dashboardRepository) : Controller
{
    

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var model = await _dashboardRepository.GetDashboardAsync(cancellationToken);
        return View(model);
    }

    [HttpPost]
    public IActionResult SetTimeZone([FromBody] TimeZoneRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.TimeZone))
            return BadRequest(new { success = false, message = "Time zone is required." });

        return Json(new { success = true, timeZone = request.TimeZone });
    }
}

public class TimeZoneRequest
{
    public string TimeZone { get; set; } = string.Empty;
}