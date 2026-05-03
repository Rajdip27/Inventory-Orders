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

public interface IProductRepository
{
    Task<PaginationModel<ProductVm>> GetProductsAsync(Filter filter, CancellationToken ct);
    Task<ProductVm> GetProductByIdAsync(long id, CancellationToken ct);
    Task<ProductVm> CreateOrUpdateProductAsync(ProductVm productVm, CancellationToken ct);
    Task<bool> DeleteProductAsync(long id, CancellationToken ct);
}
public class ProductRepository(ApplicationDbContext _context) : IProductRepository
{
    public async Task<ProductVm> CreateOrUpdateProductAsync(ProductVm vm, CancellationToken ct)
    {
        var product = vm.Id > 0
            ? await _context.Products.FirstOrDefaultAsync(p => p.Id == vm.Id, ct)
            : new Product();

        if (vm.Id > 0 && product == null)
            return null;

        product.Name = vm.Name;
        product.SKU = vm.SKU;
        product.Price = vm.Price;
        product.QuantityInStock = vm.QuantityInStock;
        product.CreatedAt = vm.Id > 0 ? product.CreatedAt : DateTime.UtcNow;

        if (vm.Id > 0)
            _context.Products.Update(product);
        else
            await _context.Products.AddAsync(product, ct);

        await _context.SaveChangesAsync(ct);

        return product.Adapt<ProductVm>();
    }

    // Delete (soft delete)
    public async Task<bool> DeleteProductAsync(long id, CancellationToken ct)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product == null) return false;

        product.IsDelete = true;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    // Get by Id
    public async Task<ProductVm> GetProductByIdAsync(long id, CancellationToken ct)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDelete, ct);
        return product?.Adapt<ProductVm>();
    }

    // Get paginated list
    public async Task<PaginationModel<ProductVm>> GetProductsAsync(Filter filter, CancellationToken ct)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => !p.IsDelete);

        query = SpecificationEvaluator<Product>.GetQuery(query, new ProductSpecification(filter));

        return await query
            .ProjectToType<ProductVm>()
            .ToPagedListAsync(filter.Page, filter.PageSize);
    }
}