using System.Linq.Expressions;

namespace InventoryOrders.Application.Expressions;

public class BaseSpecification<T>: ISpecification<T>
{
    /// <summary>
    /// Gets or sets the criteria.
    /// </summary>
    /// <value>
    /// The criteria.
    /// </value>
    public List<Expression<Func<T, bool>>> Criteria { get; } = new();
    public Expression<Func<T, object>> OrderBy { get; protected set; }
    public Expression<Func<T, object>> OrderByDescending { get; protected set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    protected void AddInclude(Expression<Func<T, object>> includeExpression) =>
        Includes.Add(includeExpression);

    protected void ApplyCriteria(Expression<Func<T, bool>> criteria) =>
        Criteria.Add(criteria);

    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) =>
        OrderBy = orderByExpression;

    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression) =>
        OrderByDescending = orderByDescExpression;
}
