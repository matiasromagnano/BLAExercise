using BLAExercise.Data.Interfaces;
using BLAExercise.Data.Models;
using Microsoft.Data.SqlClient;

namespace BLAExercise.Data.Repositories;

public class SneakerRepository : GenericRepository<Sneaker>, ISneakerRepository
{
    private readonly string _connectionString; 

    public SneakerRepository(string connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<List<Sneaker>> GetSneakersByUserIdAsync(int userId)
    {
        var query = "SELECT * FROM Sneakers WHERE UserId = @userId";
        var result = new List<Sneaker>();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var entity = Activator.CreateInstance<Sneaker>();
                        MapReaderToEntity(reader, entity);
                        result.Add(entity);
                    }
                }
            }
        }
        return result;
    }

    /// <inheritdoc/>
    public async Task<List<Sneaker>> GetSneakersByUserEmailAsync(string email)
    {
        var query = @"SELECT * FROM Sneakers S
            JOIN Users U ON S.UserId = U.Id WHERE U.Email = @email";
        var result = new List<Sneaker>();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var entity = Activator.CreateInstance<Sneaker>();
                        MapReaderToEntity(reader, entity);
                        result.Add(entity);
                    }
                }
            }
        }
        return result;
    }
}
