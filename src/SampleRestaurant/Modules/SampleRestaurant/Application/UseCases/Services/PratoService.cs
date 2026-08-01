using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Validations;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;

namespace WebApiCoreSeed.SampleRestaurant.Services
{
    public class PratoService : BaseService, IPratoService
    {
        private readonly IPratoRepository _pratoRepository;
        private readonly INotificador _notificador;

        public PratoService(IPratoRepository pratoRepository,
                                 INotificador notificador) : base(notificador)
        {
            _pratoRepository = pratoRepository;
            _notificador = notificador;
        }

        public async Task<bool> Adicionar(Prato prato)
        {
            if (!ExecutarValidacao(new PratoValidation(), prato)) return false;

            if (_pratoRepository.ObterPorId(prato.Id).Result != null)
            {
                _notificador.Handle(new Notificacao($"Já existe um objeto cadastrado com a ID {prato.Id}."));
                return false;
            }

            await _pratoRepository.Adicionar(prato);
            return true;
        }

        public async Task<bool> Atualizar(Prato prato)
        {
            if (!ExecutarValidacao(new PratoValidation(), prato)) return false;

            await _pratoRepository.Atualizar(prato);
            return true;
        }

        public async Task<bool> Remover(Guid id)
        {
            var excluido = await _pratoRepository.Remover(id);
            return true;
        }

        public async Task<Prato> ObterPorId(Guid id)
        {
            return await _pratoRepository.ObterPorId(id);
        }

        public async Task<IEnumerable<Prato>> Paginacao(PaginationParameter paginationParameter)
        {
            return await _pratoRepository.Paginacao(paginationParameter);
        }

        public async Task<int> TotalRegistros()
        {
            return await _pratoRepository.TotalRegistros();
        }

        public void Dispose()
        {
            _pratoRepository?.Dispose();
        }
    }
}
