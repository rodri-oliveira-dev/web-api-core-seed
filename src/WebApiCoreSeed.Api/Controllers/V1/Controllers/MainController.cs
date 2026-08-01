using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApiCoreSeed.Api.Errors;
using WebApiCoreSeed.Api.Results;
using WebApiCoreSeed.Api.ViewModels;
using WebApiCoreSeed.SampleRestaurant.Intefaces;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;

namespace WebApiCoreSeed.Api.Controllers.V1.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador _notificador;

        protected enum ETipoAcao
        {
            /// <summary>
            /// HTTP Code 200
            /// </summary>
            Selecionar = 0,
            /// <summary>
            /// HTTP Code 201
            /// </summary>
            Adicionado = 1,
            /// <summary>
            /// HTTP Code 204
            /// </summary>
            Atualizado = 2,
            /// <summary>
            /// HTTP Code 204
            /// </summary>
            Excluido = 3,
            /// <summary>
            /// HTTP Code 404
            /// </summary>
            NaoEncontrado = 4,
            /// <summary>
            /// HTTP Code 400
            /// </summary>
            ModeloInvalido = 5
        }

        protected MainController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object? result = null, ETipoAcao tipoAcao = ETipoAcao.Selecionar)
        {
            if (OperacaoValida())
            {
                switch (tipoAcao)
                {
                    case ETipoAcao.Selecionar: // HTTP Code 200
                        return Ok(new CustomResult(true, result));

                    case ETipoAcao.Adicionado:// HTTP Code 201
                        if (result is MainViewModel mainView)
                        {
                            return Created(new Uri($"{Url.ActionContext.HttpContext.Request.Scheme}://{Url.ActionContext.HttpContext.Request.Host}{Url.ActionContext.HttpContext.Request.Path}/{mainView.Id}"), new CustomResult(true, result));
                        }

                        return Created(new Uri($"{Url.ActionContext.HttpContext.Request.Scheme}://{Url.ActionContext.HttpContext.Request.Host}{Url.ActionContext.HttpContext.Request.Path}"), new CustomResult(true, result));

                    case ETipoAcao.Atualizado:// HTTP Code 204
                        return new CustomNoContentResult(new CustomResult(true, result));

                    case ETipoAcao.Excluido:// HTTP Code 204
                        return new CustomNoContentResult(new CustomResult(true, "Objeto excluido com sucesso"));

                    case ETipoAcao.NaoEncontrado:// HTTP Code 404
                        return ApiProblemDetails.ToObjectResult(ApiProblemDetails.Create(
                            HttpContext,
                            StatusCodes.Status404NotFound,
                            ApiProblemDetails.NotFoundType,
                            "Recurso nao encontrado.",
                            "Objeto nao foi encontrado."));

                    case ETipoAcao.ModeloInvalido:// HTTP Code 400
                        return ApiProblemDetails.ToObjectResult(ApiProblemDetails.Create(
                            HttpContext,
                            StatusCodes.Status400BadRequest,
                            ApiProblemDetails.ValidationType,
                            "Requisicao invalida.",
                            "A requisicao possui dados invalidos."));

                    default:
                        throw new ArgumentOutOfRangeException(nameof(tipoAcao), tipoAcao, string.Empty);
                }
            }

            return ApiProblemDetails.ToObjectResult(ApiProblemDetails.CreateFromNotifications(HttpContext, _notificador.ObterNotificacoes()));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            return modelState.IsValid
                ? CustomResponse()
                : ApiProblemDetails.ToObjectResult(ApiProblemDetails.CreateValidation(HttpContext, modelState));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState, ETipoAcao tipoAcao)
        {
            if (tipoAcao == ETipoAcao.NaoEncontrado)
            {
                return CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);
            }

            return CustomResponse(modelState);
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMsg = string.IsNullOrWhiteSpace(erro.ErrorMessage) ? "Valor informado invalido." : erro.ErrorMessage;
                NotificarErro(errorMsg);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }
    }
}
