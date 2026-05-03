using InventoryOrders.Application.Filters;
using InventoryOrders.Core.Entities;

namespace InventoryOrders.Application.Expressions.ModelSpecification;

public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(Filter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            ApplyCriteria(o =>
                o.CustomerName.Contains(filter.Search) ||
                o.OrderItems.Any(oi => oi.Product.Name.Contains(filter.Search)));
        }

        if (filter.IsDelete)
            ApplyCriteria(o => !o.IsDelete);

        ApplyOrderByDescending(o => o.Id);
    }
}
