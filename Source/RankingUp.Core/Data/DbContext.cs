using System.Data;

namespace RankingUp.Core.Data
{
    public class DbContext
    {
        private readonly DbFactory _dbFactory;
        public IDbConnection DbConnection { get; set; }
        public DbContext(DbFactory dbFactory)
        {
            _dbFactory = dbFactory;
            this.DbConnection = _dbFactory.GetConnection();
        }

        public IDbConnection NewConnection { get => _dbFactory.GetNewConnection(); }

    }
}
