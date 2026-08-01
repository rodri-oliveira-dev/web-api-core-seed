using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApiCoreSeed.Api;
using WebApiCoreSeed.Api.DataContext;
using WebApiCoreSeed.Api.Services.Interfaces;
using WebApiCoreSeed.Api.Settings;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using Xunit;

namespace WebApiCoreSeed.Tests.Integracao
{
    public sealed class ObservabilityConfigurationTests
    {
        [Fact]
        public async Task AplicacaoDeveIniciarComOpenTelemetryDesativado()
        {
            using var factory = new ObservabilityApiFactory(new Dictionary<string, string>
            {
                ["OpenTelemetry:Enabled"] = "false"
            });
            using var client = factory.CreateApiClient();

            var response = await client.GetAsync("/hc");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AplicacaoDeveIniciarComOtlpConfiguradoSemCollectorDisponivel()
        {
            using var factory = new ObservabilityApiFactory(new Dictionary<string, string>
            {
                ["OpenTelemetry:Enabled"] = "true",
                ["OpenTelemetry:Otlp:Enabled"] = "true",
                ["OpenTelemetry:Otlp:Endpoint"] = "http://127.0.0.1:4317",
                ["OpenTelemetry:Otlp:Protocol"] = "Grpc"
            });
            using var client = factory.CreateApiClient();

            var response = await client.GetAsync("/hc");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RequestAspNetCoreDeveProduzirSpanServidor()
        {
            using var factory = new ObservabilityApiFactory();
            using var client = factory.CreateApiClient();
            var activities = new ConcurrentBag<Activity>();

            using var listener = CreateActivityListener(activities);

            var response = await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(activities, activity =>
                activity.Kind == ActivityKind.Server
                && activity.Source.Name.Contains("AspNetCore", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task LogsDevemConterTraceIdESpanId()
        {
            using var factory = new ObservabilityApiFactory();
            using var client = factory.CreateApiClient();

            var response = await client.GetAsync("/api/v1/Pratos?pageNumber=1&pageSize=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var logs = await factory.ReadLogsAsync();
            Assert.Matches("TraceId=[0-9a-f]{32}", logs);
            Assert.Matches("SpanId=[0-9a-f]{16}", logs);
        }

        [Fact]
        public async Task TelemetriaNaoDeveConterValoresSensiveisDeQueryOuAuthorization()
        {
            using var factory = new ObservabilityApiFactory();
            using var client = factory.CreateApiClient();
            var activities = new ConcurrentBag<Activity>();
            var token = $"secret-token-{Guid.NewGuid():N}";
            var password = $"secret-password-{Guid.NewGuid():N}";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var listener = CreateActivityListener(activities);

            var response = await client.GetAsync($"/api/v1/Pratos?pageNumber=1&pageSize=10&access_token={token}&password={password}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var serializedTags = string.Join(
                Environment.NewLine,
                activities.SelectMany(activity => activity.Tags).Select(tag => $"{tag.Key}={tag.Value}"));

            Assert.DoesNotContain(token, serializedTags, StringComparison.Ordinal);
            Assert.DoesNotContain(password, serializedTags, StringComparison.Ordinal);
        }

        private static ActivityListener CreateActivityListener(ConcurrentBag<Activity> activities)
        {
            var listener = new ActivityListener
            {
                ShouldListenTo = source =>
                    source.Name.Contains("AspNetCore", StringComparison.OrdinalIgnoreCase)
                    || source.Name.Contains("EntityFrameworkCore", StringComparison.OrdinalIgnoreCase)
                    || source.Name.Contains("Http", StringComparison.OrdinalIgnoreCase),
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStopped = activities.Add
            };

            ActivitySource.AddActivityListener(listener);
            return listener;
        }

        private sealed class ObservabilityApiFactory : WebApplicationFactory<Program>
        {
            private readonly IReadOnlyDictionary<string, string> _configurationOverrides;
            private readonly string _databaseName = Guid.NewGuid().ToString();
            private readonly string _serilogFilePath = Path.Combine(
                Path.GetTempPath(),
                "web-api-core-seed",
                $"observability-{Guid.NewGuid():N}.log");

            public ObservabilityApiFactory(IReadOnlyDictionary<string, string> configurationOverrides = null)
            {
                _configurationOverrides = configurationOverrides ?? new Dictionary<string, string>();
            }

            public HttpClient CreateApiClient()
            {
                return CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                    BaseAddress = new Uri("https://localhost")
                });
            }

            public async Task<string> ReadLogsAsync()
            {
                var deadline = DateTimeOffset.UtcNow.AddSeconds(5);

                while (DateTimeOffset.UtcNow < deadline)
                {
                    if (File.Exists(_serilogFilePath) && new FileInfo(_serilogFilePath).Length > 0)
                    {
                        await using var stream = new FileStream(_serilogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var reader = new StreamReader(stream);

                        return await reader.ReadToEndAsync();
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }

                return string.Empty;
            }

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureAppConfiguration((_, configuration) =>
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_serilogFilePath)!);

                    var values = new Dictionary<string, string>
                    {
                        ["ConnectionStrings:DefaultConnection"] = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={_databaseName};Integrated Security=True;",
                        ["AppSettings:Secret"] = "X-BURGUER@COCA-2-OBSERVABILITY-TEST-SECRET-2026",
                        ["RedisCacheSettings:Enabled"] = "false",
                        ["SeqSettings:Enabled"] = "false",
                        ["SeqSettings:Url"] = "http://localhost",
                        ["SeqSettings:FilePath"] = _serilogFilePath,
                        ["OpenTelemetry:Enabled"] = "true",
                        ["OpenTelemetry:ServiceName"] = "web-api-core-seed-api-tests",
                        ["OpenTelemetry:ServiceNamespace"] = "rodri-oliveira-dev.web-api-core-seed",
                        ["OpenTelemetry:Environment"] = "Testing",
                        ["OpenTelemetry:Otlp:Enabled"] = "false"
                    };

                    foreach (var (key, value) in _configurationOverrides)
                    {
                        values[key] = value;
                    }

                    configuration.AddInMemoryCollection(values);
                });

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<SampleRestaurantDbContext>();
                    services.RemoveAll<ApplicationDbContext>();
                    services.RemoveAll<DbContextOptions<SampleRestaurantDbContext>>();
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

                    services.AddDbContext<SampleRestaurantDbContext>(options => options.UseInMemoryDatabase(_databaseName));
                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(_databaseName + "-identity"));
                    services.PostConfigure<HealthCheckServiceOptions>(options => options.Registrations.Clear());
                    services.RemoveAll<RedisCacheSettings>();
                    services.RemoveAll<IResponseCacheService>();
                    services.AddSingleton(new RedisCacheSettings { Enabled = false });
                    services.RemoveAll<IPratoRepository>();
                    services.AddScoped<IPratoRepository, FakePratoRepository>();
                });
            }
        }

        private abstract class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
        {
            public Task<int> Adicionar(TEntity entity) => Task.FromResult(1);

            public Task<TEntity> ObterPorId(Guid id) => Task.FromResult<TEntity>(null);

            public Task<List<TEntity>> ObterTodos() => Task.FromResult(new List<TEntity>());

            public Task<int> TotalRegistros() => Task.FromResult(0);

            public Task<IEnumerable<TEntity>> Paginacao(PaginationParameter paginationParameter) => Task.FromResult<IEnumerable<TEntity>>(Array.Empty<TEntity>());

            public Task<int> Atualizar(TEntity entity) => Task.FromResult(1);

            public Task<int> Remover(Guid id) => Task.FromResult(1);

            public Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate) => Task.FromResult<IEnumerable<TEntity>>(Array.Empty<TEntity>());

            public Task<int> SaveChanges() => Task.FromResult(1);

            public void Dispose()
            {
            }
        }

        private sealed class FakePratoRepository : FakeRepository<Prato>, IPratoRepository
        {
        }
    }
}
