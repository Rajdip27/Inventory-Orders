using InventoryOrders.Application.Filters;
using InventoryOrders.Core.Entities;

namespace InventoryOrders.Application.Expressions.ModelSpecification;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(Filter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
            ApplyCriteria(p => p.Name.Contains(filter.Search) || p.SKU.Contains(filter.Search));
        if (filter.IsDelete)
            ApplyCriteria(p => !p.IsDelete);
        ApplyOrderByDescending(p => p.Id);
    }
}