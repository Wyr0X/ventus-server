using System.Data;

namespace VentusServer.DataAccess.Dapper
{
    public abstract class BaseDAO
    {
        protected readonly IDbConnectionFactory _connectionFactory;

        protected BaseDAO(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        protected IDbConnection GetConnection()
        {
            return _connectionFactory.CreateConnection();
        }
    }
}
