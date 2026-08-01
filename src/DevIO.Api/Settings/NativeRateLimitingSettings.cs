namespace Restaurante.IO.Api.Settings
{
    public sealed class NativeRateLimitingSettings
    {
        public NativeRateLimitPolicySettings Public { get; set; } = new NativeRateLimitPolicySettings
        {
            PermitLimit = 3,
            WindowSeconds = 1,
            QueueLimit = 0
        };

        public NativeRateLimitPolicySettings Authenticated { get; set; } = new NativeRateLimitPolicySettings
        {
            PermitLimit = 3,
            WindowSeconds = 1,
            QueueLimit = 0
        };

        public NativeRateLimitPolicySettings AuthenticationSensitive { get; set; } = new NativeRateLimitPolicySettings
        {
            PermitLimit = 2,
            WindowSeconds = 1,
            QueueLimit = 0
        };
    }

    public sealed class NativeRateLimitPolicySettings
    {
        public int PermitLimit { get; set; } = 3;

        public int WindowSeconds { get; set; } = 1;

        public int QueueLimit { get; set; }
    }
}
