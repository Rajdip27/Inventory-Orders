using Microsoft.AspNetCore.Mvc.RazorPages;
using InventoryOrders.Application.Enums;

namespace InventoryOrders.Application.Helpers;

public static class ToastHelper
{
    public static void ShowToast(this PageModel page, string message, AlertType type)
    {
        page.TempData["AlertMessage"] = message;
        page.TempData["AlertType"] = type.ToString();
    }
}
