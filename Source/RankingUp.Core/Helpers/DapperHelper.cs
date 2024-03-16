using Dapper.Contrib.Extensions;
using System.Reflection;

namespace RankingUp.Core.Helpers
{
    public static class DapperHelper
    {
        public static string GetPropertyKeyName<T>() 
        {
            return typeof(T).GetProperties().FirstOrDefault( prop => prop.CustomAttributes.Any( key => key.AttributeType == typeof(KeyAttribute))).Name;        
        }

        public static string GetTableName<T>()
        {
            var tableName = typeof(T)?.GetCustomAttribute<TableAttribute>()?.Name;
            if (tableName == null)
                tableName = typeof(T).Name;

            if (tableName.Substring(tableName.Length - 1, 1)[0].ToString().ToLower() != "s")
                tableName += "s";

            return tableName;
        }

    }
}
