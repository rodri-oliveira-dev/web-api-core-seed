using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;

namespace WebApiCoreSeed.SampleRestaurant.Services
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
            if (!ExecutarValidacao(new MesaValidation(), mesa)) return false;

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
            await _fornecedorRepository.RemoverPorId(id);
            return true;
        }

        public async Task<Mesa> ObterPorId(Guid id)
        {
            return await _fornecedorRepository.ObterPorId(id);
        }

        public void Dispose()
        {
            _fornecedorRepository?.Dispose();
        }
    }
}
