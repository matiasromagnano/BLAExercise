using BLAExercise.Data.Interfaces;

namespace BLAExercise.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : IDomainEntity
{
    private readonly string _connectionString;

    public GenericRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddAsync(T entity)
    {
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        await Task.CompletedTask;
    }

    public async Task<List<T>> GetAllAsync()
    { 
        return await Task.FromResult(new List<T>());
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await Task.FromResult(default(T));
    }

    public async Task UpdateAsync(T entity)
    {
        await Task.CompletedTask;
    }
}
