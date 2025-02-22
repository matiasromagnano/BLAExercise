namespace BLAExercise.Core.Configuration
{
    public class ApplicationOptions
    {
        public string? SqlConnectionString { get; set; }
        public string? JWTSecretKey { get; set; }
    }
}
