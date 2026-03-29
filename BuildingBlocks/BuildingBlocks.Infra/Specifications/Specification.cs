using BuildingBlocks.Infra.Interfaces;
using System.Linq.Expressions;

namespace BuildingBlocks.Infra.Specifications;

public abstract class Specification<TEntity> : ISpecification<TEntity>
{
    private readonly List<Expression<Func<TEntity, object>>> _includes = new();

    public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }
    public IReadOnlyCollection<Expression<Func<TEntity, object>>> Includes => _includes;
    public Expression<Func<TEntity, object>>? OrderBy { get; protected set; }
    public int? Take { get; protected set; }
    public int? Skip { get; protected set; }
    public bool AsNoTracking { get; protected set; } = true;

    protected void AddCriteria(Expression<Func<TEntity, bool>>? criteria)
        => Criteria = criteria;

    protected void AddIncludes(Expression<Func<TEntity, object>> includes)
        => _includes.Add(includes);

    protected void ApplyOrderBy(Expression<Func<TEntity, object>>? orderBy)
        => OrderBy = orderBy;

    protected void ApplyPaging(int skip, int take)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(skip);

        if (take <= 0)
            throw new ArgumentOutOfRangeException(nameof(take), "The page size must be greater than zero.");

        Skip = skip;
        Take = take;
    }

    protected void EnableTracking()
        => AsNoTracking = false;
}

public abstract class Specification<TEntity, TResult>
    : Specification<TEntity>, ISpecification<TEntity, TResult>
{
    public Expression<Func<TEntity, TResult>> Selector { get; }

    protected Specification(Expression<Func<TEntity, TResult>> selector)
    {
        Selector = selector ?? throw new ArgumentNullException(nameof(selector));
    }
}