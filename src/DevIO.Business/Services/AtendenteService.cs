using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Intefaces.Service;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Validations;

namespace Restaurante.IO.Business.Services
{
    public class AtendenteService : BaseService, IAtendenteService
    {
        private readonly IAtendenteRepository _fornecedorRepository;

        public AtendenteService(IAtendenteRepository fornecedorRepository, 
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task<bool> Adicionar(Atendente atendente)
        {
            if (!ExecutarValidacao(new AtendenteValidation(), atendente) ) return false;

            await _fornecedorRepository.Adicionar(atendente);
            return true;
        }

        public async Task<bool> Atualizar(Atendente atendente)
        {
            if (!ExecutarValidacao(new AtendenteValidation(), atendente)) return false;

            await _fornecedorRepository.Atualizar(atendente);
            return true;
        }

        public async Task<bool> Remover(Guid id)
        {
            await _fornecedorRepository.Remover(id);
            return true;
        }

        public void Dispose()
        {
            _fornecedorRepository?.Dispose();
        }
    }
}