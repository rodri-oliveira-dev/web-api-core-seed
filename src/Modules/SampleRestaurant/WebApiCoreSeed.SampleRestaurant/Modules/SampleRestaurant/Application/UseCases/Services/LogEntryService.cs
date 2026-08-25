using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Interfaces;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class LogEntryService : BaseService, ILogEntryService
    {
        private readonly ILogEntryRepository _logEntryRepository;
        private readonly ISampleRestaurantUnitOfWork _unitOfWork;

        public LogEntryService(ILogEntryRepository logEntryRepository,
                                 ISampleRestaurantUnitOfWork unitOfWork,
                                 INotificador notificador) : base(notificador)
        {
            _logEntryRepository = logEntryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Adicionar(LogEntry logEntry, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ExecutarValidacao(new LogEntryValidation(), logEntry)) return false;

            await _logEntryRepository.Registrar(logEntry, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

    }
}
