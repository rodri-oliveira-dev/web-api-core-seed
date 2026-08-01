using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;
using System;
using System.Threading.Tasks;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class AtendenteService : BaseService, IAtendenteService
    {
        private readonly IAtendenteRepository _atendenteRepository;

        public AtendenteService(IAtendenteRepository atendenteRepository,
                                 INotificador notificador) : base(notificador)
        {
            _atendenteRepository = atendenteRepository;
        }

        public async Task<bool> Adicionar(Atendente atendente)
        {
            if (!ExecutarValidacao(new AtendenteValidation(), atendente)) return false;

            await _atendenteRepository.Adicionar(atendente);
            return true;
        }

        public async Task<bool> Atualizar(Atendente atendente)
        {
            if (!ExecutarValidacao(new AtendenteValidation(), atendente)) return false;

            await _atendenteRepository.Atualizar(atendente);
            return true;
        }

        public async Task<bool> Remover(Guid id)
        {
            await _atendenteRepository.RemoverPorId(id);
            return true;
        }

        public void Dispose()
        {
            _atendenteRepository?.Dispose();
        }
    }
}
