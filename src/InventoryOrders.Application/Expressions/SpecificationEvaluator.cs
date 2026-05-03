using Microsoft.EntityFrameworkCore;

namespace InventoryOrders.Application.Expressions;

public class SpecificationEvaluator<T> where T : class
{
    /// <summary>
    /// Gets the query.
    /// </summary>
    /// <param name="inputQuery">The input query.</param>
    /// <param name="spec">The spec.</param>
    /// <returns></returns>
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
    {
        var query = inputQuery;

        // Apply multiple filters
        if (spec.Criteria.Any())
        {
            foreach (var criteria in spec.Criteria)
            {
                query = query.Where(criteria);
            }
        }

        // Apply ordering
        if (spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending != null)
            query = query.OrderByDescending(spec.OrderByDescending);

        // Apply includes
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        return query;
    }
}
