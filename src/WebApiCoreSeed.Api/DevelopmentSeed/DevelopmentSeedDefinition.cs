using System;
using System.Collections.Generic;
using System.Security.Claims;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.Api.DevelopmentSeed
{
    public static class DevelopmentSeedDefinition
    {
        public const string UserId = "11111111-1111-1111-1111-111111111111";

        public static IReadOnlyList<Claim> UserClaims { get; } =
        [
            new("Mesas", "ObterPorId"),
            new("Mesas", "Adicionar"),
            new("Pratos", "ObterPorId"),
            new("Pratos", "Adicionar")
        ];

        public static IReadOnlyList<DevelopmentSeedPrato> Pratos { get; } =
        [
            new(
                Guid.Parse("21000000-0000-0000-0000-000000000001"),
                "Arroz de costela",
                "Prato principal de desenvolvimento para validar listagem, detalhes e escrita autenticada.",
                "seed-arroz-de-costela.png",
                48.90,
                true,
                ETipoPrato.Comida),
            new(
                Guid.Parse("21000000-0000-0000-0000-000000000002"),
                "Salada da casa",
                "Entrada leve usada como dado previsivel do catalogo de exemplo.",
                "seed-salada-da-casa.png",
                29.50,
                true,
                ETipoPrato.Comida),
            new(
                Guid.Parse("21000000-0000-0000-0000-000000000003"),
                "Suco de maracuja",
                "Bebida de desenvolvimento para cobrir o tipo bebida.",
                "seed-suco-de-maracuja.png",
                12.00,
                true,
                ETipoPrato.Bebida),
            new(
                Guid.Parse("21000000-0000-0000-0000-000000000004"),
                "Pudim classico",
                "Sobremesa de desenvolvimento para cobrir o tipo sobremesa.",
                "seed-pudim-classico.png",
                18.00,
                true,
                ETipoPrato.Sobremesa)
        ];

        public static IReadOnlyList<DevelopmentSeedMesa> Mesas { get; } =
        [
            new(Guid.Parse("22000000-0000-0000-0000-000000000001"), "DEV-01", 2, true, ELocalizacaoMesa.Interna),
            new(Guid.Parse("22000000-0000-0000-0000-000000000002"), "DEV-02", 4, true, ELocalizacaoMesa.Interna),
            new(Guid.Parse("22000000-0000-0000-0000-000000000003"), "DEV-EXT", 6, true, ELocalizacaoMesa.Externa)
        ];

        public static DevelopmentSeedAtendente Atendente { get; } =
            new(Guid.Parse("23000000-0000-0000-0000-000000000001"), "Atendente Desenvolvimento", ETipoAtendente.Garcom);

        public static DevelopmentSeedPedido Pedido { get; } =
            new(
                Guid.Parse("24000000-0000-0000-0000-000000000001"),
                Atendente.Id,
                Mesas[0].Id,
                "DEV-PED-001",
                new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc));

        public static IReadOnlyList<DevelopmentSeedPedidoPrato> PedidoPratos { get; } =
        [
            new(
                Guid.Parse("25000000-0000-0000-0000-000000000001"),
                Pedido.Id,
                Pratos[0].Id,
                EStatusProducao.NaFilaDeProducao,
                "Item principal do pedido de desenvolvimento."),
            new(
                Guid.Parse("25000000-0000-0000-0000-000000000002"),
                Pedido.Id,
                Pratos[2].Id,
                EStatusProducao.NaFilaDeProducao,
                "Bebida do pedido de desenvolvimento.")
        ];
    }

    public sealed record DevelopmentSeedPrato(
        Guid Id,
        string Titulo,
        string Descricao,
        string Foto,
        double Preco,
        bool Ativo,
        ETipoPrato TipoPrato);

    public sealed record DevelopmentSeedMesa(
        Guid Id,
        string Numero,
        int Lugares,
        bool Ativo,
        ELocalizacaoMesa LocalizacaoMesa);

    public sealed record DevelopmentSeedAtendente(
        Guid Id,
        string Nome,
        ETipoAtendente TipoAtendente);

    public sealed record DevelopmentSeedPedido(
        Guid Id,
        Guid AtendenteId,
        Guid MesaId,
        string Numero,
        DateTime DataHoraCadastro);

    public sealed record DevelopmentSeedPedidoPrato(
        Guid Id,
        Guid PedidoId,
        Guid PratoId,
        EStatusProducao StatusProducao,
        string Observacao);
}
