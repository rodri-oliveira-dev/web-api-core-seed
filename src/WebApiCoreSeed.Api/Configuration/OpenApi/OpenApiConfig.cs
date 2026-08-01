using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using WebApiCoreSeed.Api.Errors;
using Scalar.AspNetCore;

namespace WebApiCoreSeed.Api.Configuration.OpenApi
{
    public static class OpenApiConfig
    {
        private const string BearerScheme = "Bearer";

        public static VersionedOpenApiOptions ConfigureSeedOpenApi(this VersionedOpenApiOptions options)
        {
            options.Document.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
            options.Document.AddDocumentTransformer(ConfigureDocument);
            options.Document.AddOperationTransformer(ConfigureOperation);

            return options;
        }

        public static WebApplication UseOpenApiConfig(this WebApplication app)
        {
            app.MapOpenApi().WithDocumentPerVersion();
            app.MapScalarApiReference(options =>
            {
                var descriptions = app.DescribeApiVersions();

                for (var i = 0; i < descriptions.Count; i++)
                {
                    var description = descriptions[i];
                    options.AddDocument(description.GroupName, description.GroupName.ToUpperInvariant(), isDefault: i == descriptions.Count - 1);
                }

                options.Title = "Sample Restaurant API";
                options.PersistentAuthentication = true;
            });

            return app;
        }

        private static Task ConfigureDocument(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var description = context.DocumentName;

            document.Info.Title = "Sample Restaurant API";
            document.Info.Version = description;
            document.Info.Description = "Esta API demonstra endpoints de restaurante do sample.";

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[BearerScheme] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Informe somente o token JWT. A UI envia o prefixo Bearer no header Authorization."
            };

            return Task.CompletedTask;
        }

        private static Task ConfigureOperation(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            operation.Responses ??= new OpenApiResponses();
            operation.Responses.TryAdd(StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture), CreateProblemDetailsResponse("Requisicao invalida."));
            operation.Responses.TryAdd(StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture), CreateProblemDetailsResponse("Limite de requisicoes excedido."));

            if (RequiresAuthorization(context))
            {
                if (context.Document is not null)
                {
                    operation.Security ??= new List<OpenApiSecurityRequirement>();
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(BearerScheme, context.Document)] = new List<string>()
                    });
                }

                operation.Responses.TryAdd(StatusCodes.Status401Unauthorized.ToString(CultureInfo.InvariantCulture), CreateProblemDetailsResponse("Autenticacao necessaria."));
                operation.Responses.TryAdd(StatusCodes.Status403Forbidden.ToString(CultureInfo.InvariantCulture), CreateProblemDetailsResponse("Acesso negado."));
            }

            foreach (var statusCode in operation.Responses.Keys.ToList())
            {
                if (statusCode is "400" or "401" or "403" or "404" or "429" or "500")
                {
                    var response = operation.Responses[statusCode];
                    operation.Responses[statusCode] = new OpenApiResponse
                    {
                        Description = response.Description,
                        Content = CreateProblemDetailsContent(context.Document)
                    };
                }
            }

            return Task.CompletedTask;
        }

        private static bool RequiresAuthorization(OpenApiOperationTransformerContext context)
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            return metadata.OfType<IAuthorizeData>().Any() && !metadata.OfType<IAllowAnonymous>().Any();
        }

        private static OpenApiResponse CreateProblemDetailsResponse(string description)
        {
            return new OpenApiResponse
            {
                Description = description,
                Content = CreateProblemDetailsContent(null)
            };
        }

        private static Dictionary<string, OpenApiMediaType> CreateProblemDetailsContent(OpenApiDocument? document)
        {
            return new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = document == null ? null : new OpenApiSchemaReference("ProblemDetails", document)
                }
            };
        }
    }
}
