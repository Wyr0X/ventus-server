namespace ventus_server.Startup
{
    public class GameConfiguration
    {
        public required string CredentialsPath { get; init; }
        public required string PostgresConnectionString { get; init; }
    }
}
