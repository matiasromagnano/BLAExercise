using BLAExercise.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLAExercise.Tests.Infrastructure
{
    public class BaseRepositoryTests
    {
        public const string TestDatabaseName = "SneakerCollectionTestDB";
        public const string SqlServerConnectionString = "Server=localhost,1433;User Id=sa;Password=YourStrongPassw0rd!;TrustServerCertificate=true";
        public const string SqlFullConnectionString = $"{SqlServerConnectionString};Database={TestDatabaseName}";
        public BaseRepositoryTests()
        {
            var dbCreator = new DatabaseCreator(SqlServerConnectionString);
            dbCreator.CreateDatabaseAndTables(TestDatabaseName);
        }
    }
}
