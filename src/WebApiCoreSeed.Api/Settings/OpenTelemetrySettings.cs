namespace WebApiCoreSeed.Api.Settings
{
    public sealed class OpenTelemetrySettings
    {
        public const string SectionName = "OpenTelemetry";

        public bool Enabled { get; set; } = true;

        public string ServiceName { get; set; } = "web-api-core-seed-api";

        public string ServiceNamespace { get; set; } = "rodri-oliveira-dev.web-api-core-seed";

        public string ServiceVersion { get; set; } = string.Empty;

        public string Environment { get; set; } = string.Empty;

        public OpenTelemetryTracingSettings Tracing { get; set; } = new OpenTelemetryTracingSettings();

        public OpenTelemetryMetricsSettings Metrics { get; set; } = new OpenTelemetryMetricsSettings();

        public OpenTelemetryOtlpSettings Otlp { get; set; } = new OpenTelemetryOtlpSettings();
    }

    public sealed class OpenTelemetryTracingSettings
    {
        public double SamplingRatio { get; set; } = 1.0D;

        public bool RecordExceptions { get; set; } = true;
    }

    public sealed class OpenTelemetryMetricsSettings
    {
        public bool RuntimeInstrumentation { get; set; } = true;
    }

    public sealed class OpenTelemetryOtlpSettings
    {
        public bool Enabled { get; set; }

        public string Endpoint { get; set; } = string.Empty;

        public string Protocol { get; set; } = "Grpc";
    }
}
