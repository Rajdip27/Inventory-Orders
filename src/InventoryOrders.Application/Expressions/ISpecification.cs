using System.Linq.Expressions;

namespace InventoryOrders.Application.Expressions;

public interface ISpecification<T>
{
    /// <summary>
    /// Gets the criteria.
    /// </summary>
    /// <value>
    /// The criteria.
    /// </value>
    List<Expression<Func<T, bool>>> Criteria { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    List<Expression<Func<T, object>>> Includes { get; }
}
