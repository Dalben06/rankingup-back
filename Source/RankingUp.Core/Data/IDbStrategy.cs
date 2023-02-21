using System.Data;

namespace RankingUp.Core.Data
{
    public interface IDbStrategy
    {
        IDbConnection GetConnection(string connectionString);
    }
}
