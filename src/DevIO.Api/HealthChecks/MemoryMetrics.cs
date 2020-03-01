namespace Restaurante.IO.Api.HealthChecks
{
    public class MemoryMetrics
    {
        public double Total { get; set; }

        public double Used
        {
            get { return Total - Free; }
        }

        public double Free { get; set; }

        public long Duration { get; set; }
    }
}