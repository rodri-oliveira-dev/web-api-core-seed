namespace WebApiCoreSeed.Api.Settings
{
    public class RedisCacheSettings
    {
        public bool Enabled { get; set; }

        public string ConnectionString { get; set; } = string.Empty;

        public string InstanceName { get; set; } = string.Empty;

        public int DefaultSeconds { get; set; }
    }
}
