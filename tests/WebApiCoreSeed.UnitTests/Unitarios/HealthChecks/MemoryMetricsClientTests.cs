using WebApiCoreSeed.Api.HealthChecks;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.HealthChecks;

public sealed class MemoryMetricsClientTests
{
    [Fact(DisplayName = "Memory metrics usa API gerenciada e retorna valores coerentes")]
    [Trait("HealthChecks", "Memory")]
    public void MemoryMetricsQuandoColetadaDeveRetornarValoresNaoNegativos()
    {
        var metrics = MemoryMetricsClient.GetMetrics();

        Assert.True(metrics.Total >= 0);
        Assert.True(metrics.Free >= 0);
        Assert.True(metrics.Used >= 0);
        Assert.True(metrics.Free <= metrics.Total);
        Assert.True(metrics.Duration >= 0);
    }

    [Fact(DisplayName = "Memory metrics calcula memoria usada a partir de total e livre")]
    [Trait("HealthChecks", "Memory")]
    public void MemoryMetricsQuandoCalculaUsedDeveUsarTotalMenosLivre()
    {
        var metrics = new MemoryMetrics
        {
            Total = 1024,
            Free = 384
        };

        Assert.Equal(640, metrics.Used);
    }
}
