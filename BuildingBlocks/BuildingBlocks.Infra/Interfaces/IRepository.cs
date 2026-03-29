namespace BuildingBlocks.Infra.Interfaces;

/// <summary>
/// Defines a generic repository contract for managing aggregate persistence and retrieval operations.
/// </summary>
/// <typeparam name="TEntity">The entity type handled by the repository.</typeparam>
public interface IRepository<TEntity>
{
    /// <summary>
    /// Adds a single entity instance asynchronously.
    /// </summary>
    /// <param name="entity">The entity to persist.</param>
    /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities in a single asynchronous operation.
    /// </summary>
    /// <param name="entities">The collection of entities to persist.</param>
    /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first entity that matches the provided specification or <see langword="null"/> when no match is found.
    /// </summary>
    /// <param name="spec">The specification that defines query filters and options.</param>
    /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
    /// <returns>The matching entity, or <see langword="null"/> if none exists.</returns>
    Task<TEntity?> FindOneAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first projected result that matches the provided specification or <see langword="null"/> when no match is found.
    /// </summary>
    /// <typeparam name="TResult">The projection type returned by the query.</typeparam>
    /// <param name="spec">The specification that defines query filters and projection rules.</param>
    /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
    /// <returns>The matching projected result, or <see langword="null"/> if none exists.</returns>
    Task<TResult?> FindOneAsync<TResult>(ISpecification<TEntity, TResult> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities of the current repository type.
    /// </summary>
    /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
    /// <returns>A collection containing all persisted entities.</returns>
    Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns entities using the provided specification, typically applying paging and ordering rules.
    /// </summary>
    /// <param name="spec">The specification that defines criteria, ordering, and paging.</param>
    /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
    /// <returns>A collection of entities that satisfy the specification.</returns>
    Task<List<TEntity>> WhereAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns projected results using the provided specification, typically applying filtering, ordering, and paging rules.
    /// </summary>
    /// <typeparam name="TResult">The projection type returned by the query.</typeparam>
    /// <param name="spec">The specification that defines criteria, ordering, paging, and projection.</param>
    /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
    /// <returns>A collection of projected results that satisfy the specification.</returns>
    Task<List<TResult>> WhereAsync<TResult>(ISpecification<TEntity, TResult> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities that match the provided specification.
    /// </summary>
    /// <param name="spec">The specification used to filter records before counting.</param>
    /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
    /// <returns>The total number of matching entities.</returns>
    Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity as modified so its changes are persisted.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Marks multiple entities as modified so their changes are persisted.
    /// </summary>
    /// <param name="entities">The entities with updated values.</param>
    void UpdateMany(IEnumerable<TEntity> entities);

    /// <summary>
    /// Removes an entity from persistence.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(TEntity entity);
}
