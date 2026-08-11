using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WebApiCoreSeed.Api.Settings;

namespace WebApiCoreSeed.Api.Configuration
{
    public static class OpenTelemetryConfig
    {
        public const string ActivitySourceName = "web-api-core-seed.api";
        public const string MeterName = "web-api-core-seed.api";

        private const string AspNetCoreQueryRedactionSwitch = "OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_DISABLE_URL_QUERY_REDACTION";
        private const string HttpClientQueryRedactionSwitch = "OTEL_DOTNET_EXPERIMENTAL_HTTPCLIENT_DISABLE_URL_QUERY_REDACTION";

        public static IServiceCollection AddApiOpenTelemetry(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            var settings = ReadSettings(configuration, environment);
            services.Configure<OpenTelemetrySettings>(configuration.GetSection(OpenTelemetrySettings.SectionName));

            if (!settings.Enabled)
            {
                return services;
            }

            EnsureUrlQueryRedaction();

            var resourceAttributes = new[]
            {
                new KeyValuePair<string, object>("deployment.environment.name", settings.Environment),
                new KeyValuePair<string, object>("service.namespace", settings.ServiceNamespace)
            };

            var openTelemetry = services
                .AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(
                        serviceName: settings.ServiceName,
                        serviceVersion: settings.ServiceVersion,
                        serviceInstanceId: Environment.MachineName)
                    .AddAttributes(resourceAttributes));

            openTelemetry.WithTracing(tracing =>
            {
                tracing
                    .SetSampler(CreateSampler(settings.Tracing.SamplingRatio))
                    .AddSource(ActivitySourceName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = settings.Tracing.RecordExceptions;
                    })
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                if (ShouldUseOtlp(settings.Otlp))
                {
                    tracing.AddOtlpExporter(options => ConfigureOtlp(options, settings.Otlp));
                }
            });

            openTelemetry.WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(MeterName)
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("System.Net.Http")
                    .AddMeter("System.Net.NameResolution")
                    .AddMeter("Microsoft.EntityFrameworkCore")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (settings.Metrics.RuntimeInstrumentation)
                {
                    metrics.AddRuntimeInstrumentation();
                }

                if (ShouldUseOtlp(settings.Otlp))
                {
                    metrics.AddOtlpExporter(options => ConfigureOtlp(options, settings.Otlp));
                }
            });

            return services;
        }

        private static OpenTelemetrySettings ReadSettings(IConfiguration configuration, IHostEnvironment environment)
        {
            var settings = new OpenTelemetrySettings();
            configuration.GetSection(OpenTelemetrySettings.SectionName).Bind(settings);

            settings.ServiceName = FirstNonEmpty(
                configuration["OTEL_SERVICE_NAME"],
                settings.ServiceName,
                "web-api-core-seed-api");
            settings.ServiceNamespace = FirstNonEmpty(settings.ServiceNamespace, "rodri-oliveira-dev.web-api-core-seed");
            settings.ServiceVersion = FirstNonEmpty(settings.ServiceVersion, GetAssemblyVersion());
            settings.Environment = FirstNonEmpty(settings.Environment, environment.EnvironmentName);
            settings.Tracing.SamplingRatio = Math.Clamp(settings.Tracing.SamplingRatio, 0.0D, 1.0D);

            var otlpEndpoint = FirstNonEmpty(
                settings.Otlp.Endpoint,
                configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            settings.Otlp.Endpoint = otlpEndpoint;

            var otlpProtocol = FirstNonEmpty(
                settings.Otlp.Protocol,
                configuration["OTEL_EXPORTER_OTLP_PROTOCOL"],
                "Grpc");
            settings.Otlp.Protocol = otlpProtocol;

            return settings;
        }

        private static ParentBasedSampler CreateSampler(double samplingRatio)
        {
            return new ParentBasedSampler(new TraceIdRatioBasedSampler(samplingRatio));
        }

        private static bool ShouldUseOtlp(OpenTelemetryOtlpSettings settings)
        {
            return settings.Enabled || !string.IsNullOrWhiteSpace(settings.Endpoint);
        }

        private static void ConfigureOtlp(OtlpExporterOptions options, OpenTelemetryOtlpSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.Endpoint))
            {
                options.Endpoint = new Uri(settings.Endpoint, UriKind.Absolute);
            }

            options.Protocol = ParseProtocol(settings.Protocol);
        }

        private static OtlpExportProtocol ParseProtocol(string value)
        {
            return string.Equals(value, "HttpProtobuf", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "http/protobuf", StringComparison.OrdinalIgnoreCase)
                ? OtlpExportProtocol.HttpProtobuf
                : OtlpExportProtocol.Grpc;
        }

        private static void EnsureUrlQueryRedaction()
        {
            ForceEnvironmentValue(AspNetCoreQueryRedactionSwitch, "false");
            ForceEnvironmentValue(HttpClientQueryRedactionSwitch, "false");
        }

        private static void ForceEnvironmentValue(string key, string value)
        {
            Environment.SetEnvironmentVariable(key, value);
        }

        private static string FirstNonEmpty(params string?[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private static string GetAssemblyVersion()
        {
            var assembly = Assembly.GetEntryAssembly() ?? typeof(OpenTelemetryConfig).Assembly;
            return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version?.ToString()
                ?? "0.0.0";
        }
    }
}
