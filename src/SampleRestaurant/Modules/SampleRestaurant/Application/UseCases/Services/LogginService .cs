using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class LogginService : BaseService, ILogginService
    {
        private readonly ILogginRepository _logginRepository;

        public LogginService(ILogginRepository logginRepository, 
                                 INotificador notificador) : base(notificador)
        {
            _logginRepository = logginRepository;
        }

        public async Task<bool> Adicionar(LogginEntity mesa)
        {
            if (!ExecutarValidacao(new LogginValidation(), mesa) ) return false;

            await _logginRepository.Adicionar(mesa);
            return true;
        }

        public void Dispose()
        {
            _logginRepository?.Dispose();
        }
    }
}