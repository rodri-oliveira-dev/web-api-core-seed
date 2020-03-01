namespace Restaurante.IO.Api.Settings
{
    public class RedisCacheSettings
    {
        public bool Enabled { get; set; }

        public string ConnectionString { get; set; }

        public string InstanceName { get; set; }

        public int DefaultSeconds { get; set; }
    }
}