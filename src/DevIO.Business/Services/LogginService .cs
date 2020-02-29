using System.Threading.Tasks;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Intefaces.Service;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Validations;

namespace Restaurante.IO.Business.Services
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