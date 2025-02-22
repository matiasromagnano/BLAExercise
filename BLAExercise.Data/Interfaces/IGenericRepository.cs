using BLAExercise.Data.Models;

namespace BLAExercise.Data.Interfaces;

public interface IGenericRepository<T> where T : IDomainEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
