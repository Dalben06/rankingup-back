using MySqlConnector;
using System.Data;

namespace RankingUp.Core.Data
{
    public class MySqlServerStrategy : IDbStrategy
    {
        public IDbConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
