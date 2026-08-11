using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using WebApiCoreSeed.Api.Attributes;
using WebApiCoreSeed.Api.Configuration;
using WebApiCoreSeed.Api.Extensions;
using WebApiCoreSeed.Api.Extensions.Clains;
using WebApiCoreSeed.Api.ViewModels;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Intefaces.Service;
using WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination;
using WebApiCoreSeed.SampleRestaurant.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebApiCoreSeed.Api.Results;

namespace WebApiCoreSeed.Api.Controllers.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Pratos")]
    [ProducesResponseType(typeof(CustomResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(CustomResult), StatusCodes.Status403Forbidden)]
    [EnableRateLimiting(NativeRateLimitPolicies.Authenticated)]
    public class PratosController : MainController
    {
        private readonly IPratoService _pratoService;
        private readonly IMapper _mapper;
        private readonly ILogger<PratosController> _logger;
        private readonly IUser _user;
        private static readonly Action<ILogger, Guid, Exception?> LogAuthenticatedUserCalledMethod =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(1000, nameof(LogAuthenticatedUserCalledMethod)),
                "Usuario autenticado {UserId} chamou o metodo");

        private static readonly Action<ILogger, Exception?> LogAnonymousUserCalledMethod =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(1001, nameof(LogAnonymousUserCalledMethod)),
                "Usuario anonimo chamou o metodo");

        public PratosController(INotificador notificador,
                                  IPratoService pratoService,
                                  IMapper mapper,
                                  ILogger<PratosController> logger,
                                  IUser user) : base(notificador)
        {
            _pratoService = pratoService;
            _mapper = mapper;
            _logger = logger;
            _user = user;
        }

        /// <summary>
        /// Método responsavel pela obtenção de lista de Pratos
        /// </summary>
        /// <param name="paginationParameter">Parametros de paginação da lista</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
        /// <returns></returns>
        /// <response code="200">Retorna o objeto referente a ID informada</response>
        /// <response code="401">A chamada precisa ser efetuada por um usuario autenticado.</response>
        /// <response code="403">O usuário esta autenticado, mas o não possui permissão para executar essa ação.</response>
        /// <response code="404">Caso a lista de objeto não seja encontrada</response>
        /// <response code="429">Excedeu a cota de requisições</response>
        [AllowAnonymous]
        [EnableRateLimiting(NativeRateLimitPolicies.Public)]
        [HttpGet]
        //[ClaimsAuthorize("Pratos")]
        [ProducesResponseType(typeof(PaginationResult<PratoViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomResult), StatusCodes.Status404NotFound)]
        [Cached(20)]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
        public async Task<ActionResult<PaginationResult<PratoViewModel>>> ObterLista(
            [FromQuery] PaginationParameter paginationParameter,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var pratosViewModel = await ObterPratos(paginationParameter, cancellationToken);

            if (pratosViewModel == null) return CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);
            if (_user.IsAuthenticated())
            {
                LogAuthenticatedUserCalledMethod(_logger, _user.GetUserId(), null);
            }
            LogAnonymousUserCalledMethod(_logger, null);

            return pratosViewModel;
        }

        /// <summary>
        /// Método responsavel pela obtenção do Prato
        /// </summary>
        /// <param name="id">ID de identificação do objeto a ser pesquisado</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
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
        public async Task<ActionResult<PratoViewModel>> ObterPorId(Guid id, CancellationToken cancellationToken)
        {
            var pratoViewModel = await ObterPrato(id, cancellationToken);

            if (pratoViewModel == null) return CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);

            return pratoViewModel;
        }

        /// <summary>
        /// Cadastra o novo prato no sistema.
        /// </summary>
        /// <param name="pratoViewModel">Prato a ser cadastrado</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
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
        public async Task<ActionResult<PratoViewModel>> Adicionar(PratoRequestViewModel pratoViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState, ETipoAcao.ModeloInvalido);

            if (!UploadArquivo(pratoViewModel.FotoUpload, pratoViewModel.Foto, out var imagemNome))
            {
                return CustomResponse(pratoViewModel, ETipoAcao.ModeloInvalido);
            }

            var response = _mapper.Map<PratoViewModel>(pratoViewModel);
            response.Foto = imagemNome;
            await _pratoService.Adicionar(_mapper.Map<Prato>(response), cancellationToken);

            return CustomResponse(response, ETipoAcao.Adicionado);
        }

        /// <summary>
        /// Atualiza o prato no sistema.
        /// </summary>
        /// <param name="id">ID de identificação do prato a ser atualiado</param>
        /// <param name="pratoViewModel">Prato a ser atualizado</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
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
        public async Task<IActionResult> Atualizar(Guid id, PratoRequestViewModel pratoViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState, ETipoAcao.ModeloInvalido);

            if (id != pratoViewModel.Id)
            {
                NotificarErro("Os ids informados não são iguais!");
                return CustomResponse(pratoViewModel, ETipoAcao.ModeloInvalido);
            }

            var pratoAtualizacao = await ObterPrato(id, cancellationToken);

            if (pratoAtualizacao == null) return CustomResponse(ModelState, ETipoAcao.NaoEncontrado);

            if (pratoViewModel.FotoUpload != null)
            {
                if (!UploadArquivo(pratoViewModel.FotoUpload, pratoAtualizacao.Foto, out var imagemNome))
                {
                    return CustomResponse(ModelState, ETipoAcao.ModeloInvalido);
                }
                pratoAtualizacao.Foto = imagemNome;
            }

            pratoAtualizacao.Titulo = pratoViewModel.Titulo;
            pratoAtualizacao.Descricao = pratoViewModel.Descricao;
            pratoAtualizacao.Preco = pratoViewModel.Preco;
            pratoAtualizacao.Ativo = pratoViewModel.Ativo.GetValueOrDefault();
            pratoAtualizacao.TipoPrato = pratoAtualizacao.TipoPrato;

            await _pratoService.Atualizar(_mapper.Map<Prato>(pratoAtualizacao), cancellationToken);

            return CustomResponse(pratoViewModel, ETipoAcao.Atualizado);
        }

        /// <summary>
        /// Exclui o prato do sistema.
        /// </summary>
        /// <param name="id">ID de identificação do prato a ser atualiado</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
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
        public async Task<ActionResult<PratoViewModel>> Excluir(Guid id, CancellationToken cancellationToken)
        {
            var prato = await ObterPrato(id, cancellationToken);

            if (prato == null) return CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);

            await _pratoService.Remover(id, cancellationToken);

            return CustomResponse(prato, ETipoAcao.Excluido);
        }

        private bool UploadArquivo(string? arquivo, string? nomeOriginal, out string imgNome)
        {
            imgNome = string.Empty;

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
            imgNome = GerarNomeImagem(nomeOriginal);

            var diretorioUpload = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets"));
            Directory.CreateDirectory(diretorioUpload);

            var filePath = Path.GetFullPath(Path.Combine(diretorioUpload, imgNome));
            if (!EstaDentroDoDiretorio(filePath, diretorioUpload))
            {
                NotificarErro("Nome de arquivo invalido!");
                return false;
            }

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

            return true;
        }

        private static string GerarNomeImagem(string? nomeOriginal)
        {
            return $"{Guid.NewGuid():N}{ObterExtensaoPermitida(nomeOriginal)}";
        }

        private static string ObterExtensaoPermitida(string? nomeOriginal)
        {
            var extensao = Path.GetExtension(Path.GetFileName(nomeOriginal));

            if (string.Equals(extensao, ".gif", StringComparison.OrdinalIgnoreCase)) return ".gif";
            if (string.Equals(extensao, ".jpeg", StringComparison.OrdinalIgnoreCase)) return ".jpeg";
            if (string.Equals(extensao, ".jpg", StringComparison.OrdinalIgnoreCase)) return ".jpg";
            if (string.Equals(extensao, ".png", StringComparison.OrdinalIgnoreCase)) return ".png";
            if (string.Equals(extensao, ".webp", StringComparison.OrdinalIgnoreCase)) return ".webp";

            return string.Empty;
        }

        private static bool EstaDentroDoDiretorio(string filePath, string diretorio)
        {
            var diretorioComSeparador = diretorio.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? diretorio
                : diretorio + Path.DirectorySeparatorChar;

            return filePath.StartsWith(diretorioComSeparador, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<PratoViewModel?> ObterPrato(Guid id, CancellationToken cancellationToken)
        {
            return _mapper.Map<PratoViewModel>(await _pratoService.ObterPorId(id, cancellationToken));
        }

        private async Task<PaginationResult<PratoViewModel>> ObterPratos(PaginationParameter paginationParameter, CancellationToken cancellationToken)
        {
            var pratos = _mapper.Map<List<PratoViewModel>>(await _pratoService.Paginacao(paginationParameter, cancellationToken));
            var totalItens = await _pratoService.TotalRegistros(cancellationToken);
            var totalPaginas = (int)Math.Ceiling(totalItens / (double)paginationParameter.PageSize);

            return new PaginationResult<PratoViewModel>
            {
                Items = pratos,
                Page = paginationParameter.PageNumber,
                PageSize = paginationParameter.PageSize,
                TotalItems = totalItens,
                TotalPages = totalPaginas,
                HasNextPage = paginationParameter.PageNumber < totalPaginas,
                HasPreviousPage = totalItens > 0 && paginationParameter.PageNumber > 1
            };
        }
    }
}
