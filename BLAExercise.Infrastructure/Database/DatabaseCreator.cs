using Microsoft.Data.SqlClient;

namespace BLAExercise.Infrastructure.Database;

/// <summary>
/// A utility class responsible for managing the lifecycle of the SneakerCollection database, 
/// including creation and table setup.
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
    /// including seeding initial realistic user and sneaker data for testing purposes.
    /// </summary>
    /// <param name="databaseName">The name of the database to create.</param>
    public void CreateDatabaseAndTables(string? databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentNullException(nameof(databaseName), "Database name cannot be null or empty.");
        }

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
            Console.WriteLine(result); // Optional: Log the result for debugging
        }

        // Step 2: Create tables and seed data using the specific database connection
        string createTablesAndSeedScript = @$"
        USE [{databaseName}];

        -- Create Users table if it doesn't exist
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
        BEGIN
            CREATE TABLE Users (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Email NVARCHAR(255) NOT NULL UNIQUE,
                Password NVARCHAR(255) NOT NULL,
                CreationDate DATETIME NOT NULL DEFAULT GETUTCDATE()
            );
        END

        -- Create Sneakers table if it doesn't exist
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sneakers')
        BEGIN
            CREATE TABLE Sneakers (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Name NVARCHAR(255) NOT NULL,
                Brand NVARCHAR(255) NOT NULL,
                Price DECIMAL(18,2) NOT NULL,
                SizeUS REAL NOT NULL,
                Year INT NOT NULL,
                Rate INT NOT NULL CHECK (Rate BETWEEN 1 AND 5),
                CreationDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
                UserId INT NOT NULL,
                CONSTRAINT FK_Sneakers_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
            );
        END

        -- Seed Users table with realistic data if empty
        IF NOT EXISTS (SELECT 1 FROM Users)
        BEGIN
            SET IDENTITY_INSERT Users ON;
            INSERT INTO Users (Id, Email, Password) VALUES
                (1, 'maria.gonzalez@example.com', 'SecurePass123!'),
                (2, 'john.doe@example.com', 'MyPassword456@'),
                (3, 'emily.smith@example.com', 'Pass789#');
            SET IDENTITY_INSERT Users OFF;
        END

        -- Seed Sneakers table with realistic data if empty
        IF NOT EXISTS (SELECT 1 FROM Sneakers)
        BEGIN
            SET IDENTITY_INSERT Sneakers ON;
            INSERT INTO Sneakers (Id, Name, Brand, Price, SizeUS, Year, Rate, UserId) VALUES
                -- Sneakers for Maria Gonzalez (UserId = 1)
                (1, 'Air Max 90', 'Nike', 120.00, 8.5, 2020, 4, 1),
                (2, 'Ultraboost 4.0', 'Adidas', 150.00, 9.0, 2021, 5, 1),
                (3, 'Classic Leather', 'Reebok', 80.00, 8.0, 2019, 3, 1),
                -- Sneakers for John Doe (UserId = 2)
                (4, 'Yeezy Boost 350', 'Adidas', 220.00, 10.0, 2022, 5, 2),
                (5, 'Air Force 1', 'Nike', 90.00, 9.5, 2018, 4, 2),
                (6, 'Gel-Kayano 28', 'Asics', 160.00, 10.5, 2023, 4, 2),
                -- Sneakers for Emily Smith (UserId = 3)
                (7, 'Chuck Taylor All Star', 'Converse', 60.00, 7.5, 2020, 3, 3),
                (8, 'NMD_R1', 'Adidas', 130.00, 8.0, 2021, 4, 3),
                (9, 'Pegasus 39', 'Nike', 115.00, 7.0, 2023, 5, 3);
            SET IDENTITY_INSERT Sneakers OFF;
        END
    ";

        using (var connection = new SqlConnection($"{_sqlServerConnectionString};Database={databaseName}"))
        {
            connection.Open();
            using var command = new SqlCommand(createTablesAndSeedScript, connection);
            command.ExecuteNonQuery();
        }
    }
}
