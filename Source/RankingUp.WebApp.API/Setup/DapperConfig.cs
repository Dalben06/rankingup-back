using System;
using Dapper;

namespace RankingUp.WebApp.API.Setup;

public static class DapperConfig
{
        public static void RegisterDapperTypeHandlers(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            SqlMapper.AddTypeHandler(new TimeOnlyHandler());
            SqlMapper.AddTypeHandler(new NullableTimeOnlyHandler());
        }
}
