using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Persistence;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class MesaService : BaseService, IMesaService
    {
        private readonly IMesaRepository _fornecedorRepository;
        private readonly ISampleRestaurantUnitOfWork _unitOfWork;

        public MesaService(IMesaRepository fornecedorRepository,
                                 ISampleRestaurantUnitOfWork unitOfWork,
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Adicionar(Mesa mesa)
        {
            if (!ExecutarValidacao(new MesaValidation(), mesa)) return false;

            await _fornecedorRepository.Adicionar(mesa);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Atualizar(Mesa mesa)
        {
            if (!ExecutarValidacao(new MesaValidation(), mesa)) return false;

            await _fornecedorRepository.Atualizar(mesa);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> Remover(Guid id)
        {
            await _fornecedorRepository.RemoverPorId(id);
            await _unitOfWork.CommitAsync();
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
