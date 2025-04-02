using VentusServer.DataAccess.Postgres;

public class PersistenceManager
{
    public PostgresPlayerDAO PlayerDAO { get; }
    public PostgresMapDAO MapDAO { get; }
    public PostgresWorldDAO WorldDAO { get; }

    public PersistenceManager(PostgresPlayerDAO playerDAO, PostgresWorldDAO worldDAO, 
                              PostgresMapDAO mapDAO)
    {
        PlayerDAO = playerDAO;
        MapDAO = mapDAO;
        WorldDAO = worldDAO;
    }
}
