using Dapper;
using InventoryOrders.Application.ViewModel;
using InventoryOrders.Infrastructure.Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InventoryOrders.Application.Repositories;

public interface IDashboardRepository
{
    Task<DashboardVm> GetDashboardAsync(CancellationToken ct);
}
public class DashboardRepository(IDbConnectionFactory _connectionFactory) : IDashboardRepository
{

    public async Task<DashboardVm> GetDashboardAsync(CancellationToken ct)
    {
        using var db = _connectionFactory.CreateConnection();

        if (db is SqlConnection sqlConnection)
            await sqlConnection.OpenAsync(ct);
        else
            db.Open();

        using var transaction = db.BeginTransaction();

        try
        {
            var totalProductsTask = db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Products WHERE IsDelete = 0",
                transaction: transaction);

            var totalOrdersTask = db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Orders WHERE IsDelete = 0",
                transaction: transaction);

            var totalSalesTask = db.ExecuteScalarAsync<decimal?>(
                "SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders WHERE IsDelete = 0",
                transaction: transaction);

            var lowStockCountTask = db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Products WHERE IsDelete = 0 AND QuantityInStock <= 5",
                transaction: transaction);

            var recentOrdersTask = db.QueryAsync<RecentOrderVm>(
                @"SELECT TOP 5 Id, CustomerName, OrderDate, TotalAmount
              FROM Orders
              WHERE IsDelete = 0
              ORDER BY OrderDate DESC",
                transaction: transaction);

            var lowStockProductsTask = db.QueryAsync<LowStockProductVm>(
                @"SELECT TOP 5 Name, SKU, QuantityInStock
              FROM Products
              WHERE IsDelete = 0 AND QuantityInStock <= 5
              ORDER BY QuantityInStock ASC, Name ASC",
                transaction: transaction);

            await Task.WhenAll(
                totalProductsTask,
                totalOrdersTask,
                totalSalesTask,
                lowStockCountTask,
                recentOrdersTask,
                lowStockProductsTask);

            transaction.Commit();

            return new DashboardVm
            {
                TotalProducts = await totalProductsTask,
                TotalOrders = await totalOrdersTask,
                TotalSales = await totalSalesTask ?? 0,
                LowStockCount = await lowStockCountTask,
                RecentOrders = (await recentOrdersTask).ToList(),
                LowStockProducts = (await lowStockProductsTask).ToList()
            };
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}