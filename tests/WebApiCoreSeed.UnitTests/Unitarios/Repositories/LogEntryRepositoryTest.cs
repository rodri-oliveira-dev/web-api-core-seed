using Microsoft.EntityFrameworkCore;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Repositories
{
    public class LogEntryRepositoryTest
    {
        [Fact(DisplayName = "Repositorio LogEntry registra entidade no DbContext")]
        [Trait("Repositories", "LogEntry")]
        public async Task RegistrarQuandoLogEntryInformadoDeveAdicionarAoContexto()
        {
            await using var context = CreateContext();
            var repository = new LogEntryRepository(context);
            var logEntry = new LogEntry { Message = "Evento registrado" };

            await repository.Registrar(logEntry);
            await context.SaveChangesAsync();

            var persisted = await context.LogEntries.SingleAsync(log => log.Id == logEntry.Id);
            Assert.Equal(logEntry.Message, persisted.Message);
        }

        [Fact(DisplayName = "Repositorio LogEntry respeita token cancelado")]
        [Trait("Repositories", "LogEntry")]
        public async Task RegistrarQuandoTokenCanceladoDeveInterromperAntesDeAdicionar()
        {
            await using var context = CreateContext();
            var repository = new LogEntryRepository(context);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                repository.Registrar(new LogEntry { Message = "Evento registrado" }, cancellationTokenSource.Token));

            Assert.Empty(context.LogEntries);
        }

        private static SampleRestaurantDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SampleRestaurantDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new SampleRestaurantDbContext(options);
        }
    }
}
