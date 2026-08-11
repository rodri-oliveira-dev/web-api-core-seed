using System;
using System.Diagnostics;

namespace WebApiCoreSeed.Api.HealthChecks
{
    public static class MemoryMetricsClient
    {
        public static MemoryMetrics GetMetrics()
        {
            var watch = Stopwatch.StartNew();
            var metrics = GetManagedRuntimeMetrics();
            watch.Stop();
            metrics.Duration = watch.ElapsedMilliseconds;

            return metrics;
        }

        private static MemoryMetrics GetManagedRuntimeMetrics()
        {
            var totalAvailableBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            if (totalAvailableBytes <= 0)
            {
                return new MemoryMetrics();
            }

            var usedBytes = Environment.WorkingSet;
            var freeBytes = Math.Max(0, totalAvailableBytes - usedBytes);
            return new MemoryMetrics
            {
                Total = BytesToMegabytes(totalAvailableBytes),
                Free = BytesToMegabytes(freeBytes)
            };
        }

        private static double BytesToMegabytes(long bytes)
        {
            return Math.Round(bytes / 1024d / 1024d, 0);
        }
    }
}
