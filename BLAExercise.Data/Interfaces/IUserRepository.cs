using BLAExercise.Data.Models;

namespace BLAExercise.Data.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
}