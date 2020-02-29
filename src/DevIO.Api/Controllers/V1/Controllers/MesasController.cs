using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurante.IO.Api.Extensions;
using Restaurante.IO.Api.Extensions.Clains;
using Restaurante.IO.Api.ViewModels;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Intefaces.Service;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Api.Controllers.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Mesas")]
    [ProducesResponseType(typeof(CustomResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(CustomResult), StatusCodes.Status403Forbidden)]
    public class MesasController : MainController
    {
        private readonly IMesaRepository _mesaRepository;
        private readonly IMesaService _mesaService;
        private readonly IMapper _mapper;

        public MesasController(INotificador notificador,
                                  IMesaRepository mesaRepository,
                                  IMesaService mesaService,
                                  IMapper mapper) : base(notificador)
        {
            _mesaRepository = mesaRepository;
            _mesaService = mesaService;
            _mapper = mapper;
        }

        /// <summary>
        /// Método responsavel pela obtenção da Mesa
        /// </summary>
        /// <param name="id">ID de identificação do objeto a ser pesquisado</param>
        /// <returns></returns>
        /// <response code="200">Retorna o objeto referente a ID informada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="404">Caso o objeto não seja encontrada pela ID retorna null</response>  
        [HttpGet("{id:guid}")]
        [ClaimsAuthorize("Mesas")]
        [ProducesResponseType(typeof(MesaViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MesaViewModel>> ObterPorId(Guid id)
        {
            var mesaViewModel = await ObterPrato(id);

            if (mesaViewModel == null) return CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);

            return mesaViewModel;
        }

        /// <summary>
        /// Cadastra o novo Mesa no sistema.
        /// </summary>
        /// <param name="mesaViewModel">Mesa a ser cadastrado</param>
        /// <returns></returns>
        /// <response code="201">Retorna o objeto referente a ID informada</response>
        /// <response code="400">Não foi possivel executar a ação solicitada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        [HttpPost]
        [ClaimsAuthorize("Mesas")]
        [ProducesResponseType(typeof(MesaViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MesaViewModel>> Adicionar(MesaViewModel mesaViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState, ETipoAcao.ModeloInvalido);

            await _mesaService.Adicionar(_mapper.Map<Mesa>(mesaViewModel));

            return CustomResponse(mesaViewModel, ETipoAcao.Adicionado);
        }

        /// <summary>
        /// Atualiza o Mesa no sistema.
        /// </summary>
        /// <param name="id">ID de identificação do Mesa a ser atualiado</param>
        /// <param name="mesaViewModel">Mesa a ser atualizado</param>
        /// <returns></returns>
        /// <response code="204">Objeto atualizado com sucesso</response>
        /// <response code="400">Não foi possivel executar a ação solicitada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="404">O objeto não foi encontrado.</response>
        [HttpPut("{id:guid}")]
        [ClaimsAuthorize("Mesas")]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Atualizar(Guid id, MesaViewModel mesaViewModel)
        {
            if (id != mesaViewModel.Id)
            {
                NotificarErro("Os ids informados não são iguais!");
                return CustomResponse(mesaViewModel, ETipoAcao.ModeloInvalido);
            }

            var mesaAtualizada = await ObterPrato(id);

            if (mesaAtualizada == null) return CustomResponse(ModelState, ETipoAcao.NaoEncontrado);

            mesaAtualizada.Numero = mesaViewModel.Numero;
            mesaAtualizada.Lugares = mesaViewModel.Lugares;
            mesaAtualizada.Ativo = mesaViewModel.Ativo;
            mesaAtualizada.Ativo = mesaViewModel.Ativo;
            mesaAtualizada.LocalizacaoMesa = mesaAtualizada.LocalizacaoMesa;

            await _mesaService.Atualizar(_mapper.Map<Mesa>(mesaAtualizada));

            return CustomResponse(mesaViewModel, ETipoAcao.Atualizado);
        }

        /// <summary>
        /// Exclui o Mesa do sistema.
        /// </summary>
        /// <param name="id">ID de identificação do Mesa a ser atualiado</param>
        /// <returns></returns>
        /// <response code="204">Objeto excluido com sucesso</response>
        /// <response code="400">Não foi possivel executar a ação solicitada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="404">O objeto não foi encontrado.</response>
        [HttpDelete("{id:guid}")]
        [ClaimsAuthorize("Mesas")]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MesaViewModel>> Excluir(Guid id)
        {
            var mesa = await ObterPrato(id);

            if (mesa == null) return CustomResponse(null, ETipoAcao.NaoEncontrado);

            await _mesaService.Remover(id);

            return CustomResponse(mesa, ETipoAcao.Excluido);
        }

        private async Task<MesaViewModel> ObterPrato(Guid id)
        {
            return _mapper.Map<MesaViewModel>(await _mesaRepository.ObterPorId(id));
        }
    }
}