using BLAExercise.Data.Interfaces;
using BLAExercise.Data.Models;
using System.Numerics;

namespace BLAExercise.Data.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await Task.FromResult(new User());
    }
}
