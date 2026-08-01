using Moq;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Core;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;
using WebApiCoreSeed.SampleRestaurant.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WebApiCoreSeed.Tests.Unitarios.Services
{
    public class AtendenteServiceTest
    {
        private readonly Mock<IAtendenteRepository> _atendenteRepository;
        private readonly Mock<ISampleRestaurantUnitOfWork> _unitOfWork;
        private readonly Notificador _notificador;

        public AtendenteServiceTest()
        {
            _atendenteRepository = new Mock<IAtendenteRepository>(MockBehavior.Strict);
            _unitOfWork = new Mock<ISampleRestaurantUnitOfWork>(MockBehavior.Strict);
            _notificador = new Notificador();
        }

        [Fact(DisplayName = "Atendente cadastrado com sucesso")]
        [Trait("Services", "Atendente")]
        public async Task AdicionarQuandoAtendenteValidoDeveCadastrarAtendente()
        {
            //Arrange
            var atendente = CriarAtendenteValido();
            _atendenteRepository.Setup(r => r.Adicionar(atendente, default)).Returns(Task.CompletedTask);
            _unitOfWork.Setup(unitOfWork => unitOfWork.CommitAsync(default)).ReturnsAsync(1);
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Adicionar(atendente);

            //Assert
            Assert.True(retorno);
            Assert.False(_notificador.TemNotificacao());
            _atendenteRepository.Verify(r => r.Adicionar(atendente, default), Times.Once);
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(default), Times.Once);
            _atendenteRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente erro na validacao ao cadastrar")]
        [Trait("Services", "Atendente")]
        public async Task AdicionarQuandoAtendenteInvalidoDeveNotificarENaoCadastrar()
        {
            //Arrange
            var atendente = new Atendente();
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Adicionar(atendente);

            //Assert
            Assert.False(retorno);
            Assert.True(_notificador.TemNotificacao());
            Assert.NotEmpty(_notificador.ObterNotificacoes());
            _atendenteRepository.Verify(r => r.Adicionar(atendente, default), Times.Never);
            _atendenteRepository.VerifyNoOtherCalls();
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(default), Times.Never);
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente alterado com sucesso")]
        [Trait("Services", "Atendente")]
        public async Task AtualizarQuandoAtendenteValidoDeveAtualizarAtendente()
        {
            //Arrange
            var atendente = CriarAtendenteValido();
            _atendenteRepository.Setup(r => r.Atualizar(atendente, default)).Returns(Task.CompletedTask);
            _unitOfWork.Setup(unitOfWork => unitOfWork.CommitAsync(default)).ReturnsAsync(1);
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Atualizar(atendente);

            //Assert
            Assert.True(retorno);
            Assert.False(_notificador.TemNotificacao());
            _atendenteRepository.Verify(r => r.Atualizar(atendente, default), Times.Once);
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(default), Times.Once);
            _atendenteRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente erro na validacao ao alterar")]
        [Trait("Services", "Atendente")]
        public async Task AtualizarQuandoAtendenteInvalidoDeveNotificarENaoAtualizar()
        {
            //Arrange
            var atendente = new Atendente();
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Atualizar(atendente);

            //Assert
            Assert.False(retorno);
            Assert.True(_notificador.TemNotificacao());
            Assert.NotEmpty(_notificador.ObterNotificacoes());
            _atendenteRepository.Verify(r => r.Adicionar(atendente, default), Times.Never);
            _atendenteRepository.Verify(r => r.Atualizar(atendente, default), Times.Never);
            _atendenteRepository.VerifyNoOtherCalls();
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(default), Times.Never);
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente removido com sucesso")]
        [Trait("Services", "Atendente")]
        public async Task RemoverQuandoIdInformadoDeveRemoverAtendente()
        {
            //Arrange
            var id = Guid.Parse("6b0bb7e2-49ca-4b02-bff4-e6fed1007391");
            _atendenteRepository.Setup(r => r.RemoverPorId(id, default)).Returns(Task.CompletedTask);
            _unitOfWork.Setup(unitOfWork => unitOfWork.CommitAsync(default)).ReturnsAsync(1);
            var atendenteService = CriarService();

            //Act
            var retorno = await atendenteService.Remover(id);

            //Assert
            Assert.True(retorno);
            Assert.False(_notificador.TemNotificacao());
            _atendenteRepository.Verify(r => r.RemoverPorId(id, default), Times.Once);
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(default), Times.Once);
            _atendenteRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente propaga erro do commit")]
        [Trait("Services", "Atendente")]
        public async Task AdicionarQuandoCommitFalhaDevePropagarExcecao()
        {
            //Arrange
            var atendente = CriarAtendenteValido();
            var exception = new InvalidOperationException("commit failed");
            _atendenteRepository.Setup(r => r.Adicionar(atendente, default)).Returns(Task.CompletedTask);
            _unitOfWork.Setup(unitOfWork => unitOfWork.CommitAsync(default)).ThrowsAsync(exception);
            var atendenteService = CriarService();

            //Act
            var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => atendenteService.Adicionar(atendente));

            //Assert
            Assert.Same(exception, actual);
            _atendenteRepository.Verify(r => r.Adicionar(atendente, default), Times.Once);
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(default), Times.Once);
            _atendenteRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente com token ja cancelado nao chama dependencias")]
        [Trait("Services", "Atendente")]
        public async Task AdicionarQuandoTokenJaCanceladoNaoDeveChamarDependencias()
        {
            //Arrange
            var atendente = CriarAtendenteValido();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var atendenteService = CriarService();

            //Act
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                atendenteService.Adicionar(atendente, cancellationTokenSource.Token));

            //Assert
            _atendenteRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente propaga cancelamento do commit")]
        [Trait("Services", "Atendente")]
        public async Task AdicionarQuandoCommitCanceladoDevePropagarCancelamento()
        {
            //Arrange
            var atendente = CriarAtendenteValido();
            using var cancellationTokenSource = new CancellationTokenSource();
            _atendenteRepository.Setup(r => r.Adicionar(atendente, cancellationTokenSource.Token)).Returns(Task.CompletedTask);
            _unitOfWork
                .Setup(unitOfWork => unitOfWork.CommitAsync(cancellationTokenSource.Token))
                .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));
            var atendenteService = CriarService();

            //Act
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                atendenteService.Adicionar(atendente, cancellationTokenSource.Token));

            //Assert
            _atendenteRepository.Verify(r => r.Adicionar(atendente, cancellationTokenSource.Token), Times.Once);
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(cancellationTokenSource.Token), Times.Once);
            _atendenteRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Atendente cancelado no repositorio nao executa commit")]
        [Trait("Services", "Atendente")]
        public async Task AdicionarQuandoRepositorioCancelaNaoDeveExecutarCommit()
        {
            //Arrange
            var atendente = CriarAtendenteValido();
            using var cancellationTokenSource = new CancellationTokenSource();
            _atendenteRepository
                .Setup(r => r.Adicionar(atendente, cancellationTokenSource.Token))
                .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));
            var atendenteService = CriarService();

            //Act
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                atendenteService.Adicionar(atendente, cancellationTokenSource.Token));

            //Assert
            _atendenteRepository.Verify(r => r.Adicionar(atendente, cancellationTokenSource.Token), Times.Once);
            _unitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(cancellationTokenSource.Token), Times.Never);
            _atendenteRepository.VerifyNoOtherCalls();
            _unitOfWork.VerifyNoOtherCalls();
        }

        private AtendenteService CriarService()
        {
            return new AtendenteService(_atendenteRepository.Object, _unitOfWork.Object, _notificador);
        }

        private static Atendente CriarAtendenteValido()
        {
            return new Atendente
            {
                Nome = "Rodrigo",
                Email = "rodrigodotnet@gmail.com",
                Telefone = new Telefone
                {
                    Ddd = 19,
                    Numero = 998861788,
                    TipoTelefone = ETipoTelefone.Celular
                },
                TipoAtendente = ETipoAtendente.Garcom
            };
        }
    }
}
