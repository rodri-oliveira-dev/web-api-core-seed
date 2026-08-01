using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using WebApiCoreSeed.Api;
using WebApiCoreSeed.Api.Controllers.V1.Controllers;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using Xunit;

namespace WebApiCoreSeed.Tests.Arquitetura
{
    public class ModularHexagonalArchitectureTest
    {
        [Fact(DisplayName = "Modulo SampleRestaurant Core nao referencia API nem infraestrutura")]
        [Trait("Architecture", "ModularHexagonal")]
        public void SampleRestaurantCoreQuandoReferenciasAvaliadasNaoDeveDependerDeAdaptadores()
        {
            var forbiddenPrefixes = new[]
            {
                "WebApiCoreSeed.Api",
                "WebApiCoreSeed.SampleRestaurant.Infrastructure",
                "Microsoft.AspNetCore",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.Extensions.Logging",
                "StackExchange.Redis"
            };

            var references = typeof(Entity).Assembly.GetReferencedAssemblies()
                .Select(reference => reference.Name)
                .Where(name => forbiddenPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal)))
                .ToArray();

            Assert.Empty(references);
        }

        [Fact(DisplayName = "Infraestrutura SampleRestaurant referencia o nucleo do modulo")]
        [Trait("Architecture", "ModularHexagonal")]
        public void SampleRestaurantInfrastructureQuandoReferenciasAvaliadasDeveDependerDoCore()
        {
            var references = typeof(SampleRestaurantDbContext).Assembly.GetReferencedAssemblies()
                .Select(reference => reference.Name)
                .ToArray();

            Assert.Contains("WebApiCoreSeed.SampleRestaurant", references);
            Assert.DoesNotContain("WebApiCoreSeed.Api", references);
        }

        [Fact(DisplayName = "API referencia core e infraestrutura apenas para composicao")]
        [Trait("Architecture", "ModularHexagonal")]
        public void ApiQuandoReferenciasAvaliadasDeveComporCoreEInfraestrutura()
        {
            var references = typeof(Program).Assembly.GetReferencedAssemblies()
                .Select(reference => reference.Name)
                .ToArray();

            Assert.Contains("WebApiCoreSeed.SampleRestaurant", references);
            Assert.Contains("WebApiCoreSeed.SampleRestaurant.Infrastructure", references);
        }

        [Fact(DisplayName = "Controllers de dominio nao injetam repositorios")]
        [Trait("Architecture", "ModularHexagonal")]
        public void DomainControllersQuandoConstrutoresAvaliadosNaoDevemDependerDeRepositorios()
        {
            var controllerTypes = new[]
            {
                typeof(PratosController),
                typeof(MesasController)
            };

            var repositoryParameters = controllerTypes
                .SelectMany(type => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
                .SelectMany(constructor => constructor.GetParameters())
                .Where(parameter => parameter.ParameterType.Name.EndsWith("Repository", StringComparison.Ordinal)
                    || parameter.ParameterType.GetInterfaces().Any(@interface => @interface.Name.StartsWith("IRepository", StringComparison.Ordinal)))
                .Select(parameter => $"{parameter.Member.DeclaringType?.Name}.{parameter.Name}:{parameter.ParameterType.Name}")
                .ToArray();

            Assert.Empty(repositoryParameters);
        }

        [Fact(DisplayName = "Controllers de dominio dependem de portas de entrada da aplicacao")]
        [Trait("Architecture", "ModularHexagonal")]
        public void DomainControllersQuandoConstrutoresAvaliadosDevemDependerDeApplicationPorts()
        {
            var controllerTypes = new[]
            {
                typeof(PratosController),
                typeof(MesasController)
            };

            foreach (var controllerType in controllerTypes)
            {
                var hasApplicationPort = controllerType
                    .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .SelectMany(constructor => constructor.GetParameters())
                    .Any(parameter => string.Equals(
                        parameter.ParameterType.Namespace,
                        "WebApiCoreSeed.SampleRestaurant.Intefaces.Service",
                        StringComparison.Ordinal));

                Assert.True(hasApplicationPort, $"{controllerType.Name} deve depender de uma porta de entrada de Application.");
            }
        }

        [Fact(DisplayName = "Shared Kernel nao contem tipos especificos do dominio de exemplo")]
        [Trait("Architecture", "ModularHexagonal")]
        public void SharedKernelQuandoAvaliadoNaoDeveConterTiposDoDominioDeExemplo()
        {
            var exampleTerms = new[]
            {
                "Atendente",
                "Mesa",
                "Pedido",
                "Prato",
                "SampleRestaurant"
            };

            var sharedKernelTypes = typeof(Entity).Assembly.GetTypes()
                .Where(type => type.Namespace?.Contains("SharedKernel", StringComparison.Ordinal) == true)
                .Where(type => exampleTerms.Any(term => type.FullName?.Contains(term, StringComparison.Ordinal) == true))
                .Select(type => type.FullName)
                .ToArray();

            Assert.Empty(sharedKernelTypes);
        }
    }
}
