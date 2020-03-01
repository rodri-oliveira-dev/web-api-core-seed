using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Restaurante.IO.Api.HealthChecks
{
    public class SystemMemoryHealthcheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var metrics = MemoryMetricsClient.GetMetrics();
            var percentUsed = 100 * metrics.Used / metrics.Total;

            var status = HealthStatus.Healthy;
            var message = string.Empty;
            if (percentUsed > 80)
            {
                status = HealthStatus.Degraded;
                message = $"Memory usage above {percentUsed:F2}%";
            }
            if (percentUsed > 95)
            {
                status = HealthStatus.Unhealthy;
                message = $"Memory usage above {percentUsed:F2}%";
            }

            var data = new Dictionary<string, object>
            {
                {"Total", metrics.Total}, {"Used", metrics.Used}, {"Free", metrics.Free}
            };

            var result = new HealthCheckResult(status, message, null, data);

            return await Task.FromResult(result);
        }
    }
}