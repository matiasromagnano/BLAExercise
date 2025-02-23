using BLAExercise.Data.Extensions;
using BLAExercise.Data.Interfaces;
using BLAExercise.Data.Models;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace BLAExercise.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : IDomainEntity
{
    private readonly string _connectionString;
    private readonly string _tableName;

    public GenericRepository(string? connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException("SqlConnectionString is missing from the configuration."); ;
        _tableName = typeof(T).Name + "s"; // Convention: pluralize entity name (e.g., User -> Users)
    }

    /// <inheritdoc/>
    public async Task<T?> AddAsync(T entity)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name != $"{nameof(IDomainEntity.Id)}" && p.Name != $"{nameof(IDomainEntity.CreationDate)}" && p.CanWrite);

        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var parameterNames = string.Join(", ", properties.Select(p => "@" + p.Name));
        var outputColumns = string.Join(", ", typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => $"INSERTED.{p.Name}"));
        var query = $"INSERT INTO {_tableName} ({columnNames}) OUTPUT {outputColumns} VALUES ({parameterNames})";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                foreach (var prop in properties)
                {
                    command.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entity) ?? DBNull.Value);
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        MapReaderToEntity(reader, entity);
                    }
                }
            }
        }

        return entity;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var query = $"DELETE FROM {_tableName} WHERE Id = @Id";
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    /// <inheritdoc/>
    public async Task<List<T>> GetAsync(GetQueryParameters queryParameters)
    {
        var sortDirection = queryParameters.Descending ? "DESC" : "ASC";
        var offset = (queryParameters.Page - 1) * queryParameters.PageSize;
        var pageSize = queryParameters.PageSize;

        // Validate SortBy to prevent SQL injection
        var validProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name).ToList();
        var sortBy = validProperties.Contains(queryParameters.SortBy, StringComparer.OrdinalIgnoreCase) ? queryParameters.SortBy : "Id"; // Default to Id if invalid

        // Build SQL query with pagination and sorting
        var sqlQuery = $@"
        SELECT * FROM {_tableName}
        ORDER BY {sortBy} {sortDirection}
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var result = new List<T>();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var entity = Activator.CreateInstance<T>();
                        MapReaderToEntity(reader, entity);
                        result.Add(entity);
                    }
                }
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(int id)
    {
        var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var entity = Activator.CreateInstance<T>();
                        MapReaderToEntity(reader, entity);
                        return entity;
                    }
                }
            }
        }
        return default;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(T entity)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name != $"{nameof(IDomainEntity.Id)}" && p.Name != $"{nameof(IDomainEntity.CreationDate)}" && p.CanWrite); // Exclude Id (used in WHERE clause) and CreationDate since it's automatically set in the DB side

        var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
        var query = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                foreach (var prop in properties)
                {
                    command.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entity) ?? DBNull.Value);
                }
                command.Parameters.AddWithValue("@Id", entity.Id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public void MapReaderToEntity(SqlDataReader reader, T entity)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (reader.HasColumn(prop.Name) && !reader.IsDBNull(reader.GetOrdinal(prop.Name)))
            {
                var value = reader[prop.Name];
                prop.SetValue(entity, value);
            }
        }
    }
}
