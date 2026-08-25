using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using WebApiCoreSeed.Api.Controllers;
using WebApiCoreSeed.Api.Errors;
using WebApiCoreSeed.Api.Results;
using WebApiCoreSeed.Api.ViewModels;
using WebApiCoreSeed.SampleRestaurant.Interfaces;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Unitarios.Controllers;

public sealed class MainControllerBaseTests
{
    [Fact]
    public void CustomResponseQuandoOperacaoValidaDeveRetornarOk()
    {
        var controller = CreateController();

        var result = controller.Select("payload");

        var ok = Assert.IsType<OkObjectResult>(result);
        var custom = Assert.IsType<CustomResult>(ok.Value);
        Assert.True(custom.success);
        Assert.Equal("payload", custom.data);
    }

    [Fact]
    public void CustomResponseQuandoCriadoComMainViewModelDeveIncluirIdNaLocation()
    {
        var controller = CreateController();
        var viewModel = new TestViewModel { Id = Guid.NewGuid() };

        var result = controller.CreatedResponse(viewModel);

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Contains(viewModel.Id.ToString(), created.Location, StringComparison.Ordinal);
        Assert.IsType<CustomResult>(created.Value);
    }

    [Fact]
    public void CustomResponseQuandoCriadoSemMainViewModelDeveUsarPathDaRequest()
    {
        var controller = CreateController();

        var result = controller.CreatedResponse(new { Name = "payload" });

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal("https://api.example.local/api/v1/resources", created.Location);
        Assert.IsType<CustomResult>(created.Value);
    }

    [Theory]
    [InlineData("updated")]
    [InlineData("deleted")]
    public void CustomResponseQuandoNoContentDeveRetornarCustomNoContent(string action)
    {
        var controller = CreateController();

        var result = action == "updated"
            ? controller.UpdatedResponse()
            : controller.DeletedResponse();

        var noContent = Assert.IsType<CustomNoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContent.StatusCode);
        Assert.IsType<CustomResult>(noContent.Value);
    }

    [Fact]
    public void CustomResponseQuandoNaoEncontradoDeveRetornarProblemDetails404()
    {
        var controller = CreateController();

        var result = controller.NotFoundResponse();

        AssertProblem(result, StatusCodes.Status404NotFound, ApiProblemDetails.NotFoundType);
    }

    [Fact]
    public void CustomResponseQuandoModeloInvalidoDeveRetornarProblemDetails400()
    {
        var controller = CreateController();

        var result = controller.InvalidModelResponse();

        AssertProblem(result, StatusCodes.Status400BadRequest, ApiProblemDetails.ValidationType);
    }

    [Fact]
    public void CustomResponseQuandoModelStateInvalidoDeveRetornarValidationProblemDetails()
    {
        var controller = CreateController();
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "Required");

        var result = controller.FromModelState(modelState);

        var problemResult = AssertProblem(result, StatusCodes.Status400BadRequest, ApiProblemDetails.ValidationType);
        var problem = Assert.IsType<ValidationProblemDetails>(problemResult.Value);
        Assert.True(problem.Errors.ContainsKey("Name"));
    }

    [Fact]
    public void CustomResponseQuandoModelStateValidoENaoEncontradoDeveRetornar404()
    {
        var controller = CreateController();

        var result = controller.FromModelStateAsNotFound(new ModelStateDictionary());

        AssertProblem(result, StatusCodes.Status404NotFound, ApiProblemDetails.NotFoundType);
    }

    [Fact]
    public void CustomResponseQuandoExisteNotificacaoDeveRetornarDomainProblemDetails()
    {
        var controller = CreateController();
        controller.Notify("regra violada");

        var result = controller.Select();

        AssertProblem(result, StatusCodes.Status400BadRequest, ApiProblemDetails.DomainRuleType);
    }

    [Fact]
    public void ProblemDetailsQuandoNotificacaoIndicaConflitoDeveRetornar409()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/v1/resources";
        var notifications = new[] { new Notificacao("Já existe um objeto cadastrado.") };

        var problem = ApiProblemDetails.CreateFromNotifications(httpContext, notifications);

        Assert.Equal(StatusCodes.Status409Conflict, problem.Status);
        Assert.Equal(ApiProblemDetails.ConflictType, problem.Type);
        Assert.Equal("A operação conflita com um recurso existente.", problem.Detail);
    }

    [Fact]
    public void ProblemDetailsQuandoStatusConhecidoDeveRetornarTextoPortuguesCorreto()
    {
        Assert.Equal("Requisição inválida.", ApiProblemDetails.TitleForStatusCode(StatusCodes.Status400BadRequest));
        Assert.Equal("Autenticação necessária.", ApiProblemDetails.TitleForStatusCode(StatusCodes.Status401Unauthorized));
        Assert.Equal("Recurso não encontrado.", ApiProblemDetails.TitleForStatusCode(StatusCodes.Status404NotFound));
        Assert.Equal("Limite de requisições excedido.", ApiProblemDetails.TitleForStatusCode(StatusCodes.Status429TooManyRequests));
        Assert.Equal("Ocorreu um erro inesperado ao processar a requisição.", ApiProblemDetails.DetailForStatusCode(StatusCodes.Status500InternalServerError));
    }

    [Fact]
    public void NotificarErroModelInvalidaDeveUsarMensagemPadraoQuandoErroNaoPossuiTexto()
    {
        var controller = CreateController();
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", string.Empty);

        controller.NotifyInvalidModel(modelState);
        var result = controller.Select();

        var problemResult = AssertProblem(result, StatusCodes.Status400BadRequest, ApiProblemDetails.DomainRuleType);
        var problem = Assert.IsType<ProblemDetails>(problemResult.Value);
        var errors = Assert.IsType<Dictionary<string, string[]>>(problem.Extensions[ApiProblemDetails.ErrorsExtension]);
        Assert.Contains("Valor informado inválido.", errors["notifications"]);
    }

    [Fact]
    public void MainControllersVersionadosDevemDelegarParaBaseComMetadadosProprios()
    {
        var v1 = new V1MainControllerProbe();
        var v2 = new V2MainControllerProbe();

        Assert.IsAssignableFrom<MainControllerBase>(v1);
        Assert.IsAssignableFrom<MainControllerBase>(v2);
    }

    private static ProblemDetailsResult AssertProblem(ActionResult result, int statusCode, string type)
    {
        var problemResult = Assert.IsType<ProblemDetailsResult>(result);
        Assert.Equal(statusCode, problemResult.StatusCode);
        var problem = Assert.IsAssignableFrom<ProblemDetails>(problemResult.Value);
        Assert.Equal(type, problem.Type);

        return problemResult;
    }

    private static TestMainController CreateController()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("api.example.local");
        httpContext.Request.Path = "/api/v1/resources";

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ControllerActionDescriptor(),
            new ModelStateDictionary());

        return new TestMainController(new Notificador())
        {
            ControllerContext = new ControllerContext(actionContext),
            Url = new UrlHelper(actionContext)
        };
    }

    private sealed class TestViewModel : MainViewModel
    {
    }

    private sealed class TestMainController : MainControllerBase
    {
        public TestMainController(INotificador notificador) : base(notificador)
        {
        }

        public ActionResult Select(object? result = null) => CustomResponse(result);

        public ActionResult CreatedResponse(object? result) => CustomResponse(result, ETipoAcao.Adicionado);

        public ActionResult UpdatedResponse() => CustomResponse(tipoAcao: ETipoAcao.Atualizado);

        public ActionResult DeletedResponse() => CustomResponse(tipoAcao: ETipoAcao.Excluido);

        public ActionResult NotFoundResponse() => CustomResponse(tipoAcao: ETipoAcao.NaoEncontrado);

        public ActionResult InvalidModelResponse() => CustomResponse(tipoAcao: ETipoAcao.ModeloInvalido);

        public ActionResult FromModelState(ModelStateDictionary modelState) => CustomResponse(modelState);

        public ActionResult FromModelStateAsNotFound(ModelStateDictionary modelState) => CustomResponse(modelState, ETipoAcao.NaoEncontrado);

        public void Notify(string message) => NotificarErro(message);

        public void NotifyInvalidModel(ModelStateDictionary modelState) => NotificarErroModelInvalida(modelState);
    }

    private sealed class V1MainControllerProbe : WebApiCoreSeed.Api.Controllers.V1.Controllers.MainController
    {
        public V1MainControllerProbe() : base(new Notificador())
        {
        }
    }

    private sealed class V2MainControllerProbe : WebApiCoreSeed.Api.Controllers.V2.Controllers.MainController
    {
        public V2MainControllerProbe() : base(new Notificador())
        {
        }
    }
}
