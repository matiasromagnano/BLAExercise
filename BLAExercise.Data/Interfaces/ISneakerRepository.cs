using BLAExercise.Data.Models;

namespace BLAExercise.Data.Interfaces;

public interface ISneakerRepository : IGenericRepository<Sneaker>
{
    Task<List<Sneaker>> GetSneakersByUserIdAsync(int userId);
    Task<List<Sneaker>> GetSneakersByUserEmailAsync(string email);
}
