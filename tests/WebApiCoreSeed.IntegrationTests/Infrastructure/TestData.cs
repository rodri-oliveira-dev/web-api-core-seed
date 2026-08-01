using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.IntegrationTests.Infrastructure;

public static class TestData
{
    public static Prato CreatePrato(string? titulo = null)
    {
        return new Prato
        {
            Titulo = titulo ?? $"Prato {Guid.NewGuid():N}",
            Descricao = "Prato criado pela suite de integracao.",
            Foto = "prato-integracao.png",
            Preco = 42.5,
            Ativo = true,
            TipoPrato = ETipoPrato.Comida
        };
    }

    public static Mesa CreateMesa(string? numero = null)
    {
        return new Mesa
        {
            Numero = numero ?? $"M-{Guid.NewGuid():N}"[..12],
            Lugares = 4,
            Ativo = true,
            LocalizacaoMesa = ELocalizacaoMesa.Interna
        };
    }

    public static Atendente CreateAtendente(string? nome = null)
    {
        return new Atendente
        {
            Nome = nome ?? $"Atendente {Guid.NewGuid():N}"[..22],
            TipoAtendente = ETipoAtendente.Garcom
        };
    }
}
