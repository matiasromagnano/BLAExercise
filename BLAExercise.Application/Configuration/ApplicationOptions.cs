namespace BLAExercise.Application.Configuration
{
    public class ApplicationOptions
    {
        public string? DatabaseName { get; set; }
        public string? SqlServerConnectionString { get; set; }
        public string? JWTSecretKey { get; set; }

        public string GetFullConnectionString()
        {
            return $"{SqlServerConnectionString};Database={DatabaseName}";
        }
    }
}
