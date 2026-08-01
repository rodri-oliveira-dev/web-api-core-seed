using System;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApiCoreSeed.Api.Attributes;
using WebApiCoreSeed.Api.Configuration;
using WebApiCoreSeed.Api.Extensions;
using WebApiCoreSeed.Api.Results;
using WebApiCoreSeed.Api.ViewModels;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.Api.Controllers.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Mesas")]
    [ProducesResponseType(typeof(CustomResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(CustomResult), StatusCodes.Status403Forbidden)]
    [EnableRateLimiting(NativeRateLimitPolicies.Authenticated)]
    public class MesasController : MainController
    {
        private readonly IMesaService _mesaService;
        private readonly IMapper _mapper;

        public MesasController(INotificador notificador,
                                  IMesaService mesaService,
                                  IMapper mapper) : base(notificador)
        {
            _mesaService = mesaService;
            _mapper = mapper;
        }

        /// <summary>
        /// Método responsavel pela obtenção da Mesa
        /// </summary>
        /// <param name="id">ID de identificação do objeto a ser pesquisado</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
        /// <returns></returns>
        /// <response code="200">Retorna o objeto referente a ID informada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="404">Caso o objeto não seja encontrada pela ID retorna null</response>
        [HttpGet("{id:guid}")]
        [ClaimsAuthorize("Mesas")]
        [ProducesResponseType(typeof(MesaViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MesaViewModel>> ObterPorId(Guid id, CancellationToken cancellationToken)
        {
            var mesaViewModel = await ObterMesa(id, cancellationToken);

            if (mesaViewModel == null) return CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);

            return mesaViewModel;
        }

        /// <summary>
        /// Cadastra o novo Mesa no sistema.
        /// </summary>
        /// <param name="mesaViewModel">Mesa a ser cadastrado</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
        /// <returns></returns>
        /// <response code="201">Retorna o objeto referente a ID informada</response>
        /// <response code="400">Não foi possivel executar a ação solicitada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        [HttpPost]
        [ClaimsAuthorize("Mesas")]
        [ProducesResponseType(typeof(MesaViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MesaViewModel>> Adicionar(MesaViewModel mesaViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState, ETipoAcao.ModeloInvalido);

            await _mesaService.Adicionar(_mapper.Map<Mesa>(mesaViewModel), cancellationToken);

            return CustomResponse(mesaViewModel, ETipoAcao.Adicionado);
        }

        /// <summary>
        /// Atualiza o Mesa no sistema.
        /// </summary>
        /// <param name="id">ID de identificação do Mesa a ser atualiado</param>
        /// <param name="mesaViewModel">Mesa a ser atualizado</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
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
        public async Task<IActionResult> Atualizar(Guid id, MesaViewModel mesaViewModel, CancellationToken cancellationToken)
        {
            if (id != mesaViewModel.Id)
            {
                NotificarErro("Os ids informados não são iguais!");
                return CustomResponse(mesaViewModel, ETipoAcao.ModeloInvalido);
            }

            var mesaAtualizada = await ObterMesa(id, cancellationToken);

            if (mesaAtualizada == null) return CustomResponse(ModelState, ETipoAcao.NaoEncontrado);

            mesaAtualizada.Numero = mesaViewModel.Numero;
            mesaAtualizada.Lugares = mesaViewModel.Lugares;
            mesaAtualizada.Ativo = mesaViewModel.Ativo;
            mesaAtualizada.Ativo = mesaViewModel.Ativo;
            mesaAtualizada.LocalizacaoMesa = mesaAtualizada.LocalizacaoMesa;

            await _mesaService.Atualizar(_mapper.Map<Mesa>(mesaAtualizada), cancellationToken);

            return CustomResponse(mesaViewModel, ETipoAcao.Atualizado);
        }

        /// <summary>
        /// Exclui o Mesa do sistema.
        /// </summary>
        /// <param name="id">ID de identificação do Mesa a ser atualiado</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
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
        public async Task<ActionResult<MesaViewModel>> Excluir(Guid id, CancellationToken cancellationToken)
        {
            var mesa = await ObterMesa(id, cancellationToken);

            if (mesa == null) return CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);

            await _mesaService.Remover(id, cancellationToken);

            return CustomResponse(mesa, ETipoAcao.Excluido);
        }

        private async Task<MesaViewModel?> ObterMesa(Guid id, CancellationToken cancellationToken)
        {
            return _mapper.Map<MesaViewModel>(await _mesaService.ObterPorId(id, cancellationToken));
        }
    }
}
