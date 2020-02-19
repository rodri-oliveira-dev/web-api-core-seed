using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Restaurante.IO.Api.Extensions;
using Restaurante.IO.Api.Extensions.Authorization;
using Restaurante.IO.Api.ViewModels;
using Restaurante.IO.Business.Intefaces;
using Restaurante.IO.Business.Notificacoes;

namespace Restaurante.IO.Api.Controllers.V2.Controllers
{
    [ApiController]
    [Produces("application/json")]

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

        protected ActionResult CustomResponse(object result = null, ETipoAcao tipoAcao = ETipoAcao.Selecionar)
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
                        return NotFound(new CustomResult(false, "Objeto nâo foi encontrado"));

                    case ETipoAcao.ModeloInvalido:// HTTP Code 400
                        return BadRequest(new CustomResult(false, result));

                    default:
                        throw new ArgumentOutOfRangeException(nameof(tipoAcao), tipoAcao, null);
                }
            }

            return BadRequest(new CustomResult(false, _notificador.ObterNotificacoes().Select(n => n.Mensagem)));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificarErroModelInvalida(modelState);
            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(errorMsg);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }
    }
}
