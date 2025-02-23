using BLAExercise.Data.Models;

namespace BLAExercise.Data.Interfaces;

public interface IGenericRepository<T> where T : IDomainEntity
{
    /// <summary>
    /// Retrieves a paginated and sorted list of entities based on the provided query parameters.
    /// </summary>
    /// <param name="queryParameters">The query parameters including page number, page size, sort field, and sort direction.</param>
    /// <returns>A task that represents the asynchronous operation, returning a list of entities.</returns>
    Task<List<T>> GetAsync(GetQueryParameters queryParameters);

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation, returning the entity if found, or null if not found.</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Adds a new entity to the data store.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<T?> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity in the data store.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity from the data store by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(int id);
}
