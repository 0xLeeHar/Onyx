using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Onyx.Services.Products.Store;

// A database health check for the products "service"
public class DatabaseHealthChecks(ProductsContext databaseContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var isHealthy = await databaseContext.Database.CanConnectAsync(cancellationToken);
        
        return isHealthy
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Failed to connect to the database");
    }
}