using BuildingBlocks.Infra.Exceptions;
using BuildingBlocks.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infra.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected repository error while adding {typeof(TEntity).Name}.", ex);
        }
    }

    public async Task AddManyAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected repository error while adding multiple {typeof(TEntity).Name} entities.", ex);
        }
    }

    public async Task<TEntity?> FindOneAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ApplySpecification(spec)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            throw new RepositorySpecificationException($"Invalid specification used for {typeof(TEntity).Name} FindOne query.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryQueryException($"Failed to query {typeof(TEntity).Name} in FindOne.", ex);
        }
    }

    public async Task<TResult?> FindOneAsync<TResult>(
        ISpecification<TEntity, TResult> spec,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ApplySpecification(spec)
                .Select(spec.Selector)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            throw new RepositorySpecificationException($"Invalid specification used for {typeof(TEntity).Name} FindOne query.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryQueryException($"Failed to query {typeof(TEntity).Name} in FindOne.", ex);
        }
    }

    public async Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryQueryException($"Failed to query all {typeof(TEntity).Name} entities.", ex);
        }
    }

    public async Task<List<TEntity>> WhereAsync(
        ISpecification<TEntity> spec, 
        CancellationToken cancellationToken)
    {
        try
        {
            return await ApplySpecification(spec)
                .ToListAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            throw new RepositorySpecificationException($"Invalid specification used for {typeof(TEntity).Name} Where query.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryQueryException($"Failed to query {typeof(TEntity).Name} in Where.", ex);
        }
    }

    public async Task<List<TResult>> WhereAsync<TResult>(
        ISpecification<TEntity, TResult> spec,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ApplySpecification(spec)
                .Select(spec.Selector)
                .ToListAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            throw new RepositorySpecificationException($"Invalid specification used for {typeof(TEntity).Name} Where query.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryQueryException($"Failed to query {typeof(TEntity).Name} in Where.", ex);
        }
    }

    public async Task<int> CountAsync(
        ISpecification<TEntity> spec, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(spec);

        try
        {
            return await ApplySpecification(
                spec,
                ignorePaging: true,
                ignoreIncludes: true,
                ignoreOrdering: true)
                .CountAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            throw new RepositorySpecificationException($"Invalid specification used for {typeof(TEntity).Name} Count query.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryQueryException($"Failed to count {typeof(TEntity).Name} entities.", ex);
        }
    }

    public void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            _dbSet.Update(entity);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected repository error while updating {typeof(TEntity).Name}.", ex);
        }
    }

    public void UpdateMany(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            _dbSet.UpdateRange(entities);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected repository error while updating multiple {typeof(TEntity).Name} entities.", ex);
        }
    }

    public void Delete(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Unexpected repository error while deleting {typeof(TEntity).Name}.", ex);
        }
    }

    private IQueryable<TEntity> ApplySpecification(
        ISpecification<TEntity> spec, 
        bool ignorePaging = false,
        bool ignoreIncludes = false,
        bool ignoreOrdering = false)
    {
        ArgumentNullException.ThrowIfNull(spec);

        IQueryable<TEntity> query = _dbSet.AsQueryable().AsNoTracking();

        if (!spec.AsNoTracking)
            query = query.AsTracking();

        if (spec.Criteria != null) 
            query = query.Where(spec.Criteria);

        if (!ignoreIncludes)
        {
            query = spec.Includes
                .Aggregate(query, (current, include) => current.Include(include));
        }

        if (!ignoreOrdering && spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);

        if (!ignorePaging)
        {
            if (spec.Skip.HasValue)
                query = query.Skip(spec.Skip.Value);

            if (spec.Take.HasValue)
                query = query.Take(spec.Take.Value);
        }

        return query;
    }
}
