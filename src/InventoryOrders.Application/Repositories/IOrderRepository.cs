using InventoryOrders.Application.CommonModel;
using InventoryOrders.Application.Expressions;
using InventoryOrders.Application.Expressions.ModelSpecification;
using InventoryOrders.Application.Extensions;
using InventoryOrders.Application.Filters;
using InventoryOrders.Application.ViewModel;
using InventoryOrders.Core.Entities;
using InventoryOrders.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrders.Application.Repositories;

public interface IOrderRepository
{
    Task<PaginationModel<OrderVm>> GetOrdersAsync(Filter filter, CancellationToken ct);
    Task<OrderVm> GetOrderByIdAsync(long id, CancellationToken ct);
    Task<OrderVm> CreateOrUpdateOrderAsync(OrderVm vm, CancellationToken ct);
    Task<bool> DeleteOrderAsync(long id, CancellationToken ct);
    Task<List<ProductVm>> GetProductDropdownAsync(CancellationToken ct);
}

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<ProductVm>> GetProductDropdownAsync(CancellationToken ct)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(x => !x.IsDelete)
            .OrderBy(x => x.Name)
            .ProjectToType<ProductVm>()
            .ToListAsync(ct);
    }

    public async Task<PaginationModel<OrderVm>> GetOrdersAsync(Filter filter, CancellationToken ct)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .Where(x => !x.IsDelete);

        query = SpecificationEvaluator<Order>.GetQuery(query, new OrderSpecification(filter));

        return await query
            .ProjectToType<OrderVm>()
            .ToPagedListAsync(filter.Page, filter.PageSize);
    }

    public async Task<OrderVm?> GetOrderByIdAsync(long id, CancellationToken ct)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete, ct);

        if (order == null) return null;

        return new OrderVm
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Items = order.OrderItems.Select(x => new OrderItemVm
            {
                Id = x.Id,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                ProductName = x.Product?.Name
            }).ToList()
        };
    }

    public async Task<OrderVm> CreateOrUpdateOrderAsync(OrderVm vm, CancellationToken ct)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            if (vm.Items == null || !vm.Items.Any())
                throw new Exception("At least one order item is required.");

            Order order;
            bool isEdit = vm.Id > 0;

            // Load existing order for edit
            if (isEdit)
            {
                order = await _context.Orders
                    .Include(x => x.OrderItems)
                    .FirstOrDefaultAsync(x => x.Id == vm.Id && !x.IsDelete, ct);

                if (order == null)
                    return null;
            }
            else
            {
                order = new Order();
                await _context.Orders.AddAsync(order, ct);
            }
            var oldQtyByProduct = isEdit
                ? order!.OrderItems
                    .GroupBy(x => x.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity))
                : new Dictionary<long, int>();
            var newQtyByProduct = vm.Items
                .GroupBy(x => x.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var allProductIds = newQtyByProduct.Keys.Union(oldQtyByProduct.Keys).ToList();

            var products = await _context.Products
                .Where(p => allProductIds.Contains(p.Id) && !p.IsDelete)
                .ToListAsync(ct);
            foreach (var kvp in newQtyByProduct)
            {
                var product = products.FirstOrDefault(p => p.Id == kvp.Key);

                if (product == null)
                    throw new Exception($"Product with Id {kvp.Key} not found.");

                var oldQty = oldQtyByProduct.ContainsKey(kvp.Key) ? oldQtyByProduct[kvp.Key] : 0;
                var newQty = kvp.Value;

                var availableStock = product.QuantityInStock + oldQty;

                if (availableStock < newQty)
                {
                    throw new Exception(
                        $"Product: {product.Name} cannot be ordered. Available stock is {availableStock}, requested {newQty}."
                    );
                }
            }
            if (isEdit)
            {
                _context.OrderItems.RemoveRange(order!.OrderItems);
                order.OrderItems.Clear();
            }

            order!.CustomerName = vm.CustomerName;
            order.OrderDate = vm.OrderDate;

            decimal total = 0;
            foreach (var itemVm in vm.Items)
            {
                var product = products.First(p => p.Id == itemVm.ProductId);

                var item = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemVm.Quantity,
                    UnitPrice = itemVm.UnitPrice
                };

                total += item.Quantity * item.UnitPrice;
                order.OrderItems.Add(item);
            }
            foreach (var kvp in newQtyByProduct)
            {
                var product = products.First(p => p.Id == kvp.Key);
                var oldQty = oldQtyByProduct.ContainsKey(kvp.Key) ? oldQtyByProduct[kvp.Key] : 0;
                var newQty = kvp.Value;

                product.QuantityInStock = product.QuantityInStock + oldQty - newQty;
            }

            order.TotalAmount = total;

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return await GetOrderByIdAsync(order.Id, ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<bool> DeleteOrderAsync(long id, CancellationToken ct)
    {
        var order = await _context.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete, ct);
        if (order == null) return false;
        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == item.ProductId && !x.IsDelete, ct);

            if (product != null)
                product.QuantityInStock += item.Quantity;
        }

        order.IsDelete = true;
        await _context.SaveChangesAsync(ct);
        return true;
    }
}