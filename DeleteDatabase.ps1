# DeleteDatabase.ps1

param (
    [string]$Server = "localhost,1433",
    [string]$UserId = "sa",
    [string]$Password = "YourStrongPassw0rd!",
    [string]$DatabaseName = "SneakerCollectionDB"
)

# Build connection string
$connectionString = "Server=$Server;User Id=$UserId;Password=$Password;TrustServerCertificate=true"

# SQL script to delete the database
$sqlScript = @"
IF EXISTS (SELECT * FROM sys.databases WHERE name = '$DatabaseName')
BEGIN
    ALTER DATABASE [$DatabaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$DatabaseName];
    PRINT 'Database $DatabaseName deleted successfully.';
END
ELSE
    PRINT 'Database $DatabaseName does not exist.';
"@

try {
    # Create and open SQL connection
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    Write-Host "Opening connection to $Server..." -ForegroundColor Cyan
    $connection.Open()

    # Execute the script
    $command = $connection.CreateCommand()
    $command.CommandText = $sqlScript
    Write-Host "Executing delete script for $DatabaseName..." -ForegroundColor Cyan
    $result = $command.ExecuteScalar()

    if ($result) {
        Write-Host $result -ForegroundColor Green
    } else {
        Write-Host "No message returned from SQL Server, but operation completed." -ForegroundColor Yellow
    }
}
catch {
    Write-Host "Error deleting database: $_" -ForegroundColor Red
}
finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
        Write-Host "Connection closed." -ForegroundColor Cyan
    }
}