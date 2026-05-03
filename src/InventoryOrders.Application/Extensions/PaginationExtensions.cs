using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using InventoryOrders.Application.CommonModel;

namespace InventoryOrders.Application.Extensions;

public static class PaginationExtensions
{
    /// <summary>
    /// Returns a paged list safely, optionally sorted by a key selector.
    /// </summary>
    public static async Task<PaginationModel<T>> ToPagedListAsync<T, TKey>(
        this IQueryable<T>source,
        int pageNumber = 1,
        int pageSize = 10,
        Expression<Func<T, TKey>> orderBy = null,
        bool ascending = true)
    {
        // Handle null source
        if (source is null)
            return new PaginationModel<T>();

        // Ensure valid pageNumber and pageSize
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        // Apply sorting if provided
        if (orderBy is not null)
            source = ascending ? source.OrderBy(orderBy) : source.OrderByDescending(orderBy);

        var totalItems = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return new PaginationModel<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    /// <summary>
    /// Overload without sorting.
    /// </summary>
    public static Task<PaginationModel<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber = 1,
        int pageSize = 10)
    {
        return source.ToPagedListAsync<T, object>(pageNumber, pageSize, null);
    }
}
