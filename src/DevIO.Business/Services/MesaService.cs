using System;
using System.Threading.Tasks;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Intefaces.Repository;
using Restaurante.IO.Business.Intefaces.Service;
using Restaurante.IO.Business.Models;
using Restaurante.IO.Business.Models.Validations;

namespace Restaurante.IO.Business.Services
{
    public class MesaService : BaseService, IMesaService
    {
        private readonly IMesaRepository _fornecedorRepository;

        public MesaService(IMesaRepository fornecedorRepository, 
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task<bool> Adicionar(Mesa mesa)
        {
            if (!ExecutarValidacao(new MesaValidation(), mesa) ) return false;

            await _fornecedorRepository.Adicionar(mesa);
            return true;
        }

        public async Task<bool> Atualizar(Mesa mesa)
        {
            if (!ExecutarValidacao(new MesaValidation(), mesa)) return false;

            await _fornecedorRepository.Atualizar(mesa);
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