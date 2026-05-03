using InventoryOrders.Application.Filters;
using InventoryOrders.Application.Logging;
using InventoryOrders.Application.Repositories;
using InventoryOrders.Application.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrders.Web.Controllers;

public class OrderController(IOrderRepository _orderRepository, IAppLogger<OrderController> _logger) : Controller
{
    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = new Filter
            {
                Search = search,
                IsDelete = false,
                Page = page,
                PageSize = pageSize
            };

            var pagination = await _orderRepository.GetOrdersAsync(filter, cancellationToken);
            return View(pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching orders", ex);
            return StatusCode(500, "An error occurred while fetching orders.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderRepository.GetOrderByIdAsync(id, cancellationToken);

            if (order == null)
                return NotFound();

            return View(order);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while loading order details Id={id}", ex);
            return StatusCode(500, "An error occurred while loading order details.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> CreateOrEdit(long id = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            var vm = new OrderVm();

            if (id > 0)
            {
                vm = await _orderRepository.GetOrderByIdAsync(id, cancellationToken)
                     ?? new OrderVm();
            }

            ViewBag.Products = await _orderRepository.GetProductDropdownAsync(cancellationToken);
            return View(vm);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error opening order form Id={id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrEdit(OrderVm vm, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid || vm.Items == null || vm.Items.Count == 0)
        {
            TempData["AlertMessage"] = "Please add at least one order item.";
            TempData["AlertType"] = "Warning";
            ViewBag.Products = await _orderRepository.GetProductDropdownAsync(cancellationToken);
            return View(vm);
        }

        try
        {
            var result = await _orderRepository.CreateOrUpdateOrderAsync(vm, cancellationToken);

            if (result == null)
                return NotFound();

            TempData["AlertMessage"] = vm.Id > 0 ? "Order updated successfully!" : "Order created successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while saving order", ex);
            TempData["AlertMessage"] = ex.Message;
            TempData["AlertType"] = "Error";
            ViewBag.Products = await _orderRepository.GetProductDropdownAsync(cancellationToken);
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _orderRepository.DeleteOrderAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            TempData["AlertMessage"] = "Order deleted successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting order Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting the order.";
            TempData["AlertType"] = "Error";
            return StatusCode(500, "An error occurred while deleting the order.");
        }
    }
}