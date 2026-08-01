namespace WebApiCoreSeed.Api.Settings
{
    public sealed class RequestLimitsSettings
    {
        public int TimeoutSeconds { get; set; } = 30;

        public long MaxRequestBodyBytes { get; set; } = 10 * 1024 * 1024;
    }
}
