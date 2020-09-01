using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Intefaces.Service;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Validations;
using System;
using System.Threading.Tasks;

namespace Restaurante.IO.Business.Services
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
            await _atendenteRepository.Remover(id);
            return true;
        }

        public void Dispose()
        {
            _atendenteRepository?.Dispose();
        }
    }
}