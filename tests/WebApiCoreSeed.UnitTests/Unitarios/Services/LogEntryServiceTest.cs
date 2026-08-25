using Moq;
using WebApiCoreSeed.SampleRestaurant.Interfaces;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;
using WebApiCoreSeed.SampleRestaurant.Services;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Services
{
    public class LogEntryServiceTest
    {
        private readonly Mock<ILogEntryRepository> _logEntryRepository;
        private readonly Mock<ISampleRestaurantUnitOfWork> _unitOfWork;
        private readonly Notificador _notificador;

        public LogEntryServiceTest()
        {
            _logEntryRepository = new Mock<ILogEntryRepository>(MockBehavior.Strict);
            _unitOfWork = new Mock<ISampleRestaurantUnitOfWork>(MockBehavior.Strict);
            _notificador = new Notificador();
        }

        [Fact(DisplayName = "LogEntry valido registra e confirma Unit of Work")]
        [Trait("Services", "LogEntry")]
        public async Task AdicionarQuandoLogEntryValidoDeveRegistrarEConfirmar()
        {
            var logEntry = CreateValidLogEntry();
            _logEntryRepository.Setup(repository => repository.Registrar(logEntry, default)).Returns(Task.CompletedTask);
            _unitOfWork.Setup(unitOfWork => unitOfWork.CommitAsync(default)).ReturnsAsync(1);
            var service = CreateService();

            var result = await service.Adicionar(logEntry);

            Assert.True(result);
            Assert.False(_notificador.TemNotificacao());
            _logEntryRepository.Verify(repository => repository.Registrar(logEntry, default), Times.Once);
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(default), Times.Once);
            _logEntryRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "LogEntry invalido notifica e nao registra")]
        [Trait("Services", "LogEntry")]
        public async Task AdicionarQuandoLogEntryInvalidoDeveNotificarENaoRegistrar()
        {
            var logEntry = new LogEntry { Message = string.Empty };
            var service = CreateService();

            var result = await service.Adicionar(logEntry);

            Assert.False(result);
            Assert.True(_notificador.TemNotificacao());
            _logEntryRepository.Verify(repository => repository.Registrar(logEntry, default), Times.Never);
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(default), Times.Never);
            _logEntryRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "LogEntry com token cancelado nao chama dependencias")]
        [Trait("Services", "LogEntry")]
        public async Task AdicionarQuandoTokenCanceladoNaoDeveChamarDependencias()
        {
            var logEntry = CreateValidLogEntry();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var service = CreateService();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                service.Adicionar(logEntry, cancellationTokenSource.Token));

            _logEntryRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        private LogEntryService CreateService()
        {
            return new LogEntryService(_logEntryRepository.Object, _unitOfWork.Object, _notificador);
        }

        private static LogEntry CreateValidLogEntry()
        {
            return new LogEntry { Message = "Evento registrado" };
        }
    }
}
