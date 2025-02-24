using BLAExercise.Domain.Models;
using BLAExercise.Infrastructure.Interfaces;
using Microsoft.Data.SqlClient;

namespace BLAExercise.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString) : base(connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<User?> GetByEmailAsync(string email)
    {
        var query = "SELECT * FROM Users WHERE Email = @email";
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
                        var entity = Activator.CreateInstance<User>();
                        MapReaderToEntity(reader, entity);
                        return entity;
                    }
                }
            }
        }
        return default;
    }
}
