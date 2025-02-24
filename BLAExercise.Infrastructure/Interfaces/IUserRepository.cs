using BLAExercise.Domain.Models;

namespace BLAExercise.Infrastructure.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation, returning the user if found, or null if not found.</returns>
    Task<User?> GetByEmailAsync(string email);
}