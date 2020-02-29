using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Restaurante.IO.Api.Configuration.Cache;
using Restaurante.IO.Api.Extensions;
using Restaurante.IO.Api.Extensions.Clains;
using Restaurante.IO.Api.ViewModels;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Intefaces.Service;
using Restaurante.IO.Business.Interfaces.Pagination;
using Restaurante.IO.Business.Interfaces.Repository;
using Restaurante.IO.Business.Models;

namespace Restaurante.IO.Api.Controllers.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Pratos")]
    [ProducesResponseType(typeof(CustomResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(CustomResult), StatusCodes.Status403Forbidden)]
    public class PratosController : MainController
    {
        private readonly IPratoRepository _pratoRepository;
        private readonly IPratoService _pratoService;
        private readonly IMapper _mapper;
        private readonly ILogger<PratosController> _logger;
        private readonly IUser _user;

        public PratosController(INotificador notificador,
                                  IPratoRepository pratoRepository,
                                  IPratoService pratoService,
                                  IMapper mapper,
                                  ILogger<PratosController> logger,
                                  IUser user) : base(notificador)
        {
            _pratoRepository = pratoRepository;
            _pratoService = pratoService;
            _mapper = mapper;
            _logger = logger;
            _user = user;
        }

        /// <summary>
        /// Método responsavel pela obtenção de lista de Pratos
        /// </summary>
        /// <param name="paginationParameter">Parametros de paginação da lista</param>
        /// <returns></returns>
        /// <response code="200">Retorna o objeto referente a ID informada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="404">Caso a lista de objeto não seja encontrada</response>
        /// <response code="429">Excedeu a cota de requisições</response> 
        [AllowAnonymous] 
        [HttpGet]
        //[ClaimsAuthorize("Pratos")]
        [ProducesResponseType(typeof(PaginationResult<PratoViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status404NotFound)]
        [Cached(20)]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
        public async Task<ActionResult<PaginationResult<PratoViewModel>>> ObterLista([FromQuery] PaginationParameter paginationParameter)
        {
            var pratosViewModel = await ObterPratos(paginationParameter);

            if (pratosViewModel == null) return CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);
            if (_user.IsAuthenticated())
            {
                _logger.LogInformation($"{_user.GetUserId()} chamou o metodo");
            }
            _logger.LogInformation("usuario anonimo chamou o metodo");

            return pratosViewModel;
        }

        /// <summary>
        /// Método responsavel pela obtenção do Prato
        /// </summary>
        /// <param name="id">ID de identificação do objeto a ser pesquisado</param>
        /// <returns></returns>
        /// <response code="200">Retorna o objeto referente a ID informada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="404">Caso o objeto não seja encontrada pela ID retorna null</response>
        /// <response code="429">Excedeu a cota de requisições</response> 
        [HttpGet("{id:guid}")]
        [ClaimsAuthorize("Pratos")]
        [ProducesResponseType(typeof(PratoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PratoViewModel>> ObterPorId(Guid id)
        {
            var pratoViewModel = await ObterPrato(id);

            if (pratoViewModel == null) return CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);

            return pratoViewModel;
        }

        /// <summary>
        /// Cadastra o novo prato no sistema.
        /// </summary>
        /// <param name="pratoViewModel">Prato a ser cadastrado</param>
        /// <returns></returns>
        /// <response code="201">Retorna o objeto referente a ID informada</response>
        /// <response code="400">Não foi possivel executar a ação solicitada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="429">Excedeu a cota de requisições</response> 
        [HttpPost]
        [ClaimsAuthorize("Pratos")]
        [ProducesResponseType(typeof(PratoViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PratoViewModel>> Adicionar(PratoViewModel pratoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState, ETipoAcao.ModeloInvalido);

            var imagemNome = Guid.NewGuid() + "_" + pratoViewModel.Foto;
            if (!UploadArquivo(pratoViewModel.FotoUpload, imagemNome))
            {
                return CustomResponse(pratoViewModel, ETipoAcao.ModeloInvalido);
            }

            pratoViewModel.Foto = imagemNome;
            await _pratoService.Adicionar(_mapper.Map<Prato>(pratoViewModel));

            return CustomResponse(pratoViewModel, ETipoAcao.Adicionado);
        }

        /// <summary>
        /// Atualiza o prato no sistema.
        /// </summary>
        /// <param name="id">ID de identificação do prato a ser atualiado</param>
        /// <param name="pratoViewModel">Prato a ser atualizado</param>
        /// <returns></returns>
        /// <response code="204">Objeto atualizado com sucesso</response>
        /// <response code="400">Não foi possivel executar a ação solicitada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="404">O objeto não foi encontrado.</response>
        /// <response code="429">Excedeu a cota de requisições</response> 
        [HttpPut("{id:guid}")]
        [ClaimsAuthorize("Pratos")]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Atualizar(Guid id, PratoViewModel pratoViewModel)
        {
            if (id != pratoViewModel.Id)
            {
                NotificarErro("Os ids informados não são iguais!");
                return CustomResponse(pratoViewModel, ETipoAcao.ModeloInvalido);
            }

            var pratoAtualizacao = await ObterPrato(id);

            if (pratoAtualizacao == null) return CustomResponse(ModelState, ETipoAcao.NaoEncontrado);

            pratoViewModel.Foto = pratoAtualizacao.Foto;
            if (!ModelState.IsValid) return CustomResponse(ModelState, ETipoAcao.ModeloInvalido);

            if (pratoViewModel.FotoUpload != null)
            {
                var imagemNome = Guid.NewGuid() + "_" + pratoViewModel.Foto;
                if (!UploadArquivo(pratoViewModel.FotoUpload, imagemNome))
                {
                    return CustomResponse(ModelState, ETipoAcao.ModeloInvalido);
                }
                pratoAtualizacao.Foto = imagemNome;
            }

            pratoAtualizacao.Titulo = pratoViewModel.Titulo;
            pratoAtualizacao.Descricao = pratoViewModel.Descricao;
            pratoAtualizacao.Preco = pratoViewModel.Preco;
            pratoAtualizacao.Ativo = pratoViewModel.Ativo;
            pratoAtualizacao.TipoPrato = pratoAtualizacao.TipoPrato;

            await _pratoService.Atualizar(_mapper.Map<Prato>(pratoAtualizacao));

            return CustomResponse(pratoViewModel, ETipoAcao.Atualizado);
        }

        /// <summary>
        /// Exclui o prato do sistema.
        /// </summary>
        /// <param name="id">ID de identificação do prato a ser atualiado</param>
        /// <returns></returns>
        /// <response code="204">Objeto excluido com sucesso</response>
        /// <response code="400">Não foi possivel executar a ação solicitada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="404">O objeto não foi encontrado.</response>
        /// <response code="429">Excedeu a cota de requisições</response> 
        [HttpDelete("{id:guid}")]
        [ClaimsAuthorize("Pratos")]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PratoViewModel>> Excluir(Guid id)
        {
            var prato = await ObterPrato(id);

            if (prato == null) return CustomResponse(null, ETipoAcao.NaoEncontrado);

            await _pratoService.Remover(id);

            return CustomResponse(prato, ETipoAcao.Excluido);
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este Prato!");
                return false;
            }

            if (!arquivo.IsBase64String())
            {
                NotificarErro("Forneça uma imagem no formato Base64 para este Prato!");
                return false;
            }

            var imageDataByteArray = Convert.FromBase64String(arquivo);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

            return true;
        }

        private async Task<PratoViewModel> ObterPrato(Guid id)
        {
            return _mapper.Map<PratoViewModel>(await _pratoRepository.ObterPorId(id));
        }

        private async Task<PaginationResult<PratoViewModel>> ObterPratos(PaginationParameter paginationParameter)
        {
            var pratos = _mapper.Map<List<PratoViewModel>>(await _pratoRepository.Paginacao(paginationParameter));
            var totalItens = await _pratoRepository.TotalRegistros();
            var totalPaginas = totalItens / paginationParameter.PageSize;

            return new PaginationResult<PratoViewModel>
            {
                PageNumber = paginationParameter.PageNumber,
                TotalItens = totalItens,
                TotalPages = totalItens % paginationParameter.PageSize > 0 ? totalPaginas + 1 : totalPaginas,
                Data = pratos
            };
        }
    }
}