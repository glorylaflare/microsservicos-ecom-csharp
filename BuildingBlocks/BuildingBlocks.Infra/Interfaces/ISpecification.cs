using System.Linq.Expressions;

namespace BuildingBlocks.Infra.Interfaces;

/// <summary>
/// Defines a reusable query specification for <typeparamref name="TEntity"/>.
/// Each property represents one part of the query:
/// Criteria = WHERE, Includes = JOIN, OrderBy = ORDER BY, Skip/Take = OFFSET/LIMIT.
/// </summary>
/// <typeparam name="TEntity">The entity type targeted by this specification.</typeparam>
public interface ISpecification<TEntity>
{
    /// <summary>
    /// Filter expression used to restrict results (SQL equivalent: WHERE).
    /// </summary>
    Expression<Func<TEntity, bool>>? Criteria { get; }

    /// <summary>
    /// Navigation properties to eagerly load related data (SQL equivalent: JOIN).
    /// </summary>
    IReadOnlyCollection<Expression<Func<TEntity, object>>> Includes { get; }

    /// <summary>
    /// Expression used to sort the result set in ascending order (SQL equivalent: ORDER BY).
    /// </summary>
    Expression<Func<TEntity, object>>? OrderBy { get; }

    /// <summary>
    /// Maximum number of records to return (SQL equivalent: LIMIT).
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Number of records to skip before returning results (SQL equivalent: OFFSET).
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Indicates whether the query should disable entity tracking (EF Core equivalent: AsNoTracking).
    /// </summary>
    bool AsNoTracking { get; }
}

/// <summary>
/// Extends a specification with a projection expression that maps <typeparamref name="TEntity"/> into <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TEntity">The source entity type used by the query.</typeparam>
/// <typeparam name="TResult">The projected result type returned by the query.</typeparam>
public interface ISpecification<TEntity, TResult> : ISpecification<TEntity>
{
    /// <summary>
    /// Projection expression used to shape query results (SQL equivalent: SELECT).
    /// </summary>
    Expression<Func<TEntity, TResult>> Selector { get; }
}