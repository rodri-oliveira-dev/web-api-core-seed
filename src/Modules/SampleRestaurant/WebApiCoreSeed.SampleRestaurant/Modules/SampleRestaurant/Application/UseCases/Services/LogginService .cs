using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class LogginService : BaseService, ILogginService
    {
        private readonly ILogginRepository _logginRepository;
        private readonly ISampleRestaurantUnitOfWork _unitOfWork;

        public LogginService(ILogginRepository logginRepository,
                                 ISampleRestaurantUnitOfWork unitOfWork,
                                 INotificador notificador) : base(notificador)
        {
            _logginRepository = logginRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Adicionar(LogginEntity mesa, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new LogginValidation(), mesa)) return false;

            await _logginRepository.Registrar(mesa, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        public void Dispose()
        {
            _logginRepository?.Dispose();
        }
    }
}
