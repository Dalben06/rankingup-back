using Microsoft.Extensions.Diagnostics.HealthChecks;
using RankingUp.Core.Data;
using System.Data.Common;

namespace RankingUp.WebApp.API.Setup;

public sealed class DatabaseHealthCheck(DbFactory dbFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = dbFactory.GetNewConnection();

            if (connection is DbConnection dbConnection)
                await dbConnection.OpenAsync(cancellationToken);
            else
                connection.Open();

            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Database connection failed.", exception);
        }
    }
}
