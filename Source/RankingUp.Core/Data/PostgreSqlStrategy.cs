using Npgsql;
using System.Data;

namespace RankingUp.Core.Data
{
    /// <summary>
    /// PostgreSQL connection strategy. Reserved for future migration from MySQL.
    /// When activating, remove the MySqlGuidTypeConfig Dapper handler (PostgreSQL has native UUID support).
    /// </summary>
    public class PostgreSqlStrategy : IDbStrategy
    {
        public IDbConnection GetConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
