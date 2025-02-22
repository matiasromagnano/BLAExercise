using BLAExercise.Data.Interfaces;
using BLAExercise.Data.Models;

namespace BLAExercise.Data.Repositories;

public class SneakerRepository : GenericRepository<Sneaker>, ISneakerRepository
{
    private readonly string _connectionString; 

    public SneakerRepository(string connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<List<Sneaker>> GetSneakersByUserIdAsync(int userId)
    {
        return await Task.FromResult(new List<Sneaker>());
    }

    public async Task<List<Sneaker>> GetSneakersByUserEmailAsync(string email)
    {
        return await Task.FromResult(new List<Sneaker>());
    }
}
