namespace InventoryOrders.Application.ViewModel;

public class DashboardVm
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSales { get; set; }
    public int LowStockCount { get; set; }
    public List<RecentOrderVm> RecentOrders { get; set; } = new();
    public List<LowStockProductVm> LowStockProducts { get; set; } = new();
}

public class RecentOrderVm
{
    public long Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class LowStockProductVm
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
}