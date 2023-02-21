namespace RankingUp.Core.Configuration
{
    public sealed class Settings
    {
        public DatabaseContext DatabaseContext { get; set; }
    }

    public class DatabaseContext
    {
        public DatabaseType DatabaseType { get; set; }
        public string ConnectionString { get; set; }
    }

    public enum DatabaseType
    {
        MySql = 1, 
        SqlServer = 2,
    }
}
