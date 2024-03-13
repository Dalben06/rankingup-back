using RankingUp.Core.Configuration;
using System.Data;

namespace RankingUp.Core.Data
{
    public class DbFactory
    {
        private readonly string _connectionString;
        private readonly DatabaseType _provider;
        public DbFactory(Settings OptionsSettings)
        {
            this._connectionString = OptionsSettings?.DatabaseContext?.ConnectionString ?? "";
            this._provider = OptionsSettings?.DatabaseContext.DatabaseType ?? DatabaseType.MySql ;
        }

        public IDbConnection GetConnection()
        {
            return this._provider switch
            {
                DatabaseType.SqlServer => new SqlServerStrategy().GetConnection(this._connectionString),
                DatabaseType.MySql => new MySqlServerStrategy().GetConnection(this._connectionString),
                _ => null
            };

        }
        public IDbConnection GetNewConnection()
        {
            return this._provider switch
            {
                DatabaseType.SqlServer => new SqlServerStrategy().GetConnection(this._connectionString),
                DatabaseType.MySql => new MySqlServerStrategy().GetConnection(this._connectionString),
                _ => null
            };
        }

    }
}
