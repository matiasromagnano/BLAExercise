using Microsoft.Data.SqlClient;

namespace BLAExercise.Data.Database;

/// <summary>
/// A utility class responsible for managing the lifecycle of the SneakerCollection database, 
/// including creation, table setup, and optional deletion.
/// </summary>
public class DatabaseCreator
{
    private readonly string _sqlServerConnectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseCreator"/> class with the specified dependencies.
    /// </summary>
    /// <param name="sqlServerConnectionString">The full connection string including the database name.</param>
    /// <param name="logger">The logger instance for tracking database operations.</param>
    public DatabaseCreator(string sqlServerConnectionString)
    {
        _sqlServerConnectionString = sqlServerConnectionString ?? throw new ArgumentNullException(nameof(sqlServerConnectionString));
    }

    /// <summary>
    /// Creates the SneakerCollection database and its tables (Users and Sneakers) if they do not exist, 
    /// including seeding initial user data.
    /// </summary>
    public void CreateDatabaseAndTables(string? databaseName)
    {
        if (databaseName is not null)
        {
            // Step 1: Create the database using the master database
            string createDatabaseScript = @$"
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
            BEGIN
                CREATE DATABASE {databaseName};
                SELECT 'Database created.';
            END
            ELSE
                SELECT 'Database already exists.';
        ";

            using (var connection = new SqlConnection(_sqlServerConnectionString))
            {
                connection.Open();
                using var command = new SqlCommand(createDatabaseScript, connection);
                var result = command.ExecuteScalar()?.ToString();
            }

            // Step 2: Create tables using the original connection string
            string createTablesScript = @$"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
            BEGIN
                CREATE TABLE Users (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Email NVARCHAR(255) NOT NULL UNIQUE,
                    Password NVARCHAR(255) NOT NULL,
                    CreationDate DATETIME NOT NULL DEFAULT GETUTCDATE()
                );
            END
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sneakers')
            BEGIN
                CREATE TABLE Sneakers (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(255),
                    Brand NVARCHAR(255),
                    Price DECIMAL(18,2),
                    SizeUS FLOAT,
                    Year INT,
                    Rate INT,
                    CreationDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    UserId INT NOT NULL,
                    CONSTRAINT FK_Sneakers_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
                );
            END
            IF NOT EXISTS (SELECT 1 FROM Users)
            BEGIN
                SET IDENTITY_INSERT Users ON;
                INSERT INTO Users (Id, Email, Password)
                VALUES 
                (1, 'user@example.com', 'string'),
                (2, 'user2@example.com', 'Password2'),
                (3, 'user3@example.com', 'Password3');
                SET IDENTITY_INSERT Users OFF;
            END
        ";

            using (var connection = new SqlConnection($"{_sqlServerConnectionString};Database={databaseName}"))
            {
                connection.Open();
                using var command = new SqlCommand(createTablesScript, connection);
                command.ExecuteNonQuery();
            }
        }        
    }

    /// <summary>
    /// Removes the Database parameter from the connection string to connect to the master database.
    /// </summary>
    /// <param name="connectionString">The original connection string with a database specified.</param>
    /// <returns>A connection string without the Database parameter.</returns>
    private string RemoveDatabaseFromConnectionString(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        builder.InitialCatalog = "master"; // Use master database explicitly to avoid failures when using "USE" statement in SQL
        return builder.ConnectionString;
    }
}
